using ExchangeRatesApi.Features.ExchangeRate.Query;

namespace ExchangeRatesApi.DTOs
{
    public class CurrencyRateResponseDto
    {
        public string SourceCurrencyCode { get; set; }
        public string TargetCurrencyCode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal MinRate { get; set; }
        public decimal MaxRate { get; set; }
        public decimal AvgRate { get; set; }
        public List<RateForDateDto> RateList;
    }

    public class RateForDate
    {
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }

    public class RateForDateDto
    {
        public string Date { get; set; }
        public decimal Rate { get; set; }
    }
}
