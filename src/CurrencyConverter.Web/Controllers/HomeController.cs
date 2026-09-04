using System.Diagnostics;
using CurrencyConverter.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Web.Controllers;

public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
