using CurrencyConverter.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Web.Controllers;

[ApiController]
[Route("api/rates")]
public class RatesApiController : ControllerBase
{
    private readonly IRateService _rates;

    public RatesApiController(IRateService rates)
    {
        _rates = rates;
    }

    [HttpGet("{baseCode}")]
    public async Task<IActionResult> Get(string baseCode)
    {
        var model = await _rates.GetRatesAsync(baseCode);

        if (model.ErrorMessage != null)
        {
            return BadRequest(new { error = model.ErrorMessage });
        }

        return Ok(new
        {
            @base = model.BaseCode,
            date = model.RateDate,
            source = model.Source,
            rates = model.Rows.ToDictionary(r => r.Code, r => r.Rate)
        });
    }
}
