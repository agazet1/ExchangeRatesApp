using ExchangeRatesApi.DTOs;
using MediatR;

namespace ExchangeRatesApi.Features.ExchangeRate.Query
{
    public class CalculateCurrencyRate() : IRequest<CurrencyRateResponseDto>
    {
        public string ApiCode { get; set; }
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
