using ExchangeRatesApi.DTOs;
using ExchangeRatesApi.Exceptions;
using ExchangeRatesApi.Features.ExchangeRate.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExchangeRatesApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExchangeRateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CurrencyDto>>> GetCurrencyList(string apiCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var response = await _mediator.Send(new GetCurrencyList(apiCode), cancellationToken);

                if (response == null || response.Count == 0)
                {
                    return NoContent();
                }

                return Ok(response);
            }
            catch (Exception ex) when (ex is ValidationException || ex is BadRequestException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<ExchangeRateApiTypeDto>>> GetExchangeRateApiList(CancellationToken cancellationToken = default(CancellationToken))
        {
            try 
            { 
                var response = await _mediator.Send(new GetExchangeRateApiList(), cancellationToken);

                if (response == null || response.Count == 0)
                {
                    return NoContent();
                }

                return Ok(response);
            }
            catch (Exception ex) when (ex is ValidationException || ex is BadRequestException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                 return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<CurrencyRateResponseDto>> CalculateCurrencyRate(
            string apiCode, string sourceCurrency, string targetCurrency, DateTime DateFrom, DateTime DateTo, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(
                    new CalculateCurrencyRate()
                    {
                        ApiCode = apiCode,
                        SourceCurrency = sourceCurrency,
                        TargetCurrency = targetCurrency,
                        DateFrom = DateFrom,
                        DateTo = DateTo
                    }, cancellationToken);

                if (response == null || response.RateList is null || response.RateList.Count == 0)
                {
                    return NoContent();
                }

                return Ok(response);
            }
            catch (Exception ex) when (ex is ValidationException || ex is BadRequestException )
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
