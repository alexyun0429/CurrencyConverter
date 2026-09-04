namespace CurrencyConverter.Web.Models;

public class RatesViewModel
{
    public required string BaseCode { get; init; }
    public List<Currency> Currencies { get; init; } = new();
    public List<RateRow> Rows { get; init; } = new();
    public string Source { get; init; } = "";
    public DateOnly? RateDate { get; init; }
    public string? ErrorMessage { get; init; }
}
