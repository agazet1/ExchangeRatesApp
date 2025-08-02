using ExchangeRatesApi.DTOs;
using ExchangeRatesApi.Exceptions;
using ExchangeRatesApi.Features.ExchangeRate.Query;
using MediatR;

namespace ExchangeRatesApi.Features.ExchangeRate.QueryHandlers
{
    internal class CalculateCurrencyRateHandler : IRequestHandler<CalculateCurrencyRate, CurrencyRateResponseDto>
    {
        private readonly IExchangeRateService _exchangeRateService;

        public CalculateCurrencyRateHandler(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public async Task<CurrencyRateResponseDto> Handle(CalculateCurrencyRate request, CancellationToken cancellationToken)
        {
            if (request.DateFrom.Date > DateTime.Now.Date || request.DateTo.Date > DateTime.Now.Date ||
                request.DateFrom.Date > request.DateTo.Date)
            {
                throw new BadRequestException("Incorrect date selected.");
            }

            if (string.IsNullOrEmpty(request.ApiCode))
            {
                throw new BadRequestException("Api code is required.");
            }

            if (string.IsNullOrEmpty(request.SourceCurrency) || string.IsNullOrEmpty(request.TargetCurrency))
            {
                throw new BadRequestException("Currency is required.");
            }

            var apiType = _exchangeRateService.GetApiType(request.ApiCode);
            var rate = await _exchangeRateService.CalculateCurrencyRate(apiType, request.SourceCurrency, request.TargetCurrency, request.DateFrom, request.DateTo, cancellationToken);

            return rate;
        }
    }
}
