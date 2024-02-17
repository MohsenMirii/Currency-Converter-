using MediatR;
using Microsoft.AspNetCore.Mvc;
using Share.ConversionRates.Commands;
using Share.ConversionRates.Queries;

namespace API.Controllers;

[ApiController]
[Route("conversion-rates")]
public class ConversionRatesController : ControllerBase {
    private readonly IMediator _mediator;

    public ConversionRatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<Unit> UpdateConfiguration([FromBody] UpdateConfigurationCommand input)
    {
        return await _mediator.Send(input);
    }

    [HttpGet("convert-currency")]
    public async Task<ConvertCurrencyResponse> Convert([FromQuery] ConvertCurrencyQuery input)
    {
        return await _mediator.Send(input);
    }

    [HttpDelete]
    public async Task<Unit> ClearConfiguration()
    {
        return await _mediator.Send(new ClearConfigurationCommand());
    }
}