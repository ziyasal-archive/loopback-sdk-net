using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    /// <summary>
    ///  A local representative of remote model repository, it provides access to static methods like <pre>User.findById()</pre>
    /// </summary>
    public class RemoteRepository<T> : IRemoteRepository where T : RemoteClass
    {
        private string _className;

        /// <summary>
        /// The Adapter that should be used for invoking methods, both
        /// for static methods on this prototype and all methods on all instances of
        /// this prototype.
        /// </summary>
        public Adapter Adapter { get; set; }

        /// <summary>
        /// The name given to this prototype on the server.
        /// </summary>
        public string ClassName
        {
            get { return _className; }
            private set { _className = value; }
        }

        /// <summary>
        /// Creates a new Repository, associating it with the named remote class.
        /// </summary>
        /// <param name="className">The remote class name</param>
        public RemoteRepository(string className)
        {
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentException("Class name cannot be null or empty.");
            }
            ClassName = className;
        }


        /// <summary>
        /// Creates a new <see cref="RemoteClass"/> as a virtual instance of this remote class.
        /// </summary>
        /// <param name="creationParameters">The creation parameters of the new object</param>
        /// <returns>A new <see cref="RemoteClass"/> based on this prototype.</returns>
        public virtual T CreateObject(Dictionary<string, object> creationParameters)
        {
            T objectToCreate;
            try
            {
                objectToCreate = Activator.CreateInstance<T>();
            }
            catch (Exception e)
            {
                ArgumentException ex = new ArgumentException("", e);

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
        /// Invokes a remotable method exposed statically within this class on the server.
        /// </summary>
        /// <param name="method">The method to invoke (without the class name), e.g. <code>"doSomething"</code>.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void InvokeStaticMethod(string method, Dictionary<string, object> parameters, Action<string> onSuccess, Action<Exception> onError)
        {
            if (Adapter == null)
            {
                throw new ArgumentException("No adapter set");
            }
            string path = _className + "." + method;
            Adapter.InvokeStaticMethod(path, parameters, onSuccess, onError);
        }

        /// <summary>
        /// Invokes a remotable method exposed statically within this class on the server, parses the response as binary data.,
        /// </summary>
        /// <param name="method">The method to invoke (without the class name), e.g. <code>"doSomething"</code>.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void InvokeStaticMethod(string method,
                                  Dictionary<string, object> parameters, Action<byte[], string> onSuccess, Action<Exception> onError)
        {
            if (Adapter == null)
            {
                throw new ArgumentException("No adapter set");
            }
            string path = _className + "." + method;
            Adapter.InvokeStaticMethod(path, parameters, onSuccess, onError);
        }
    }
}