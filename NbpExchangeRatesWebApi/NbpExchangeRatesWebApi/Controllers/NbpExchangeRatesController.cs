using Microsoft.AspNetCore.Mvc;
using NbpExchangeRatesWebApi.Dtos;
using NbpExchangeRatesWebApi.Interfaces;

namespace NbpExchangeRatesWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyRatesController(IExchangeRateService service)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ExchangeRatesTableDto>> Get(CancellationToken cancellationToken)
    {
        var exchangeRates = await service.GetAndSaveExchangeRates(cancellationToken);

        if (exchangeRates == null)
        {
            return NotFound(new { Message = "No exchange rates available from both NBP API and internal database." });
        }

        return Ok(exchangeRates);
    }
}