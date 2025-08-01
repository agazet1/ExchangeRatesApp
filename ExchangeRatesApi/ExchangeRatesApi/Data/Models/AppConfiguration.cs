namespace ExchangeRatesApi.Data.Models
{
    public class AppConfiguration
    {
        public List<ExchangeRateApiType> ExchangeRateApiList { get; set; }
        public string DateFormat { get; set; }
        public CurrencyOrderEnum CurrencyOrder { get; set; }
    }
}
