using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;
using ExchangeRatesApi.Services;
using ExchangeRatesApi.Services.Interfaces;

internal class ExchangeRateService : IExchangeRateService
{
    private readonly IExchangeRateApiAdapter _apiAdapter;
    private readonly IAppConfigurationService _appConfigService;
    public ExchangeRateService(IExchangeRateApiAdapter apiAdapter, IAppConfigurationService appConfigService)
    {
        _apiAdapter = apiAdapter;
        _appConfigService = appConfigService;
    }

    public async Task<List<Currency>> GetCurrencyList(ExchangeRateApiType exchangeRateApiType, CancellationToken cancellationToken)
    {
        var currencyList = await _apiAdapter.GetCurrencyList(exchangeRateApiType, cancellationToken);

        return _appConfigService.GetCurrencyOrder() switch
        {
            CurrencyOrderEnum.CODE_ASC => currencyList.OrderBy(x => x.Code).ToList(),
            CurrencyOrderEnum.CODE_DESC => currencyList.OrderByDescending(x => x.Code).ToList(),
            CurrencyOrderEnum.NAME_ASC => currencyList.OrderBy(x => x.Name).ToList(),
            CurrencyOrderEnum.NAME_DESC => currencyList.OrderByDescending(x => x.Name).ToList(),
            _ => throw new NotImplementedException()
        };
    }

    public ExchangeRateApiType GetApiType(string apiCode)
    {
        if (string.IsNullOrEmpty(apiCode))
        {
            throw new Exception("Missing exchange rate API code");
        }

        var apiType = _appConfigService.GetExchangeRateApiList().FirstOrDefault(x => x.Code == apiCode);

        if (apiType is null)
        {
            throw new Exception($"No configured exchange rate API with code: {apiCode}");
        }

        if (string.IsNullOrEmpty(apiType.Url) || !Uri.TryCreate(apiType.Url, UriKind.Absolute, out var validatedUri))
        {
            throw new Exception($"Incorrect exchange rate API address for code: {apiType.Code}");
        }

        return apiType;
    }

    public async Task<CurrencyRateResponseDto> CalculateCurrencyRate(ExchangeRateApiType exchangeRateApiType, string sourceCurrencyCode, string targetCurrencyCode, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
    {
        string dateFormat = _appConfigService.GetDateFormat();

        var midRatesSourceCurrency = await _apiAdapter.GetExchangeRatesForDatesList(exchangeRateApiType, sourceCurrencyCode, dateFrom, dateTo, cancellationToken);
        var midRatesTargetCurrency = await _apiAdapter.GetExchangeRatesForDatesList(exchangeRateApiType, targetCurrencyCode, dateFrom, dateTo, cancellationToken);

        if(midRatesSourceCurrency == null || midRatesTargetCurrency == null ||
            midRatesSourceCurrency.Count == 0 || midRatesTargetCurrency.Count == 0)
        {
            return null;
        }

        CurrencyRateResponseDto rate = new CurrencyRateResponseDto();
        rate.SourceCurrencyCode = sourceCurrencyCode;
        rate.TargetCurrencyCode = targetCurrencyCode;
        rate.DateFrom = dateFrom;
        rate.DateTo = dateTo;

        var rateList = new List<RateForDateDto>();

        for (DateTime date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
        {
            var rate1 = midRatesSourceCurrency.FirstOrDefault(x => x.Date == date);
            if (rate1 is null)
                continue;
            var rate2 = midRatesTargetCurrency.FirstOrDefault(x => x.Date == date);
            if (rate2 is null)
                continue;

            var currencyRateFrom2To = rate1.Rate / rate2.Rate; // e.g. 1 USD = 1.3684 GBP (rate)
            rateList.Add(new RateForDateDto() { Date = date.ToString(dateFormat), Rate = currencyRateFrom2To });
        }

        rate.RateList = rateList;

        var allRates = rateList.Select(x => x.Rate);
        rate.AvgRate = allRates.Average();
        rate.MinRate = allRates.Min();
        rate.MaxRate = allRates.Max();

        return rate;
    }
}