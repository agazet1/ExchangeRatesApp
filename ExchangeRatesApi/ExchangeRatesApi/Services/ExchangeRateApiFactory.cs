using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.Services.ExchangeApis;

namespace ExchangeRatesApi.Services
{
    internal class ExchangeRateApiFactory: IExchangeRateApiFactory
    {
        private ExchangeRateNBPApi _exchangeRateNBPApi;
        //private ExchangeRateAnotherApi _exchangeRateAnotherApi;
        private HttpClient _httpClient;

        public ExchangeRateApiFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                    _exchangeRateNBPApi ??= new ExchangeRateNBPApi(exchangeApiType, _httpClient),
                //"ANOTHER" =>
                //    _exchangeRateAnotherApi ??= new ExchangeRateAnotherApi(_service),
                _ => throw new NotImplementedException()
            };
        }
    }
}
