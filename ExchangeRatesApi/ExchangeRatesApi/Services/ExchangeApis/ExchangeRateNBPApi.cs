
using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ExchangeRatesApi.Services.ExchangeApis
{
    internal class ExchangeRateNBPApi : IExchangeRateApi
    {
        private readonly HttpClient _httpClient;
        private readonly List<string> tableApi = new List<string>() { "A", "B", "C" };

        public ExchangeRateNBPApi(ExchangeRateApiType exchangeApiType, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(exchangeApiType.Url);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Currency>> GetCurrencyList(CancellationToken cancellationToken)
        {
            var currencyList = new List<Currency>();

            foreach (var table in tableApi)
            {
                string endpoint = $"api/exchangerates/tables/{table}/";
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

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
            return currencyList;
        }

        public async Task<List<RateForDate>> GetMidRatesForDateList(string currencyCode, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            //TODO
            var currencyTable = await GetTableForCurrencyAsync(currencyCode, cancellationToken);
            string endpoint = $"api/exchangerates/rates/{currencyTable}/{currencyCode}/{FormatRequestDate(dateFrom)}/{FormatRequestDate(dateTo)}/";
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var rateTable = JsonConvert.DeserializeObject<ApiExchangeRatesForCurrency>(json);

            var rateForDayList = new List<RateForDate>();
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
