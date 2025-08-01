using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;

namespace ExchangeRatesApi.Services
{
    internal interface IExchangeRateApi
    {
        Task<List<Currency>> GetCurrencyList(CancellationToken cancellationToken);
        Task<List<RateForDate>> GetMidRatesForDateList(string currencyCode, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken);
    }
}