using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;

namespace ExchangeRatesApi.Services
{
    public interface IExchangeRateApiAdapter
    {
        Task<List<Currency>> GetCurrencyList(ExchangeRateApiType apiType, CancellationToken cancellationToken);
        Task<List<RateForDate>> GetExchangeRatesForDatesList(ExchangeRateApiType apiType, string currencyCode, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken);
    }
}
