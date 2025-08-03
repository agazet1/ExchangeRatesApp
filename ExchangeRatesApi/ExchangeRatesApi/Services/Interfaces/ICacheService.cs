namespace ExchangeRatesApi.Services.Interfaces
{
    public interface ICacheService
    {
        T? GetData<T>(string key) where T : class;
        void AddData(string key, object data);
    }
}
