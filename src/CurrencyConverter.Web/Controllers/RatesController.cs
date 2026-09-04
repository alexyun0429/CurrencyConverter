using CurrencyConverter.Web.Models;
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
        try
        {
            var model = await _rates.GetRatesAsync(baseCode ?? "AUD");
            return View(model);
        }
        catch (HttpRequestException)
        {
            return View(new RatesViewModel
            {
                BaseCode = baseCode ?? "AUD",
                ErrorMessage = "The rate source is not reachable right now. Please try again in a moment."
            });
        }
    }
}
