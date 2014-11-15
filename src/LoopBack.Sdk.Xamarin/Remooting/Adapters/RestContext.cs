using LoopBack.Sdk.Xamarin.Common;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    public class RestContext : IContext
    {
        public RestContext(string userAgent)
        {
            UserAgent = userAgent;
        }

        public string UserAgent { get; set; }
    }
}