using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExchangeRatesApi.Features.ExchangeRate.Query;
using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;

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
        public Task<List<Currency>> GetCurrencyList(string apiCode, DateTime date, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _mediator.Send(new GetCurrencyList(apiCode), cancellationToken);
        }

        [HttpGet]
        public Task<List<ExchangeRateApiType>> GetExchangeRateApiList(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _mediator.Send(new GetExchangeRateApiList(), cancellationToken);
        }

        [HttpGet]
        public Task<CurrencyRateResponseDto> CalculateCurrencyRate(
            string apiCode, string sourceCurrency, string targetCurrency, DateTime DateFrom, DateTime DateTo, CancellationToken cancellationToken)
        {
            return _mediator.Send(
                new CalculateCurrencyRate() {
                    ApiCode = apiCode,
                    SourceCurrency = sourceCurrency, 
                    TargetCurrency = targetCurrency,
                    DateFrom = DateFrom,
                    DateTo = DateTo
            }, cancellationToken);
        }
    }
}
