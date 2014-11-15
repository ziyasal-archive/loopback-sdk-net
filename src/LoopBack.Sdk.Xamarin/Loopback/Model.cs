using System;
using System.Collections.Generic;
using System.Linq;
using LoopBack.Sdk.Xamarin.Extensions;
using LoopBack.Sdk.Xamarin.Remoting;
using Newtonsoft.Json.Linq;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class Model : RemoteClass
    {
        private readonly Dictionary<string, object> _overflow = new Dictionary<string, object>();

        public Model()
            : this(null, null)
        {
        }

        public Model(IRemoteRepository repository, Dictionary<string, object> creationParameters)
            : base(repository, creationParameters)
        {
        }

        /// <summary>
        ///     The model's id field.
        /// </summary>
        public object Id { get; internal set; }

        /// <summary>
        ///     Gets the value associated with a given key.
        /// </summary>
        /// <param name="key">The key for which to return the corresponding value.</param>
        /// <returns>The value associated with the key, or <code>null</code> if no value is associated with the key.</returns>
        public object Get(string key)
        {
            object result;
            if (_overflow.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        ///     Adds a given key-value pair to the dictionary.
        /// </summary>
        /// <param name="key">The key for value. If the key already exists in the dictionary, the specified value takes its place.</param>
        /// <param name="value">The value for the key. The value may be <code>null</code>.</param>
        public void Put(string key, object value)
        {
            _overflow.Add(key, value);
        }

        /// <summary>
        ///     Adds all the specified params to the dictionary.
        /// </summary>
        /// <param name="parameters">The parameters to add.</param>
        public void PutAll(Dictionary<string, object> parameters)
        {
            _overflow.AddRange(parameters);
        }

        protected override Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();

            result.AddRange(_overflow);

            result.Add("id", Id);

            //TODO:
            result.AddRange(base.ToDictionary());

            return result;
        }

        /// <summary>
        ///     Saves the Model to the server.
        ///     <p> This method calls <see cref="RemoteClass.ToDictionary()" /> to determine which fields should be saved.</p>
        /// </summary>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void Save(Action onSuccess, Action<Exception> onError)
        {
            var method = Id == null ? "create" : "save";
            InvokeMethod(method, ToDictionary(), responseContent =>
            {
                try
                {
                    var response = JObject.Parse(responseContent);

                    JToken id;
                    if (response.TryGetValue("id", out id))
                    {
                        if (id.HasValues)
                            Id = id.Values().First();
                    }

                    onSuccess();
                }
                catch (Exception ex)
                {
                    //TODO:Log
                }
            }, onError);
        }

        /// <summary>
        ///     Destroys the Model from the server.
        /// </summary>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void Destroy(Action onSuccess, Action<Exception> onError)
        {
            InvokeMethod("remove", ToDictionary(), responseContent => { onSuccess(); }, onError);
        }
    }
}