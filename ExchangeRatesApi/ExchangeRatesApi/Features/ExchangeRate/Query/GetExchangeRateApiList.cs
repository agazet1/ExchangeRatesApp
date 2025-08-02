using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;
using MediatR;

namespace ExchangeRatesApi.Features.ExchangeRate.Query
{
    public record GetExchangeRateApiList() : IRequest<List<ExchangeRateApiTypeDto>>
    {
    }
}
