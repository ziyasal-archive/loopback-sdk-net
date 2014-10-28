using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;
using RestSharp.Portable;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    public class RestAdapter : Adapter
    {
        private static string TAG = "remoting.RestAdapter";
        private IRestClient _client;

        public RestAdapter(IContext context, string url)
            : base(context, url)
        {
            Contract = new RestContract();
            _client = new RestClient(url);
        }

        public RestContract Contract { get; set; }

        public override void Connect(IContext context, string url)
        {
            throw new NotImplementedException();
        }

        public override bool IsConnected()
        {
            throw new NotImplementedException();
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
    }
}