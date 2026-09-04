using CurrencyConverter.Web.Models;

namespace CurrencyConverter.Web.Services;

public interface IRateService
{
    Task<RatesViewModel> GetRatesAsync(string baseCode);
}
