using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;
using Newtonsoft.Json;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    public class VirtualObject
    {
        [JsonIgnore]
        protected IRepository Repository { get; set; }

        private readonly Dictionary<String, object> _creationParameters;

        /// <summary>
        /// 
        /// </summary>
        public VirtualObject()
            : this(null, null)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="creationParameters"></param>
        public VirtualObject(IRepository repository, Dictionary<String, object> creationParameters)
        {
            Repository = repository;
            _creationParameters = creationParameters;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, object> ToMap()
        {
            return this.ToDictionary();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public void InvokeMethod(String method, Dictionary<String, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError)
        {
            Adapter adapter = Repository.Adapter;
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter", "Repository adapter cannot be null");
            }
            String path = Repository.GetClassName() + ".prototype." + method;
            adapter.InvokeInstanceMethod(path, _creationParameters, parameters, onSuccess, onError);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public void InvokeMethod(String method, Dictionary<String, object> parameters,
            Action<byte[], String> onSuccess,
            Action<Exception> onError)
        {
            Adapter adapter = Repository.Adapter;
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter", "Repository adapter cannot be null");
            }
            String path = Repository.GetClassName() + ".prototype." + method;

            adapter.InvokeInstanceMethod(path, _creationParameters, parameters, onSuccess, onError);
        }
    }
}