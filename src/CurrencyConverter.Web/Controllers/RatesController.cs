using CurrencyConverter.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Web.Controllers;

public class RatesController : Controller
{
    private readonly IRateService _rates;

    public RatesController(IRateService rates)
    {
        _rates = rates;
    }

    public async Task<IActionResult> Index(string? baseCode)
    {
        var model = await _rates.GetRatesAsync(baseCode ?? "AUD");
        return View(model);
    }
}
