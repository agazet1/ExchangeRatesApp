using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;

internal interface IExchangeRateService
{
    Task<List<Currency>> GetCurrencyList(ExchangeRateApiType exchangeRateApiType, CancellationToken cancellationToken);
    ExchangeRateApiType GetApiType(string apiCode);
    Task<CurrencyRateResponseDto> CalculateCurrencyRate(ExchangeRateApiType exchangeRateApiType, string currencyCodeFrom, string currencyCodeTo, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken);
}