using CurrencyConverter.Web.Models;

namespace CurrencyConverter.Web.Services;

public interface IExchangeRateClient
{
    Task<List<Currency>> GetCurrenciesAsync();
    Task<LatestRates> GetLatestAsync(string baseCode);
    Task<Dictionary<string, List<decimal>>> GetHistoryAsync(string baseCode, DateOnly from, DateOnly to);
}
