using System.Globalization;

namespace CurrencyConverter.Web.Helpers;

public static class Sparkline
{
    // Turns a series of rates into the "points" attribute of an SVG <polyline>.
    public static string Points(IReadOnlyList<decimal> values, int width = 100, int height = 24)
    {
        if (values.Count < 2)
        {
            return "";
        }

        var min = values.Min();
        var max = values.Max();
        var range = max - min;
        var points = new List<string>();

        for (var i = 0; i < values.Count; i++)
        {
            var x = (decimal)i / (values.Count - 1) * width;
            var normalised = range == 0 ? 0.5m : (values[i] - min) / range;
            var y = 1 + (height - 2) * (1 - normalised);

            points.Add(x.ToString("0.#", CultureInfo.InvariantCulture) + "," +
                       y.ToString("0.#", CultureInfo.InvariantCulture));
        }

        return string.Join(" ", points);
    }
}
