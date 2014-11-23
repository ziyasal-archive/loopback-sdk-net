namespace Loopback.Sdk.Xamarin.Shared
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