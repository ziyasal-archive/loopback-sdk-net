using Loopback.Sdk.Xamarin.Shared;

namespace Loopback.Sdk.Xamarin.Test.Loopback
{
    //TODO: Mock
    public class DummyStorageService : IStorageService
    {
        public void Save(string key, object value)
        {
        }

        public T Get<T>(string key)
        {
            return default(T);
        }
    }
}