using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;
using Newtonsoft.Json;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    /// <summary>
    ///  A local representative of a single virtual object. The behavior of this
    /// object is defined through a model defined on the server, and the identity
    /// of this instance is defined through its `creationParameters`.
    /// </summary>
    public class VirtualObject
    {
        /// <summary>
        /// The creation parameters this object was created from.
        /// </summary>
        public Dictionary<String, object> CreationParameters { get; set; }

        /// <summary>
        /// The Repository this object was created from
        /// </summary>
        [JsonIgnore]
        public IRepository Repository { get; set; }

        /// <summary>
        /// Creates a new object not attached to any repository.
        /// </summary>
        public VirtualObject()
            : this(null, null)
        {
        }

        /// <summary>
        /// Creates a new object from the given repository and parameters.
        /// </summary>
        /// <param name="repository">The repository this object should inherit from</param>
        /// <param name="creationParameters">The creationParameters of the new object</param>
        public VirtualObject(IRepository repository, Dictionary<String, object> creationParameters)
        {
            Repository = repository;
            CreationParameters = creationParameters;
        }

        /// <summary>
        /// Converts the object into a <see cref="Dictionary{TKey,TValue}"></see>
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, object> ToDictionary()
        {
            return DictionaryExtensions.ToDictionary(this);
        }

        /// <summary>
        /// Invokes a remotable method exposed within instances of this class on the server.
        /// </summary>
        /// <param name="method">The method to invoke (without the repository), e.g. <code>doSomething</code></param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void InvokeMethod(String method,
            Dictionary<String, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError)
        {
            var adapter = Repository.Adapter;
            if (adapter == null)
            {
                throw new ArgumentException("adapter", "Repository adapter cannot be null");
            }
            var path = Repository.ClassName + ".prototype." + method;
            adapter.InvokeInstanceMethod(path, CreationParameters, parameters, onSuccess, onError);
        }

        /// <summary>
        /// Invokes a remotable method exposed within instances of this class on the server, parses the response as binary data.
        /// </summary>
        /// <param name="method">The method to invoke (without the repository), e.g. <code>doSomething</code></param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void InvokeMethod(String method, Dictionary<String, object> parameters,
            Action<byte[], String> onSuccess,
            Action<Exception> onError)
        {
            var adapter = Repository.Adapter;
            if (adapter == null)
            {
                throw new ArgumentException("adapter", "Repository adapter cannot be null");
            }
            var path = Repository.ClassName + ".prototype." + method;

            adapter.InvokeInstanceMethod(path, CreationParameters, parameters, onSuccess, onError);
        }
    }
}