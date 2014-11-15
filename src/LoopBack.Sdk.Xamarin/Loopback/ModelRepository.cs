using System;
using System.Collections.Generic;
using Humanizer;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;
using Newtonsoft.Json.Linq;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    /// <summary>
    ///     A local representative of a single model type on the server, encapsulating the name of the model type for easy
    ///     <see cref="Model" />
    ///     creation, discovery, and management.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelRepository<T> : RestRepository<T> where T : Model
    {
        /// <summary>
        ///     Creates a new Repository, associating it with the named remote class.
        /// </summary>
        /// <param name="className">The remote class name.</param>
        public ModelRepository(string className) : this(className, null)
        {
        }

        /// <summary>
        ///     Creates a new Repository, associating it with the named remote class.
        /// </summary>
        /// <param name="className">The remote class name.</param>
        /// <param name="nameForRestUrl">
        ///     The pluralized class name to use in REST transport. Use <code>null</code> for the default
        ///     value, which is the plural form of className.
        /// </param>
        public ModelRepository(string className, string nameForRestUrl)
            : base(className)
        {
            NameForRestUrl = nameForRestUrl ?? className.Pluralize(); //It uses Humanizer
        }

        /// <summary>
        ///     the name of the REST url
        /// </summary>
        public string NameForRestUrl { get; private set; }

        /// <summary>
        ///     Creates a <see cref="RestContract" /> representing this model type's custom  routes.Used to extend an
        ///     <see cref="Adapter" /> to support this model type.
        /// </summary>
        /// <returns>A <see cref="RestContract" /> for this model type.</returns>
        public override RestContract CreateContract()
        {
            var contract = base.CreateContract();

            contract.AddItem(new RestContractItem("/" + NameForRestUrl, "POST"), ClassName + ".prototype.create");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl + "/:id", "PUT"), ClassName + ".prototype.save");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl + "/:id", "DELETE"),
                ClassName + ".prototype.remove");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl + "/:id", "GET"), ClassName + ".findById");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl, "GET"), ClassName + ".all");

            return contract;
        }

        /// <summary>
        ///     Creates a new <see cref="Model" />of this type with the parameters described.
        /// </summary>
        /// <param name="creationParameters">The creation parameters.</param>
        /// <returns>A new <see cref="Model" />.</returns>
        public override T CreateObject(Dictionary<string, object> creationParameters)
        {
            var model = base.CreateObject(creationParameters);
            model.PutAll(creationParameters);

            var id = creationParameters["id"];
            if (id != null)
            {
                model.Id = id;
            }

            return model;
        }

        /// <summary>
        ///     Finds and downloads a single instance of this model type on and from the server with the given id.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void FindById(object id, Action<T> onSuccess, Action<Exception> onError)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("id", id);
            InvokeStaticMethod("findById", parameters, response =>
            {
                try
                {
                    if (string.IsNullOrEmpty(response))
                    {
                        // Not found
                        onSuccess(null);
                        return;
                    }

                    var creationParameters = response.ToDictionaryFromJson();
                    var created = CreateObject(creationParameters);
                    onSuccess(created);
                }
                catch (Exception ex)
                {
                    onError(ex);
                }
            }, onError);
        }

        /// <summary>
        ///     Finds and downloads all models of this type on and from the server.
        /// </summary>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void FindAll(Action<List<T>> onSuccess, Action<Exception> onError)
        {
            //WARNING: REusable Action
            InvokeStaticMethod("all", null, response =>
            {
                var result = new List<T>();
                var jObject = JObject.Parse(response);

                //TODO: Handle list in JObject
                foreach (var pair in jObject)
                {
                    var creationParams = pair.Value.ToString().ToDictionaryFromJson();
                    var file = CreateObject(creationParams);
                    result.Add(file);
                }

                onSuccess(result);
            }, onError);
        }
    }
}