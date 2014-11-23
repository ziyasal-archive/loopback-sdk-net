using Loopback.Sdk.Xamarin.Remoting;

namespace Loopback.Sdk.Xamarin.Loopback
{
    public class RestResponse<T> : RestResponse
    {
        public T Result { get; set; }
    }

    public class RestResponse : RemotingResponse
    {
    }
}