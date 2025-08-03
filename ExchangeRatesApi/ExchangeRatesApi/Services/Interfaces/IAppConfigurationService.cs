using ExchangeRatesApi.Data.Models;

namespace ExchangeRatesApi.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        AppConfiguration GetAppConfigurations();
        List<ExchangeRateApiType> GetExchangeRateApiList();
        CurrencyOrderEnum GetCurrencyOrder();
        string GetDateFormat();
    }
}
