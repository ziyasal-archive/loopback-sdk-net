using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loopback.Sdk.Xamarin.Extensions;
using Loopback.Sdk.Xamarin.Remoting.Adapters;
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
        private string _remoteType;
        private readonly Dictionary<string, RemotingOptions> _remotes;

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
            RemoteMethodDefinitions = new Dictionary<string, RemoteDefinition>();
            _remotes = new Dictionary<string, RemotingOptions>();
        }

        [JsonIgnore]
        protected Dictionary<string, RemoteDefinition> RemoteMethodDefinitions { get; set; }

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

            var methodTmp = RemoteRepository.RemoteClassName + ".prototype." + method;

            var combinedParameters = new Dictionary<string, object>();

            if (CreationParameters != null)
            {
                combinedParameters.AddRange(CreationParameters);
            }

            if (parameters != null)
            {
                combinedParameters.AddRange(parameters);
            }

            var verb = GetVerbForMethod(methodTmp);
            var path = GetUrlForMethod(methodTmp, combinedParameters);
            var parameterEncoding = GetParameterEncodingForMethod(methodTmp);

            return await adapter.InvokeInstanceMethod(combinedParameters, path, verb, parameterEncoding);
        }

        /// <summary>
        ///     Converts the object into a <see cref="Dictionary{TKey,TValue}"></see>
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, object> ToDictionary()
        {
            return DictionaryExtensions.ToDictionary(this);
        }

        /// <summary>
        ///     Gets the HTTP verb for the given method string.
        /// </summary>
        /// <param name="method">The method to resolve.</param>
        /// <returns>The resolved verb, or "POST" if it isn't defined.</returns>
        public string GetVerbForMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }
            const string defaultVerb = "POST";

            if (RemoteMethodDefinitions.ContainsKey(method))
            {
                var item = RemoteMethodDefinitions[method];
                return item != null ? item.Verb : defaultVerb;
            }

            return defaultVerb;
        }

        /// <summary>
        ///     Resolves a specific method, replacing pattern fragments with the optional parameters as appropriate.
        /// </summary>
        /// <param name="method">The method to resolve.</param>
        /// <param name="parameters">Pattern parameters. Can be <code>null</code>.</param>
        /// <returns>The complete, resolved URL.</returns>
        public string GetUrlForMethod(string method, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(method))
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

        /// <summary>
        ///     Gets the <see cref="AdapterBase.ParameterEncoding" /> for the given method.
        /// </summary>
        /// <param name="method">The method to resolve.</param>
        /// <returns>The parameter encoding.</returns>
        public AdapterBase.ParameterEncoding GetParameterEncodingForMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            if (RemoteMethodDefinitions.ContainsKey(method))
            {
                var item = RemoteMethodDefinitions[method];

                return item != null ? item.ParameterEncoding : AdapterBase.ParameterEncoding.JSON;
            }

            return AdapterBase.ParameterEncoding.JSON;
        }

        /// <summary>
        ///     Returns the custom pattern representing the given method string, or
        ///     <code>null</code> if no custom pattern exists.
        /// </summary>
        /// <param name="method">The method to resolve.</param>
        /// <returns>The custom pattern if one exists, <code>null</code> otherwise.</returns>
        public string GetPatternForMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            if (RemoteMethodDefinitions.ContainsKey(method))
            {
                var item = RemoteMethodDefinitions[method];

                return item != null ? item.Pattern : null;
            }

            return null;
        }

        /// <summary>
        ///     Generates a fallback URL for a method whose contract has not been customized.
        /// </summary>
        /// <param name="method">The method to generate from.</param>
        /// <returns>The resolved URL.</returns>
        public string GetUrlForMethodWithoutItem(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new ArgumentNullException("method", "Method cannot be null");
            }

            return method.Replace('.', '/');
        }

        /// <summary>
        ///     Returns a rendered URL pattern using the parameters provided. For
        ///     example, the pattern <code>"/widgets/:id"</code> with the parameters
        ///     that contain the value <code>"57"</code> for key <code>"id"</code>,
        ///     begets <code>"/widgets/57"</code>.
        /// </summary>
        /// <param name="pattern">The pattern to render.</param>
        /// <param name="parameters">The values to render with.</param>
        /// <returns>The rendered URL.</returns>
        public string GetUrl(string pattern, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentNullException("pattern", "Pattern cannot be null");
            }

            var url = pattern;
            if (parameters == null)
            {
                return url;
            }

            foreach (var entry in parameters)
            {
                var key = ":" + entry.Key;
                var value = entry.Value;

                if (value != null)
                {
                    url = url.Replace(key, value.ToString());
                }
            }

            return url;
        }

        public void AddItemsFromModel(RemoteClass contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException("contract", "Contract cannot be null");
            }

            RemoteMethodDefinitions.AddRange(contract.RemoteMethodDefinitions);
        }

        public void SetRemoting(string remoteType, string method, RemotingOptions remotingOptions)
        {
            _remoteType = remoteType;
            var key = remoteType + "." + method;

            if (!_remotes.ContainsKey(key))
            {
                _remotes.Add(key, remotingOptions);
            }
        }

        public void SetRemoting(string pattern, string verb, string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new ArgumentException("Method can't be null");
            }

            var definition = new RemoteDefinition(pattern, verb);
            RemoteMethodDefinitions.Add(method, definition);
        }
    }
}