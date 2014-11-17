using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoopBack.Sdk.Xamarin.Remoting.Adapters
{
    /// <summary>
    ///     The entry point to all networking accomplished with LoopBack. Adapters
    ///     encapsulate information consistent to all networked operations, such as base
    ///     URL, port, etc.
    /// </summary>
    public abstract class AdapterBase
    {
        public enum ParameterEncoding
        {
            FORM_URL,
            JSON,
            FORM_MULTIPART
        }

        /// <summary>
        ///     Creates a new, disconnected Adapter.
        /// </summary>
        protected AdapterBase()
            : this(null)
        {
        }

        /// <summary>
        ///     Creates a new Adapter, connecting it to `url`.
        /// </summary>
        /// <param name="url">The URL to connect to.</param>
        protected AdapterBase(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Url = url;
                Connect(url);
            }
        }

        public string Url { get; set; }

        /// <summary>
        ///     Connects the Adapter to `url`.
        /// </summary>
        /// <param name="url">The URL to connect to.</param>
        public abstract void Connect(string url);

        /// <summary>
        ///     Gets whether this adapter is connected to a server.
        /// </summary>
        /// <returns> <code>true</code> if connected, <code>false</code> otherwise.</returns>
        public abstract bool IsConnected();

        /// <summary>
        ///     Invokes a remotable method exposed statically on the server.
        /// </summary>
        /// <param name="method">The method to invoke, e.g. <code>"module.doSomething"</code>.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public abstract Task InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError);

        /// <summary>
        ///     Invokes a remotable method exposed statically on the server, parses the response as binary data.
        /// </summary>
        /// <param name="method">The method to invoke, e.g. <code>"module.doSomething"</code>.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public virtual Task InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<byte[], string> onSuccess,
            Action<Exception> onError)
        {
            throw new NotSupportedException(GetType().Name + " does not support binary responses.");
        }

        /// <summary>
        ///     Invokes a remotable method exposed within a prototype on the server.
        ///     <p>
        ///         This should be thought of as a two-step process. First, the server loads
        ///         or creates an object with the appropriate type. Then and only then is
        ///         the method invoked on that object. The two parameter dictionaries
        ///         correspond to these two steps: `creationParameters` for the former, and
        ///         `parameters` for the latter.
        ///     </p>
        /// </summary>
        /// <param name="method">method The method to invoke, e.g. <code>"MyClass.prototype.doSomething"</code>.</param>
        /// <param name="constructorParameters">The parameters the virtual object should be created with.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public abstract Task InvokeInstanceMethod(string method,
            Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError);

        /// <summary>
        ///     Invokes a remotable method exposed within a prototype on the server, parses the response as binary data.
        ///     <p>
        ///         This should be thought of as a two-step process. First, the server loads
        ///         or creates an object with the appropriate type. Then and only then is
        ///         the method invoked on that object. The two parameter dictionaries
        ///         correspond to these two steps: `creationParameters` for the former, and
        ///         `parameters` for the latter.
        ///     </p>
        /// </summary>
        /// <param name="method">method The method to invoke, e.g. <code>"MyClass.prototype.doSomething"</code>.</param>
        /// <param name="constructorParameters">The parameters the virtual object should be created with.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public virtual Task InvokeInstanceMethod(string method,
            Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters,
            Action<byte[], string> onSuccess,
            Action<Exception> onError)
        {
            throw new NotSupportedException(GetType().Name + " does not support binary responses.");
        }
    }
}