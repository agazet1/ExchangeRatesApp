namespace ExchangeRatesApi.DTOs
{
    public class CurrencyRateRequestDto
    {
        public string ApiCode { get; set; }
        public string CurrencyCodeFrom { get; set; }
        public string CurrencyCodeTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
