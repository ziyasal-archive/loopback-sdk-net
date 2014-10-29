using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    public class Repository<T> : IRepository where T : VirtualObject
    {
        private readonly Type _objectClass;
        private readonly string _className;

        public Repository(string className)
        {
            _className = className;
        }

        public Repository(string className, Type objectClass)
        {
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentNullException("Class name cannot be null or empty.");
            }
            _className = className;

            _objectClass = objectClass ?? typeof(VirtualObject);
        }

        public Adapter Adapter { get; set; }

        public string GetClassName()
        {
            return _className;
        }

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
            objectToCreate.Repository = this;

            if (creationParameters != null)
            {
                objectToCreate.CreationParameters = creationParameters;
                creationParameters.ToObject(objectToCreate);
            }

            return objectToCreate;
        }

        public void InvokeStaticMethod(string method, Dictionary<string, object> parameters, Action<string> onSuccess, Action<Exception> onError)
        {
            if (Adapter == null)
            {
                throw new ArgumentNullException("No adapter set");
            }
            string path = _className + "." + method;
            Adapter.InvokeStaticMethod(path, parameters, onSuccess, onError);
        }

        public void InvokeStaticMethod(string method,
                                  Dictionary<string, object> parameters, Action<byte[], string> onSuccess, Action<Exception> onError)
        {
            if (Adapter == null)
            {
                throw new ArgumentNullException("No adapter set");
            }
            string path = _className + "." + method;
            Adapter.InvokeStaticMethod(path, parameters, onSuccess, onError);
        }
    }
}