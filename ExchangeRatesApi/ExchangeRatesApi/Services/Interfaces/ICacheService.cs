using ExchangeRatesApi.Data.Models;

namespace ExchangeRatesApi.Services.Interfaces
{
    internal interface ICacheService
    {
        T? GetData<T>(string key) where T : class;
        void AddData(string key, object data);
    }
}
