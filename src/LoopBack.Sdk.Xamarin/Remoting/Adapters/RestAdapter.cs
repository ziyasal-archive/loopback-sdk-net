using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Loopback.Sdk.Xamarin.Extensions;
using Loopback.Sdk.Xamarin.Shared;
using ModernHttpClient;
using Newtonsoft.Json;

namespace Loopback.Sdk.Xamarin.Remoting.Adapters
{
    /// <summary>
    ///     A specific <see cref="AdapterBase" /> implementation for RESTful servers.
    /// </summary>
    public class RestAdapter : AdapterBase, IRemotingRestAdapter
    {
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        public RestAdapter(IContext context, string url)
            : base(url)
        {
            ApplicationContext = context;
            Contract = new RestContract();

            AttchContextToClient();
        }

        public RestAdapter(string url)
            : base(url)
        {
            ApplicationContext = new RestContext("loopback-xamarin/1.0");
            Contract = new RestContract();
            AttchContextToClient();
        }

        public virtual IContext ApplicationContext { get; protected set; }

        /// <summary>
        ///     The underlying HTTP client. This allows subclasses to add  custom headers like Authorization.
        /// </summary>
        public HttpClient Client { get; private set; }

        /// <summary>
        ///     This adapter's <see cref="RestContract">adapter</see>, a custom contract for fine-grained route configuration.
        /// </summary>
        public RestContract Contract { get; set; }

        public override void Connect(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Client = null;
            }
            else
            {
                //TODO: AFNetworking? According to Paul Betts "modernhttpclient" solves general needs.
                Client = new HttpClient(new NativeMessageHandler());
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
            }
        }

        public override bool IsConnected()
        {
            return Client != null;
        }

        public override async Task<RemotingResponse> InvokeInstanceMethod(string method,
            Dictionary<string, object> constructorParameters, Dictionary<string, object> parameters)
        {
            return await InvokeInstanceMethodImpl(method, constructorParameters, parameters);
        }

        public override async Task<RemotingResponse> InvokeStaticMethod(string method,
            Dictionary<string, object> parameters)
        {
            return await InvokeStaticMethodImpl(method, parameters);
        }

        private void AttchContextToClient()
        {
            if (Client != null)
            {
                Client.DefaultRequestHeaders.Add("User-Agent", ApplicationContext.UserAgent);
            }
        }

        private async Task<RemotingResponse> InvokeInstanceMethodImpl(string method,
            Dictionary<string, object> constructorParameters, Dictionary<string, object> parameters)
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

            return await HandleResponse(await Request(path, verb, combinedParameters, parameterEncoding));
        }

        private async Task<RemotingResponse> InvokeStaticMethodImpl(string method, Dictionary<string, object> parameters)
        {
            if (Contract == null)
            {
                throw new InvalidOperationException("Invalid contract");
            }

            var verb = Contract.GetVerbForMethod(method);
            var path = Contract.GetUrlForMethod(method, parameters);
            var parameterEncoding = Contract.GetParameterEncodingForMethod(method);

            var result = new RemotingResponse();
            try
            {
                var response = await Request(path, verb, parameters, parameterEncoding);
                if (response.IsSuccessStatusCode)
                {
                    result.Raw = response;
                    result.Content = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception exception)
            {
                result.Exception = exception;
            }

            return await HandleResponse(await Request(path, verb, parameters, parameterEncoding));
        }

        private async Task<RemotingResponse> HandleResponse(HttpResponseMessage response)
        {
            var result = new RemotingResponse();
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    result.Raw = response;
                    result.Content = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception exception)
            {
                result.Exception = exception;
            }

            return result;
        }

        private async Task<HttpResponseMessage> Request(string path, string verb, Dictionary<string, object> parameters,
            ParameterEncoding parameterEncoding)
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

                    if (keyValues.Count > 0)
                    {
                        path += "?" + string.Join("&", keyValues);
                    }

                    skipBody = true;
                }
            }

            var method = new HttpMethod(verb);

            if (!path.StartsWith("/") && !Url.EndsWith("/"))
            {
                path = string.Format("/{0}", path);
            }

            var request = new HttpRequestMessage(method, new Uri(Url + path));
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
                        var serializeObject = JsonConvert.SerializeObject(parameters);
                        content = new StringContent(serializeObject);
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

            return await Client.SendAsync(request);
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