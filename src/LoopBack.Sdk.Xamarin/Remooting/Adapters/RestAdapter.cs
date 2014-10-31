using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Humanizer;
using LoopBack.Sdk.Xamarin.Common;
using ModernHttpClient;
using Newtonsoft.Json;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    /// <summary>
    /// A specific <see cref="Adapter"/> implementation for RESTful servers.
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
            InvokeStaticMethodImpl(method, parameters, response => { Callback(onSuccess, onError, response); });
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
            InvokeInstanceMethodImpl(method, constructorParameters, parameters, response =>
            {
                Callback(onSuccess, onError, response);
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

        private static void Callback(Action<string> onSuccess, Action<Exception> onError,
            HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    Task<string> task = response.Content.ReadAsStringAsync();
                    onSuccess(task.Result);
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

        private void Request(string path,
            string verb,
            Dictionary<string, object> parameters,
            ParameterEncoding parameterEncoding,
            Action<HttpResponseMessage> responseHandler)
        {
            bool skipBody = false;
            if (!IsConnected())
            {
                throw new Exception("Adapter not connected");
            }


            //TODO:
            //http://stackoverflow.com/questions/3981564/cannot-send-a-content-body-with-this-verb-type
            //http://stackoverflow.com/questions/2064281/sending-post-data-with-get-request-valid

            if (parameters != null)
            {
                if ("GET".Equals(verb) || "HEAD".Equals(verb) || "DELETE".Equals(verb))
                {
                    Dictionary<string, object> flattenParameters = FlattenParameters(parameters);

                    List<string> keyValues = new List<string>(flattenParameters.Count);
                    keyValues.AddRange(flattenParameters.Select(param => string.Format("{0}={1}", param.Key, param.Value)));
                    path += "?" + string.Join("&", keyValues);
                    skipBody = true;
                }
            }

            //TODO: AFNetworking . Paul Betts, says "modernhttpclient" solves general needs.
            //TODO: Dispose HttpClient
            _httpClient.BaseAddress = new Uri(Url);

            HttpMethod method = new HttpMethod(verb);
            HttpRequestMessage request = new HttpRequestMessage(method, path);
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
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!skipBody)
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

            Task<HttpResponseMessage> message = Client.SendAsync(request);
            responseHandler(message.Result);
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

            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> entry in parameters)
            {

                String key = keyPrefix != null
                        ? keyPrefix + "[" + entry.Key + "]"
                        : entry.Key;

                if (entry.Value is Dictionary<string, object>)
                {
                    result.AddRange(FlattenParameters(key, (Dictionary<string, object>)entry.Value));
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