using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoopBack.Sdk.Xamarin.Extensions;
using LoopBack.Sdk.Xamarin.Remoting.Adapters;

namespace LoopBack.Sdk.Xamarin.Remoting
{
    /// <summary>
    ///     A local representative of remote model repository, it provides access to static methods like
    ///     <pre>User.findById()</pre>
    /// </summary>
    public class RemoteRepository<T> : IRemoteRepository where T : RemoteClass
    {
        /// <summary>
        ///     Creates a new Repository, associating it with the named remote class.
        /// </summary>
        /// <param name="remoteClassName">The remote class name</param>
        public RemoteRepository(string remoteClassName)
        {
            if (string.IsNullOrEmpty(remoteClassName))
            {
                throw new ArgumentException("Class name cannot be null or empty.");
            }
            RemoteClassName = remoteClassName;
        }

        /// <summary>
        ///     The Adapter that should be used for invoking methods, both
        ///     for static methods on this prototype and all methods on all instances of
        ///     this prototype.
        /// </summary>
        public AdapterBase Adapter { get; set; }

        /// <summary>
        ///     The name given to this prototype on the server.
        /// </summary>
        public string RemoteClassName { get; private set; }

        /// <summary>
        ///     Creates a new <see cref="RemoteClass" /> as a virtual instance of this remote class.
        /// </summary>
        /// <param name="creationParameters">The creation parameters of the new object</param>
        /// <returns>A new <see cref="RemoteClass" /> based on this prototype.</returns>
        public virtual T CreateObject(Dictionary<string, object> creationParameters)
        {
            T objectToCreate;
            try
            {
                objectToCreate = Activator.CreateInstance<T>();
            }
            catch (Exception e)
            {
                var ex = new ArgumentException("", e);

                throw ex;
            }
            objectToCreate.Repository = this;

            if (creationParameters != null)
            {
                objectToCreate.CreationParameters = creationParameters;
                creationParameters.ToObject(objectToCreate);
            }

            return objectToCreate;
        }

        /// <summary>
        ///     Invokes a remotable method exposed statically within this class on the server.
        /// </summary>
        /// <param name="method">The method to invoke (without the class name), e.g. <code>"doSomething"</code>.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public async Task InvokeStaticMethod(string method, Dictionary<string, object> parameters, Action<string> onSuccess,
            Action<Exception> onError)
        {
            if (Adapter == null)
            {
                throw new ArgumentException("No adapter set");
            }
            var path = RemoteClassName + "." + method;
            await Adapter.InvokeStaticMethod(path, parameters, onSuccess, onError);
        }

        /// <summary>
        ///     Invokes a remotable method exposed statically within this class on the server, parses the response as binary data.,
        /// </summary>
        /// <param name="method">The method to invoke (without the class name), e.g. <code>"doSomething"</code>.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public async Task InvokeStaticMethod(string method,
            Dictionary<string, object> parameters, Action<byte[], string> onSuccess, Action<Exception> onError)
        {
            if (Adapter == null)
            {
                throw new ArgumentException("No adapter set");
            }
            var path = RemoteClassName + "." + method;
            await Adapter.InvokeStaticMethod(path, parameters, onSuccess, onError);
        }
    }
}