using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LoopBack.Sdk.Xamarin.Extensions;
using LoopBack.Sdk.Xamarin.Shared;
using ModernHttpClient;
using Newtonsoft.Json;

namespace LoopBack.Sdk.Xamarin.Remoting.Adapters
{
    /// <summary>
    ///     A specific <see cref="AdapterBase" /> implementation for RESTful servers.
    ///     In addition to implementing the <see cref="AdapterBase" /> interface,
    ///     <code>RestAdapter</code> contains a single <see cref="RestContract" /> to map
    ///     remote methods to custom HTTP routes. This is only required if the HTTP
    ///     settings have been customized on the server. When in doubt, try without. <see cref="RestContract" />
    /// </summary>
    public class RestAdapter : AdapterBase
    {
        private static string TAG = "remoting.RestAdapter";
        public virtual IContext ApplicationContext { get; protected set; }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        public RestAdapter(IContext context, string url)
            : base(url)
        {
            ApplicationContext = context;
            Contract = new RestContract();
            AddUserAgentHeaderToHttpClient();
        }

        private void AddUserAgentHeaderToHttpClient()
        {
            if (Client != null)
            {
                Client.DefaultRequestHeaders.Add("User-Agent", ApplicationContext.UserAgent);
            }
        }

        public RestAdapter(string url)
            : base(url)
        {
            ApplicationContext = new RestContext("loopback-xamarin/1.0");
            Contract = new RestContract();
            AddUserAgentHeaderToHttpClient();
        }

        /// <summary>
        ///     This adapter's <see cref="RestContract">adapter</see>, a custom contract for fine-grained route configuration.
        /// </summary>
        public RestContract Contract { get; set; }

        /// <summary>
        ///     The underlying HTTP client. This allows subclasses to add  custom headers like Authorization.
        /// </summary>
        public HttpClient Client { get; private set; }

        public override void Connect(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Client = null;
            }
            else
            {
                Client = new HttpClient(new NativeMessageHandler());
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
            }
        }

        public override bool IsConnected()
        {
            return Client != null;
        }

        public override async Task InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError)
        {
            await InvokeStaticMethodImpl(method, parameters,
                async response => { await Callback(onSuccess, onError, response); });
        }

        public override async Task InvokeStaticMethod(string method, Dictionary<string, object> parameters, Action<byte[], string> onSuccess, Action<Exception> onError)
        {
            //TODO: Fix
            //Client.DefaultRequestHeaders.Accept.Clear();
            //Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

            await InvokeStaticMethodImpl(method, parameters,
                async response => { await BinaryCallback(onSuccess, onError, response); });
        }


        public override async Task InvokeInstanceMethod(string method, Dictionary<string, object> constructorParameters, Dictionary<string, object> parameters, Action<string> onSuccess, Action<Exception> onError)
        {
            await InvokeInstanceMethodImpl(method, constructorParameters, parameters,
                async response => { await Callback(onSuccess, onError, response); });
        }

        public override async Task InvokeInstanceMethod(string method, Dictionary<string, object> constructorParameters, Dictionary<string, object> parameters, Action<byte[], string> onSuccess, Action<Exception> onError)
        {
            await InvokeInstanceMethodImpl(method, constructorParameters, parameters,
                async response => { await BinaryCallback(onSuccess, onError, response); });
        }

        private async Task Callback(Action<string> onSuccess, Action<Exception> onError,
            HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    onSuccess(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception exception)
            {
                onError(exception);
            }
        }

        private async Task BinaryCallback(Action<byte[], string> onSuccess, Action<Exception> onError,
            HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    string contentType = null;

                    foreach (var header in response.Headers)
                    {
                        if (header.Key.Equals("content-type"))
                        {
                            contentType = header.Value.First();
                        }
                    }

                    onSuccess(await response.Content.ReadAsByteArrayAsync(), contentType);
                }
            }
            catch (Exception exception)
            {
                onError(exception);
            }
        }

        private async Task InvokeStaticMethodImpl(string method,
            Dictionary<string, object> parameters,
            Action<HttpResponseMessage> httpHandler)
        {
            if (Contract == null)
            {
                throw new InvalidOperationException("Invalid contract");
            }

            var verb = Contract.GetVerbForMethod(method);
            var path = Contract.GetUrlForMethod(method, parameters);
            var parameterEncoding = Contract.GetParameterEncodingForMethod(method);

            await Request(path, verb, parameters, parameterEncoding, httpHandler);
        }

        private async Task InvokeInstanceMethodImpl(string method,
            Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters,
            Action<HttpResponseMessage> httpHandler)
        {
            if (Contract == null)
            {
                throw new Exception("Invalid contract");
            }

            var combinedParameters = new Dictionary<string, object>();
            if (constructorParameters != null)
            {
                combinedParameters.AddRange(constructorParameters);
            }
            if (parameters != null)
            {
                combinedParameters.AddRange(parameters);
            }

            var verb = Contract.GetVerbForMethod(method);
            var path = Contract.GetUrlForMethod(method, combinedParameters);
            var parameterEncoding = Contract.GetParameterEncodingForMethod(method);

            await Request(path, verb, combinedParameters, parameterEncoding, httpHandler);
        }

        private async Task Request(string path, string verb, Dictionary<string, object> parameters,
            ParameterEncoding parameterEncoding, Action<HttpResponseMessage> responseHandler)
        {
            var skipBody = false;
            if (!IsConnected())
            {
                throw new Exception("Adapter not connected");
            }

            if (parameters != null)
            {
                if ("GET".Equals(verb) || "HEAD".Equals(verb) || "DELETE".Equals(verb))
                {
                    var flattenParameters = FlattenParameters(parameters);

                    var keyValues = new List<string>(flattenParameters.Count);
                    keyValues.AddRange(
                        flattenParameters.Select(param => string.Format("{0}={1}", param.Key, param.Value)));
                    path += "?" + string.Join("&", keyValues);
                    skipBody = true;
                }
            }

            //TODO: AFNetworking . Paul Betts, says "modernhttpclient" solves general needs.
            //TODO: Dispose HttpClient
            Client.BaseAddress = new Uri(Url);

            var method = new HttpMethod(verb);
            var request = new HttpRequestMessage(method, path);
            HttpContent content;
            switch (parameterEncoding)
            {
            case ParameterEncoding.FORM_URL:
                var listOfParams =
                    parameters.Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value.ToString()))
                        .AsEnumerable();
                content = new FormUrlEncodedContent(listOfParams);
                request.Content = content;
                break;
            case ParameterEncoding.JSON:
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!skipBody && parameters != null)
                {
                    content = new StringContent(JsonConvert.SerializeObject(parameters));
                    request.Content = content;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                break;
            case ParameterEncoding.FORM_MULTIPART:
                //TODO:
                content = new MultipartFormDataContent("");
                request.Content = content;
                break;
            default:
                throw new ArgumentOutOfRangeException("parameterEncoding");
            }

            responseHandler(await Client.SendAsync(request));
        }

        private Dictionary<string, object> FlattenParameters(Dictionary<string, object> parameters)
        {
            return FlattenParameters(null, parameters);
        }

        private Dictionary<string, object> FlattenParameters(string keyPrefix, Dictionary<string, object> parameters)
        {
            // This method converts nested maps into a flat list
            //   Input:  { "here": { "lat": 10, "lng": 20 }
            //   Output: { "here[lat]": 10, "here[lng]": 20 }

            var result = new Dictionary<string, object>();

            foreach (var entry in parameters)
            {
                var key = keyPrefix != null
                    ? keyPrefix + "[" + entry.Key + "]"
                    : entry.Key;

                var value = entry.Value as Dictionary<string, object>;
                if (value != null)
                {
                    result.AddRange(FlattenParameters(key, value));
                }
                else
                {
                    result.Add(key, entry.Value);
                }
            }

            return result;
        }
    }
}