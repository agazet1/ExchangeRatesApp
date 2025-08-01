using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;

internal class AppConfigurationService : IAppConfigurationService
{
    private readonly AppConfiguration _appConfig;
    public AppConfigurationService(IOptions<AppConfiguration> options)
    {
        _appConfig = options.Value;
    }

    public AppConfiguration GetAppConfigurations() => _appConfig;
    public CurrencyOrderEnum GetCurrencyOrder() => _appConfig.CurrencyOrder;
    public string GetDateFormat() => _appConfig.DateFormat;
    public List<ExchangeRateApiType> GetExchangeRateApiList() => _appConfig.ExchangeRateApiList;
    
}