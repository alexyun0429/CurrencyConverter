using System.Net.Http.Json;
using CurrencyConverter.Web.Models;

namespace CurrencyConverter.Web.Services;

public class FrankfurterClient : IExchangeRateClient
{
    private readonly HttpClient _http;

    public FrankfurterClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Currency>> GetCurrenciesAsync()
    {
        var data = await _http.GetFromJsonAsync<Dictionary<string, string>>("currencies")
                   ?? new Dictionary<string, string>();

        return data.Select(kv => new Currency(kv.Key, kv.Value))
                   .OrderBy(c => c.Code)
                   .ToList();
    }

    public async Task<LatestRates> GetLatestAsync(string baseCode)
    {
        var data = await _http.GetFromJsonAsync<LatestResponse>($"latest?base={baseCode}")
                   ?? throw new InvalidOperationException("Empty response from rate source.");

        return new LatestRates(DateOnly.Parse(data.Date), data.Rates);
    }

    public async Task<Dictionary<string, List<decimal>>> GetHistoryAsync(string baseCode, DateOnly from, DateOnly to)
    {
        // Omitting "symbols" returns every currency, so one request covers the whole table.
        var url = $"{from:yyyy-MM-dd}..{to:yyyy-MM-dd}?base={baseCode}";
        var data = await _http.GetFromJsonAsync<HistoryResponse>(url)
                   ?? throw new InvalidOperationException("Empty response from rate source.");

        var result = new Dictionary<string, List<decimal>>();

        foreach (var day in data.Rates.OrderBy(d => d.Key))
        {
            foreach (var (code, rate) in day.Value)
            {
                if (!result.TryGetValue(code, out var series))
                {
                    series = new List<decimal>();
                    result[code] = series;
                }
                series.Add(rate);
            }
        }

        return result;
    }

    private record LatestResponse(string Base, string Date, Dictionary<string, decimal> Rates);
    private record HistoryResponse(Dictionary<string, Dictionary<string, decimal>> Rates);
}
