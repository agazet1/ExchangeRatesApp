using ExchangeRatesApi.Data.Models;
using MediatR;

namespace ExchangeRatesApi.Features.ExchangeRate.Query
{
    public record GetExchangeRateApiList() : IRequest<List<ExchangeRateApiType>>
    {
    }
}
