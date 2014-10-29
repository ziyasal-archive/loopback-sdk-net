using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    public class RestContract
    {
        public RestContract()
        {
            Items = new Dictionary<string, RestContractItem>();
        }

        protected Dictionary<string, RestContractItem> Items { get; set; }

        public void AddItem(RestContractItem item, String method)
        {
            if (item == null || method == null)
            {
                throw new ArgumentNullException("Neither item nor method can be null");
            }

            Items.Add(method, item);
        }

        public void AddItemsFromContract(RestContract contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException("Contract cannot be null");
            }

            Items.AddRange(contract.Items);
        }

        public string GetPatternForMethod(String method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            var item = Items[method];

            return item != null ? item.Pattern : null;
        }

        public string GetVerbForMethod(String method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            var item = Items[method];

            return item != null ? item.Verb : "POST";
        }

        public Adapter.ParameterEncoding GetParameterEncodingForMethod(String method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            var item = Items[method];

            return item != null
                ? item.ParameterEncoding
                : Adapter.ParameterEncoding.JSON;
        }

        public String GetUrlForMethod(string method, Dictionary<string, object> parameters)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            var pattern = GetPatternForMethod(method);

            if (pattern != null)
            {
                return GetUrl(pattern, parameters);
            }

            return GetUrlForMethodWithoutItem(method);
        }

        public string GetUrlForMethodWithoutItem(string method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            return method.Replace('.', '/');
        }

        public string GetUrl(string pattern, Dictionary<string, object> parameters)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            var url = pattern;
            if (parameters == null)
            {
                return url;
            }

            foreach (var entry in parameters)
            {
                var key = ":" + entry.Key;
                var value = entry.Value.ToString();
                url = url.Replace(key, value);
            }

            return url;
        }
    }
}