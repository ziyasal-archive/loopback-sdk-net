using Loopback.Sdk.Xamarin.Loopback;
using Loopback.Sdk.Xamarin.Remoting;

namespace Loopback.Sdk.Xamarin.Extensions
{
    public static class RestResponseExtensions
    {
        public static void FillFrom(this RestResponse response, RemotingResponse remotingResponse)
        {
            response.Content = remotingResponse.Content;
            response.Raw = remotingResponse.Raw;


            if (remotingResponse.Exception != null)
            {
                response.Exception = remotingResponse.Exception;
            }
        }
    }
}