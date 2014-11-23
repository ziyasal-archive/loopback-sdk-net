using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loopback.Sdk.Xamarin.Extensions;
using Newtonsoft.Json;

namespace Loopback.Sdk.Xamarin.Remoting
{
    /// <summary>
    ///     A local representative of a single virtual object. The behavior of this
    ///     object is defined through a model defined on the server, and the identity
    ///     of this instance is defined through its `creationParameters`.
    /// </summary>
    public class RemoteClass
    {
        /// <summary>
        ///     Creates a new object not attached to any repository.
        /// </summary>
        public RemoteClass()
            : this(null, null)
        {
        }

        /// <summary>
        ///     Creates a new object from the given repository and parameters.
        /// </summary>
        /// <param name="repository">The repository this object should inherit from</param>
        /// <param name="creationParameters">The creationParameters of the new object</param>
        public RemoteClass(IRemoteRepository repository, Dictionary<string, object> creationParameters)
        {
            RemoteRepository = repository;
            CreationParameters = creationParameters;
        }

        /// <summary>
        ///     The creation parameters this object was created from.
        /// </summary>
        [JsonIgnore] //TODO: check
        public Dictionary<string, object> CreationParameters { get; set; }

        /// <summary>
        ///     The Repository this object was created from
        /// </summary>
        [JsonIgnore]
        public IRemoteRepository RemoteRepository { get; set; }

        /// <summary>
        ///     Invokes a remotable method exposed within instances of this class on the server.
        /// </summary>
        /// <param name="method">The method to invoke (without the repository), e.g. <code>doSomething</code></param>
        /// <param name="parameters">The parameters to invoke with.</param>
        public async Task<RemotingResponse> InvokeMethod(string method, Dictionary<string, object> parameters)
        {
            var adapter = RemoteRepository.Adapter;
            if (adapter == null)
            {
                throw new ArgumentException("Repository adapter cannot be null");
            }
            var path = RemoteRepository.RemoteClassName + ".prototype." + method;

            return await adapter.InvokeInstanceMethod(path, CreationParameters, parameters);
        }

        /// <summary>
        ///     Converts the object into a <see cref="Dictionary{TKey,TValue}"></see>
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, object> ToDictionary()
        {
            return DictionaryExtensions.ToDictionary(this);
        }
    }
}