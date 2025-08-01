using ExchangeRatesApi.Data.Models;

namespace ExchangeRatesApi.Services
{
    internal interface IExchangeRateApiFactory
    {
        IExchangeRateApi GetApi(ExchangeRateApiType? exchangeApiType);
    }
}
