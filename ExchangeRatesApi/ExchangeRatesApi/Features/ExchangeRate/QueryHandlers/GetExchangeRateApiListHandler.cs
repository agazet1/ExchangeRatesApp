using MediatR;
using ExchangeRatesApi.Features.ExchangeRate.Query;
using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.Services.Interfaces;

namespace ExchangeRatesApi.Features.ExchangeRate.QueryHandlers
{
    internal class GetExchangeRateApiListHandler : IRequestHandler<GetExchangeRateApiList, List<ExchangeRateApiType>>
    {
        private readonly IAppConfigurationService _appConfigService;

        public GetExchangeRateApiListHandler(IAppConfigurationService appConfigService)
        {
            _appConfigService = appConfigService;
        }

        public Task<List<ExchangeRateApiType>> Handle(GetExchangeRateApiList request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_appConfigService.GetExchangeRateApiList());
        }
    }
}
