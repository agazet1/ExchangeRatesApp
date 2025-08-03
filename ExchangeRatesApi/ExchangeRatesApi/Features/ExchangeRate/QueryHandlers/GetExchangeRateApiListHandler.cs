using MediatR;
using ExchangeRatesApi.Features.ExchangeRate.Query;
using ExchangeRatesApi.Services.Interfaces;
using ExchangeRatesApi.DTOs;

namespace ExchangeRatesApi.Features.ExchangeRate.QueryHandlers
{
    internal class GetExchangeRateApiListHandler : IRequestHandler<GetExchangeRateApiList, List<ExchangeRateApiTypeDto>>
    {
        private readonly IAppConfigurationService _appConfigService;

        public GetExchangeRateApiListHandler(IAppConfigurationService appConfigService)
        {
            _appConfigService = appConfigService;
        }

        public Task<List<ExchangeRateApiTypeDto>> Handle(GetExchangeRateApiList request, CancellationToken cancellationToken)
        {
            var result = _appConfigService.GetExchangeRateApiList();
            List<ExchangeRateApiTypeDto> apiList = new List<ExchangeRateApiTypeDto>();
            result.ForEach(x => apiList.Add(new ExchangeRateApiTypeDto() { Code = x.Code, Name = x.Name }));

            return Task.FromResult(apiList);
        }
    }
}
