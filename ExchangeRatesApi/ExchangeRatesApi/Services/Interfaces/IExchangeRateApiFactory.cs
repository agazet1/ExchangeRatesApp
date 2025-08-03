using ExchangeRatesApi.Data.Models;

namespace ExchangeRatesApi.Services
{
    public interface IExchangeRateApiFactory
    {
        IExchangeRateApi GetApi(ExchangeRateApiType? exchangeApiType);
    }
}
