using CurrencyConverter.Web.Models;
using CurrencyConverter.Web.Services;

namespace CurrencyConverter.Tests;

// Stands in for Frankfurter so tests never touch the network.
public class FakeExchangeRateClient : IExchangeRateClient
{
    public int LatestCalls { get; private set; }

    public Task<List<Currency>> GetCurrenciesAsync()
    {
        return Task.FromResult(new List<Currency>
        {
            new("AUD", "Australian Dollar"),
            new("JPY", "Japanese Yen"),
            new("USD", "United States Dollar")
        });
    }

    public Task<LatestRates> GetLatestAsync(string baseCode)
    {
        LatestCalls++;

        return Task.FromResult(new LatestRates(
            new DateOnly(2026, 9, 4),
            new Dictionary<string, decimal> { ["USD"] = 0.70m, ["JPY"] = 100m }));
    }

    public Task<Dictionary<string, List<decimal>>> GetHistoryAsync(string baseCode, DateOnly from, DateOnly to)
    {
        return Task.FromResult(new Dictionary<string, List<decimal>>
        {
            ["USD"] = new() { 0.60m, 0.65m, 0.70m },
            ["JPY"] = new() { 100m, 100m, 100m }
        });
    }
}
