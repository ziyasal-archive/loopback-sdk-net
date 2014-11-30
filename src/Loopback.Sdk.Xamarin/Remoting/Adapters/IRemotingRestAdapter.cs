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

        string Url { get; set; }
        void Connect(string url);
        bool IsConnected();

        Task<RemotingResponse> InvokeInstanceMethod(Dictionary<string, object> parameters, string path, string verb,
            AdapterBase.ParameterEncoding parameterEncoding);

        Task<RemotingResponse> InvokeStaticMethod(Dictionary<string, object> parameters, string path, string verb,
            AdapterBase.ParameterEncoding parameterEncoding);
    }
}