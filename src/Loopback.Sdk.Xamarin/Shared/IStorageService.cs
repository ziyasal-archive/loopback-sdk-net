namespace Loopback.Sdk.Xamarin.Shared
{
    public interface IStorageService
    {
        void Save(string key, object value);
        T Get<T>(string key);
    }
}