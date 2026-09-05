using CurrencyConverter.Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConverter.Web.Services;

public class RateService : IRateService
{
    private const string Source = "European Central Bank (via Frankfurter)";
    private const int TrendDays = 30;
    private readonly IExchangeRateClient _client;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _ratesTtl;
    private readonly TimeSpan _currenciesTtl;

    public RateService(IExchangeRateClient client, IMemoryCache cache, IConfiguration config)
    {
        _client = client;
        _cache = cache;
        _ratesTtl = TimeSpan.FromMinutes(config.GetValue("Cache:RatesMinutes", 10));
        _currenciesTtl = TimeSpan.FromHours(config.GetValue("Cache:CurrenciesHours", 24));
    }

    public async Task<RatesViewModel> GetRatesAsync(string baseCode)
    {
        var currencies = await GetCurrenciesCachedAsync();
        var code = baseCode.Trim().ToUpperInvariant();

        // Allowlist: only a code the provider itself lists is ever put into an outbound URL.
        if (!currencies.Any(c => c.Code == code))
        {
            return new RatesViewModel
            {
                BaseCode = "AUD",
                Currencies = currencies,
                ErrorMessage = $"'{baseCode}' is not a supported currency."
            };
        }

        // Rates change once a day, so a short cache costs nothing in accuracy.
        return (await _cache.GetOrCreateAsync($"rates:{code}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ratesTtl;
            return BuildAsync(code, currencies);
        }))!;
    }

    private Task<List<Currency>> GetCurrenciesCachedAsync()
    {
        return _cache.GetOrCreateAsync("currencies", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _currenciesTtl;
            return _client.GetCurrenciesAsync();
        })!;
    }

    private async Task<RatesViewModel> BuildAsync(string code, List<Currency> currencies)
    {
        var latest = await _client.GetLatestAsync(code);
        var history = await _client.GetHistoryAsync(code, latest.Date.AddDays(-TrendDays), latest.Date);
        var names = currencies.ToDictionary(c => c.Code, c => c.Name);

        var rows = latest.Rates
            .Select(kv =>
            {
                var trend = history.GetValueOrDefault(kv.Key) ?? new List<decimal>();
                return new RateRow
                {
                    Code = kv.Key,
                    Name = names.GetValueOrDefault(kv.Key, kv.Key),
                    Rate = kv.Value,
                    Trend = trend,
                    ChangePercent = ChangePercent(trend)
                };
            })
            .OrderBy(r => r.Code)
            .ToList();

        return new RatesViewModel
        {
            BaseCode = code,
            Currencies = currencies,
            Rows = rows,
            Source = Source,
            RateDate = latest.Date
        };
    }

    private static decimal ChangePercent(List<decimal> trend)
    {
        if (trend.Count < 2 || trend[0] == 0)
        {
            return 0;
        }

        return (trend[^1] - trend[0]) / trend[0] * 100;
    }
}
