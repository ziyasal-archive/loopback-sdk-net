using Loopback.Sdk.Xamarin.Remoting;
using Newtonsoft.Json;

namespace Loopback.Sdk.Xamarin.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        public static T ReadAs<T>(this RemotingResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}