using CurrencyConverter.Web.Helpers;

namespace CurrencyConverter.Tests;

public class SparklineTests
{
    [Fact]
    public void A_flat_series_draws_along_the_midline()
    {
        var points = Sparkline.Points(new[] { 1m, 1m, 1m }, width: 100, height: 24);

        // height 24 -> midline y = 12, for every point
        Assert.Equal("0,12 50,12 100,12", points);
    }

    [Fact]
    public void Fewer_than_two_values_produces_no_line()
    {
        Assert.Equal("", Sparkline.Points(new[] { 1m }));
    }
}
