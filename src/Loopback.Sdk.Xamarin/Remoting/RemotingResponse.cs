using System;
using System.Net.Http;

namespace Loopback.Sdk.Xamarin.Remoting
{
    public class RemotingResponse
    {
        public string Content { get; set; }
        public Exception Exception { get; set; }
        public HttpResponseMessage Raw { get; set; }

        public bool IsSuccessStatusCode
        {
            get { return Raw.IsSuccessStatusCode; }
        }
    }
}