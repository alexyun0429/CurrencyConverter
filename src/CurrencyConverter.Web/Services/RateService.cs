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
        var latest = await _client.GetLatestAsync(baseCode);
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
            BaseCode = baseCode,
            Currencies = currencies,
            Rows = rows,
            Source = Source,
            RateDate = latest.Date
        };
    }
}
