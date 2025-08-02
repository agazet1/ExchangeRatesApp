
using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;
using ExchangeRatesApi.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ExchangeRatesApi.Services.ExchangeApis
{
    internal class ExchangeRateNBPApi : IExchangeRateApi
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _dailyCacheService;
        private readonly List<string> tableApi = new List<string>() { "A", "B" };
        private readonly string currencyListCacheKey = "CurrencyListNBP";

        public ExchangeRateNBPApi(ExchangeRateApiType exchangeApiType, HttpClient httpClient, ICacheService dailyCacheService)
        {
            _dailyCacheService = dailyCacheService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(exchangeApiType.Url);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Currency>> GetCurrencyList(CancellationToken cancellationToken)
        {
            var currencyCacheList = _dailyCacheService.GetData<List<Currency>>(currencyListCacheKey);
            if (currencyCacheList is not null)
            {
                return currencyCacheList;
            }

            var currencyList = new List<Currency>();
            foreach (var table in tableApi)
            {
                string endpoint = $"api/exchangerates/tables/{table}/";
                var json = "";
                try 
                {     
                    var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    json = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e) when(e.StatusCode == System.Net.HttpStatusCode.NotFound || e.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return currencyList;
                }

                var tables = JsonConvert.DeserializeObject<List<ApiExchangeRatesTable>>(json);

                if (tables != null && tables.Count > 0)
                {
                    foreach (var rate in tables[0].Rates)
                    {
                        currencyList.Add(new Currency
                        {
                            Code = rate.Code,
                            Name = rate.Currency,
                            ApiTable = table
                        });
                    }
                }
            };

            _dailyCacheService.AddData(currencyListCacheKey, currencyList);
            return currencyList;
        }

        public async Task<List<RateForDate>> GetMidRatesForDateList(string currencyCode, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            var currencyTable = await GetTableForCurrencyAsync(currencyCode, cancellationToken);
            string endpoint = $"api/exchangerates/rates/{currencyTable}/{currencyCode}/{FormatRequestDate(dateFrom)}/{FormatRequestDate(dateTo)}/";

            var rateForDayList = new List<RateForDate>();
            string json = "";
            try
            {
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound || e.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return rateForDayList;
            }
                
            var rateTable = JsonConvert.DeserializeObject<ApiExchangeRatesForCurrency>(json);
            if (rateTable is null)
                return rateForDayList;

            foreach (var rate in rateTable.Rates)
            {
                rateForDayList.Add(new RateForDate
                {
                    Date = DateTime.Parse(rate.EffectiveDate),
                    Rate = rate.Mid,
                });
            }
            return rateForDayList;
        }

        private async Task<string> GetTableForCurrencyAsync(string currencyCode, CancellationToken cancellationToken)
        {
            var allCurrency = await GetCurrencyList(cancellationToken);
            var currency = allCurrency.FirstOrDefault(x => x.Code == currencyCode);
            if (currency is null || string.IsNullOrEmpty(currency.ApiTable))
            {
                throw new Exception("Currency not find.");
            }
            return currency.ApiTable;
        }

        private string FormatRequestDate(DateTime date)
        {
            return date.Date.ToString("yyyy-MM-dd");
        }

        private class ApiExchangeRatesTable
        {
            public string Table { get; set; }
            public string No { get; set; }
            public string EffectiveDate { get; set; }
            public List<ApiRate> Rates { get; set; }
        }

        private class ApiRate
        {
            public string Currency { get; set; }
            public string Code { get; set; }
            public decimal Mid { get; set; }
        }

        private class ApiExchangeRatesForCurrency
        {
            public string Table { get; set; }
            public string Currency { get; set; }
            public string Code { get; set; }
            public List<ApiRateForCurrency> Rates { get; set; }
        }

        private class ApiRateForCurrency
        {
            public string No { get; set; }
            public string EffectiveDate { get; set; }
            public decimal Mid { get; set; }
        }
    }
}
