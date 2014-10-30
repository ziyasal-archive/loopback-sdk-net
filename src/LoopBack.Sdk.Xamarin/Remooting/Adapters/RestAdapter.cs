using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LoopBack.Sdk.Xamarin.Common;
using ModernHttpClient;
using Newtonsoft.Json;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    /// <summary>
    /// A specific {@link Adapter} implementation for RESTful servers.
    ///
    /// In addition to implementing the <see cref="Adapter"/> interface,
    /// <code>RestAdapter</code> contains a single <see cref="RestContract"/> to map
    /// remote methods to custom HTTP routes. This is only required if the HTTP
    /// settings have been customized on the server. When in doubt, try without.
    ///
    /// <see cref="RestContract"/>
    /// </summary>
    public class RestAdapter : Adapter
    {
        private static string TAG = "remoting.RestAdapter";
        private HttpClient _httpClient;

        /// <summary>
        /// This adapter's <see cref="RestContract">adapter</see>, a custom contract for fine-grained route configuration.
        /// </summary>
        public RestContract Contract { get; set; }

        /// <summary>
        /// The underlying HTTP client. This allows subclasses to add  custom headers like Authorization.
        /// </summary>
        protected HttpClient Client
        {
            get { return _httpClient; }
        }


        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        public RestAdapter(IContext context, string url)
            : base(context, url)
        {
            Contract = new RestContract();
        }

        public override void Connect(IContext context, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                _httpClient = null;
            }
            else
            {
                _httpClient = new HttpClient(new NativeMessageHandler());
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            }
        }

        public override bool IsConnected()
        {
            return Client != null;
        }

        public override void InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError)
        {
            InvokeStaticMethodImpl(method, parameters, async response => { await Callback(onSuccess, onError, response); });
        }


        public override void InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<byte[], string> onSuccess,
            Action<Exception> onError)
        {
            InvokeStaticMethodImpl(method, parameters, async response =>
            {
                await BinaryCallback(onSuccess, onError, response);
            });
        }

        public override void InvokeInstanceMethod(string method,
            Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError)
        {
            InvokeInstanceMethodImpl(method, constructorParameters, parameters, async response =>
            {
                await Callback(onSuccess, onError, response);
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="constructorParameters"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public override void InvokeInstanceMethod(string method,
            Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters,
            Action<byte[], string> onSuccess,
            Action<Exception> onError)
        {
            InvokeInstanceMethodImpl(method, constructorParameters, parameters,
                async response => { await BinaryCallback(onSuccess, onError, response); });
        }

        private static async Task Callback(Action<string> onSuccess, Action<Exception> onError,
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

        private static async Task BinaryCallback(Action<byte[], string> onSuccess, Action<Exception> onError,
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

        private void InvokeStaticMethodImpl(string method,
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

            Request(path, verb, parameters, parameterEncoding, httpHandler);
        }

        private void InvokeInstanceMethodImpl(string method,
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

            Request(path, verb, combinedParameters, parameterEncoding, httpHandler);
        }

        private async void Request(string path,
            string verb,
            Dictionary<string, object> parameters,
            ParameterEncoding parameterEncoding,
            Action<HttpResponseMessage> responseHandler)
        {
            if (!IsConnected())
            {
                throw new Exception("Adapter not connected");
            }

            //TODO:
            //using (var client = new HttpClient(new NativeMessageHandler()))
            //{
            _httpClient.BaseAddress = new Uri(Url);

            var method = (HttpMethod)Enum.Parse(typeof(HttpMethod), verb, true);
            var request = new HttpRequestMessage(method, path);
            HttpContent content;
            switch (parameterEncoding)
            {
                case ParameterEncoding.FORM_URL:
                    var listOfParams =
                        parameters.Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value.ToString()))
                            .AsEnumerable();
                    content = new FormUrlEncodedContent(listOfParams);
                    break;
                case ParameterEncoding.JSON:
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    content = new StringContent(JsonConvert.SerializeObject(parameters));
                    break;
                case ParameterEncoding.FORM_MULTIPART:
                    //TODO:
                    content = new MultipartFormDataContent("");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("parameterEncoding");
            }

            request.Content = content;
            var message = await Client.SendAsync(request);
            responseHandler(message);
            //}
        }
    }

    //TODO: AFNetworking . Paul Betts, says "modernhttpclient" solves general needs.
}