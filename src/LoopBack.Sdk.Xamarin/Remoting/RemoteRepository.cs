using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loopback.Sdk.Xamarin.Extensions;
using Loopback.Sdk.Xamarin.Remoting.Adapters;

namespace Loopback.Sdk.Xamarin.Remoting
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
        public IRemotingRestAdapter Adapter { get; set; }

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
            objectToCreate.RemoteRepository = this;

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
        public async Task<RemotingResponse> InvokeStaticMethod(string method, Dictionary<string, object> parameters)
        {
            if (Adapter == null)
            {
                throw new ArgumentException("No adapter set");
            }
            var path = RemoteClassName + "." + method;

            return await Adapter.InvokeStaticMethod(path, parameters);
        }
    }
}