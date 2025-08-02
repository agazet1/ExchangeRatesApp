using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;
using ExchangeRatesApi.Exceptions;
using ExchangeRatesApi.Features.ExchangeRate.Query;
using MediatR;

namespace ExchangeRatesApi.Features.ExchangeRate.QueryHandlers
{
    internal class GetCurrencyListHandler : IRequestHandler<GetCurrencyList, List<CurrencyDto>>
    {
        private readonly IExchangeRateService _exchangeRateService;

        public GetCurrencyListHandler(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public async Task<List<CurrencyDto>> Handle(GetCurrencyList request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.apiCode))
            {
                throw new BadRequestException("Api code is required.");
            }

            var apiType = _exchangeRateService.GetApiType(request.apiCode);
            var result = await _exchangeRateService.GetCurrencyList(apiType, cancellationToken);

            List<CurrencyDto> currList = new List<CurrencyDto>();
            result.ForEach(x => currList.Add(new CurrencyDto() { Code = x.Code, Name = x.Name }));

            return currList;
        }
    }
}
