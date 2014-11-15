using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Extensions;

namespace LoopBack.Sdk.Xamarin.Remoting.Adapters
{
    /// <summary>
    ///     A contract specifies how remote method names map to HTTP routes.
    ///     For example, if a remote method on the server has been remapped like so:
    ///     <pre>
    ///         <code>
    ///  project.getObject = function (id, callback) {
    ///      callback(null, { ... });
    ///  };
    ///  helper.method(project.getObject, {
    ///      http: { verb: 'GET', path: '/:id'},
    ///      accepts: { name: 'id', type: 'string' }
    ///      returns: { name: 'object', type: 'object' }
    ///  })
    /// }
    ///  </code>
    ///     </pre>
    ///     The new route is GET /:id, instead of POST /project/getObject, so we need to update our contract on the client:
    ///     <pre>
    ///         <code>
    ///  contract.AddItem(new RestContractItem("/:id", "GET"), "project.getObject")); 
    ///  </code>
    ///     </pre>
    /// </summary>
    public class RestContract
    {
        public RestContract()
        {
            Items = new Dictionary<string, RestContractItem>();
        }

        protected Dictionary<string, RestContractItem> Items { get; set; }

        /// <summary>
        ///     Adds a single item to this contract. The item can be shared among
        ///     Similarly, each item can be used for more than one method, like so:
        ///     <pre>
        ///         <code>
        ///  RestContractItem upsert = new RestContractItem("/widgets/:id", "PUT");
        ///  contract.AddItem(upsert, "widgets.create");
        ///  contract.AddItem(upsert, "widgets.update");
        ///  </code>
        ///     </pre>
        /// </summary>
        /// <param name="item">The item to add to this contract.</param>
        /// <param name="method">The method the item should represent.</param>
        public void AddItem(RestContractItem item, String method)
        {
            if (item == null || string.IsNullOrEmpty(method))
            {
                throw new ArgumentNullException("Neither item nor method can be null");
            }

            Items.Add(method, item);
        }

        /// <summary>
        ///     Adds all items from contract.
        /// </summary>
        /// <param name="contract">The <see cref="RestContract">contract</see> to copy from.</param>
        public void AddItemsFromContract(RestContract contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException("Contract cannot be null");
            }

            Items.AddRange(contract.Items);
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

            if (Items.ContainsKey(method))
            {
                var item = Items[method];

                return item != null ? item.Pattern : null;
            }

            return null;
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

            if (Items.ContainsKey(method))
            {
                var item = Items[method];
                return item != null ? item.Verb : defaultVerb;
            }

            return defaultVerb;
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

            if (Items.ContainsKey(method))
            {
                var item = Items[method];

                return item != null ? item.ParameterEncoding : AdapterBase.ParameterEncoding.JSON;
            }

            return AdapterBase.ParameterEncoding.JSON;
        }

        /// <summary>
        ///     Resolves a specific method, replacing pattern fragments with the optional parameters as appropriate.
        /// </summary>
        /// <param name="method">The method to resolve.</param>
        /// <param name="parameters">Pattern parameters. Can be <code>null</code>.</param>
        /// <returns>The complete, resolved URL.</returns>
        public String GetUrlForMethod(string method, Dictionary<string, object> parameters = null)
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
                var value = entry.Value.ToString();
                url = url.Replace(key, value);
            }

            return url;
        }
    }
}