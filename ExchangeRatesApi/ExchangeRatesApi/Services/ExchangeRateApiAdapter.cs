
using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;

namespace ExchangeRatesApi.Services
{
    internal class ExchangeRateApiAdapter : IExchangeRateApiAdapter
    {
        private readonly IExchangeRateApiFactory _exchangeRateApiFactory;
        public ExchangeRateApiAdapter(IExchangeRateApiFactory exchangeRateApiFactory)
        {
            _exchangeRateApiFactory = exchangeRateApiFactory;
        }

        public Task<List<Currency>> GetCurrencyList(ExchangeRateApiType apiType, CancellationToken cancellationToken)
        {
            return GetManager(apiType).GetCurrencyList(cancellationToken);
        }

        public Task<List<RateForDate>> GetExchangeRatesForDatesList(ExchangeRateApiType apiType, string currencyCode, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            return GetManager(apiType).GetMidRatesForDateList(currencyCode, dateFrom, dateTo, cancellationToken);
        }

        private IExchangeRateApi GetManager(ExchangeRateApiType apiType) => _exchangeRateApiFactory.GetApi(apiType);
    }
}
