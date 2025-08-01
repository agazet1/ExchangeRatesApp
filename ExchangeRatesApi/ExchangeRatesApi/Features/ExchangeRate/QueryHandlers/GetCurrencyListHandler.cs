using MediatR;
using ExchangeRatesApi.Features.ExchangeRate.Query;
using ExchangeRatesApi.Data.Models;

namespace ExchangeRatesApi.Features.ExchangeRate.QueryHandlers
{
    internal class GetCurrencyListHandler : IRequestHandler<GetCurrencyList, List<Currency>>
    {
        private readonly IExchangeRateService _exchangeRateService;

        public GetCurrencyListHandler(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public async Task<List<Currency>> Handle(GetCurrencyList request, CancellationToken cancellationToken)
        {
            List<Currency> currencyList = new List<Currency>();

            var apiType = _exchangeRateService.GetApiType(request.apiCode);
            var result = await _exchangeRateService.GetCurrencyList(apiType, cancellationToken);

            return result;
        }
    }
}
