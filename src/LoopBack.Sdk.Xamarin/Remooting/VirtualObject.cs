using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;
using Newtonsoft.Json;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    public class VirtualObject
    {
        public Dictionary<String, object> CreationParameters { get; set; }

        /// <summary>
        /// </summary>
        public VirtualObject()
            : this(null, null)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="creationParameters"></param>
        public VirtualObject(IRepository repository, Dictionary<String, object> creationParameters)
        {
            Repository = repository;
            CreationParameters = creationParameters;
        }

        [JsonIgnore]
        public IRepository Repository { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, object> ToMap()
        {
            return this.ToDictionary();
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public void InvokeMethod(String method, Dictionary<String, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError)
        {
            var adapter = Repository.Adapter;
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter", "Repository adapter cannot be null");
            }
            var path = Repository.GetClassName() + ".prototype." + method;
            adapter.InvokeInstanceMethod(path, CreationParameters, parameters, onSuccess, onError);
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public void InvokeMethod(String method, Dictionary<String, object> parameters,
            Action<byte[], String> onSuccess,
            Action<Exception> onError)
        {
            var adapter = Repository.Adapter;
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter", "Repository adapter cannot be null");
            }
            var path = Repository.GetClassName() + ".prototype." + method;

            adapter.InvokeInstanceMethod(path, CreationParameters, parameters, onSuccess, onError);
        }
    }
}