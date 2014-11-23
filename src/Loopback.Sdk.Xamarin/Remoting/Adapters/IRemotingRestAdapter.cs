using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Loopback.Sdk.Xamarin.Shared;

namespace Loopback.Sdk.Xamarin.Remoting.Adapters
{
    public interface IRemotingRestAdapter
    {
        IContext ApplicationContext { get; }

        /// <summary>
        ///     The underlying HTTP client. This allows subclasses to add  custom headers like Authorization.
        /// </summary>
        HttpClient Client { get; }

        /// <summary>
        ///     This adapter's <see cref="RestContract">adapter</see>, a custom contract for fine-grained route configuration.
        /// </summary>
        RestContract Contract { get; set; }

        string Url { get; set; }
        void Connect(string url);
        bool IsConnected();

        Task<RemotingResponse> InvokeInstanceMethod(string method, Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters);

        Task<RemotingResponse> InvokeStaticMethod(string method, Dictionary<string, object> parameters);
    }
}