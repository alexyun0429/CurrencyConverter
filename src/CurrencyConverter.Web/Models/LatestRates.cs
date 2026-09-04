namespace CurrencyConverter.Web.Models;

public record LatestRates(DateOnly Date, Dictionary<string, decimal> Rates);
