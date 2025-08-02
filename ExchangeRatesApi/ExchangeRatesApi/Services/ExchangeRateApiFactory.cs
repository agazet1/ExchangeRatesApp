using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.Services.ExchangeApis;
using ExchangeRatesApi.Services.Interfaces;

namespace ExchangeRatesApi.Services
{
    internal class ExchangeRateApiFactory: IExchangeRateApiFactory
    {  
        private readonly HttpClient _httpClient;
        private readonly ICacheService _dailyCacheService;
        private ExchangeRateNBPApi _exchangeRateNBPApi;
        //private ExchangeRateAnotherApi _exchangeRateAnotherApi;

        public ExchangeRateApiFactory(HttpClient httpClient, ICacheService dailyCacheService)
        {
            _httpClient = httpClient;
            _dailyCacheService = dailyCacheService;
        }

        IExchangeRateApi IExchangeRateApiFactory.GetApi(ExchangeRateApiType? exchangeApiType)
        {
            if (exchangeApiType is null)
            {
                throw new Exception("Missing API type");
            }

            return exchangeApiType.Code switch
            {
                "NBP" =>
                    _exchangeRateNBPApi ??= new ExchangeRateNBPApi(exchangeApiType, _httpClient, _dailyCacheService),
                //"ANOTHER" =>
                //    _exchangeRateAnotherApi ??= new ExchangeRateAnotherApi(_service),
                _ => throw new NotImplementedException("Unknown api")
            };
        }
    }
}
