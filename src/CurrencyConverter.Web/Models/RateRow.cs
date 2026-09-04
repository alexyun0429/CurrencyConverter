namespace CurrencyConverter.Web.Models;

public class RateRow
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required decimal Rate { get; init; }
    public List<decimal> Trend { get; init; } = new();
    public decimal ChangePercent { get; init; }
}
