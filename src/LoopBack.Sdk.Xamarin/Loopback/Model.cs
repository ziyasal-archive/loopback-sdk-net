using System.Collections.Generic;
using System.Threading.Tasks;
using Loopback.Sdk.Xamarin.Extensions;
using Loopback.Sdk.Xamarin.Remoting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Loopback.Sdk.Xamarin.Loopback
{
    /// <summary>
    ///     A local representative of a single model instance on the server. The data is
    ///     immediately accessible locally, but can be saved, destroyed, etc.from the
    ///     server easily.
    /// </summary>
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
        [JsonProperty("id")]
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

            //result.AddRange(_overflow); //TODO:? Check

            result.Add("id", Id);

            //result.AddRange(base.ToDictionary()); //TODO:? Check

            return result;
        }

        /// <summary>
        ///     Saves the Model to the server.
        ///     <p> This method calls <see cref="RemoteClass.ToDictionary()" /> to determine which fields should be saved.</p>
        /// </summary>
        public async Task<RestResponse> Save()
        {
            var result = new RestResponse();
            var method = Id == null ? "create" : "save";

            var remotingResponse = await InvokeMethod(method, ToDictionary());

            result.FillFrom(remotingResponse);

            if (result.IsSuccessStatusCode)
            {
                var response = JObject.Parse(remotingResponse.Content);

                JToken id;
                if (response.TryGetValue("id", out id))
                {
                    Id = id.ToString();
                }
            }

            return result;
        }

        /// <summary>
        ///     Destroys the Model from the server.
        /// </summary>
        public async Task<RestResponse> Destroy()
        {
            var result = new RestResponse();

            var response = await InvokeMethod("remove", ToDictionary());

            if (response.IsSuccessStatusCode)
            {
                Id = null;
            }

            result.FillFrom(response);

            return result;
        }
    }
}