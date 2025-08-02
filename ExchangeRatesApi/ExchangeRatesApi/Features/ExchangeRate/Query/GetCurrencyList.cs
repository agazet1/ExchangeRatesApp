using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;
using MediatR;

namespace ExchangeRatesApi.Features.ExchangeRate.Query
{
    public record GetCurrencyList(string apiCode) : IRequest<List<CurrencyDto>>
    {
    }
}
