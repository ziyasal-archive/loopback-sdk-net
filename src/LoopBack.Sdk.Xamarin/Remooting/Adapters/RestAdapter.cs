using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;
using RestSharp.Portable;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    public class RestAdapter : Adapter
    {
        private static string TAG = "remoting.RestAdapter";
		private IRestClient _httpClient;

        public RestAdapter(IContext context, string url)
            : base(context, url)
        {
            Contract = new RestContract();
        }

        public RestContract Contract { get; set; }

        public override void Connect(IContext context, string url)
        {
			if (url == null) {
				_httpClient = null;
			}
			else {
				_httpClient = new RestClient (url);
			}
        }

        public override bool IsConnected()
        {
			return _httpClient != null;
        }

        public override void InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<string> onSucces,
            Action<Exception> onError)
        {
            throw new NotImplementedException();
        }

        public override void InvokeInstanceMethod(string path,
            Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError)
        {
            throw new NotImplementedException();
        }

		private void invokeStaticMethod(String method, Dictionary<string,object> parameters, Action handler) {
			if (contract == null) {
				throw new IllegalStateException("Invalid contract");
			}

			String verb = contract.getVerbForMethod(method);
			String path = contract.getUrlForMethod(method, parameters);
			ParameterEncoding parameterEncoding = contract.getParameterEncodingForMethod(method);

			request(path, verb, parameters, parameterEncoding, httpHandler);
		}
    }
}