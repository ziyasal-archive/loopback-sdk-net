using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    public abstract class Adapter
    {
        public enum ParameterEncoding
        {
            FORM_URL,
            JSON,
            FORM_MULTIPART
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        protected Adapter(IContext context)
            : this(context, null)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        protected Adapter(IContext context, string url)
        {
            if (url != null)
            {
                Url = url;
                Connect(context, url);
            }
        }

        public string Url { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        public abstract void Connect(IContext context, string url);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public abstract bool IsConnected();

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="onSucces"></param>
        /// <param name="onError"></param>
        public abstract void InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<string> onSucces,
            Action<Exception> onError);

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public virtual void InvokeStaticMethod(string method,
            Dictionary<string, object> parameters,
            Action<byte[], string> onSuccess,
            Action<Exception> onError)
        {
            throw new NotSupportedException(GetType().Name + " does not support binary responses.");
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="constructorParameters"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public abstract void InvokeInstanceMethod(string method,
            Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters,
            Action<string> onSuccess,
            Action<Exception> onError);

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="constructorParameters"></param>
        /// <param name="parameters"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public virtual void InvokeInstanceMethod(string method,
            Dictionary<string, object> constructorParameters,
            Dictionary<string, object> parameters,
            Action<byte[], string> onSuccess,
            Action<Exception> onError)
        {
            throw new NotSupportedException(GetType().Name + " does not support binary responses.");
        }
    }
}