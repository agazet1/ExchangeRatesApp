using ExchangeRatesApi.Data.Models;
using MediatR;

namespace ExchangeRatesApi.Features.ExchangeRate.Query
{
    public record GetCurrencyList(string apiCode) : IRequest<List<Currency>>
    {
    }
}
