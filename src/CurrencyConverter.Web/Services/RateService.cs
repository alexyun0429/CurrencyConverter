using CurrencyConverter.Web.Models;

namespace CurrencyConverter.Web.Services;

public class RateService : IRateService
{
    private const string Source = "European Central Bank (via Frankfurter)";
    private readonly IExchangeRateClient _client;

    public RateService(IExchangeRateClient client)
    {
        _client = client;
    }

    public async Task<RatesViewModel> GetRatesAsync(string baseCode)
    {
        var currencies = await _client.GetCurrenciesAsync();
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

        var latest = await _client.GetLatestAsync(code);
        var names = currencies.ToDictionary(c => c.Code, c => c.Name);

        var rows = latest.Rates
            .Select(kv => new RateRow
            {
                Code = kv.Key,
                Name = names.GetValueOrDefault(kv.Key, kv.Key),
                Rate = kv.Value
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
}
