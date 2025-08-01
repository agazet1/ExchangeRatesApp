using ExchangeRatesApi.DTOs;
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
            if (request.DateFrom > DateTime.Now.Date || request.DateTo > DateTime.Now.Date ||
                request.DateFrom > request.DateTo)
            {
                throw new Exception("Incorrect date selected.");
            }

            var apiType = _exchangeRateService.GetApiType(request.ApiCode);
            var rate = await _exchangeRateService.CalculateCurrencyRate(apiType, request.SourceCurrency, request.TargetCurrency, request.DateFrom, request.DateTo, cancellationToken);

            if (rate is null || rate.RateList is null || !rate.RateList.Any())
            {
                return (CurrencyRateResponseDto)Results.NoContent();
            }
            return rate;
        }
    }
}
