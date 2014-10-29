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
    public class RestAdapter : Adapter
    {
        private static string TAG = "remoting.RestAdapter";
        protected HttpClient Client;

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        public RestAdapter(IContext context, string url)
            : base(context, url)
        {
            Contract = new RestContract();
        }

        public RestContract Contract { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        public override void Connect(IContext context, string url)
        {
            //TODO: Refactor!
            if (string.IsNullOrEmpty(url))
            {
                Client = null;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override bool IsConnected()
        {
            return Client != null;
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="onSucces"></param>
        /// <param name="onError"></param>
        public override void InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<string> onSucces,
            Action<Exception> onError)
        {
            InvokeStaticMethodImpl(method, parameters,
                async response => { await Callback(onSucces, onError, response); });
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public override void InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<byte[], string> onSuccess,
            Action<Exception> onError)
        {
            InvokeStaticMethodImpl(method, parameters,
                async response => { await BinaryCallback(onSuccess, onError, response); });
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
            Action<string> onSuccess,
            Action<Exception> onError)
        {
            InvokeInstanceMethodImpl(method, constructorParameters, parameters,
                async response => { await Callback(onSuccess, onError, response); });
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
                throw new Exception("Invalid contract");
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

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.BaseAddress = new Uri(Url);

                var method = (HttpMethod) Enum.Parse(typeof (HttpMethod), verb, true);
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
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
            }
        }
    }
}