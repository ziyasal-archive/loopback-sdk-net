using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Humanizer;
using Loopback.Sdk.Xamarin.Extensions;
using Loopback.Sdk.Xamarin.Remoting.Adapters;

namespace Loopback.Sdk.Xamarin.Loopback
{
    /// <summary>
    ///     A local representative of a single model type on the server, encapsulating the name of the model type
    ///     <see cref="Model" /> for easy  creation, discovery, and management.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelRepository<T> : RestRepository<T> where T : Model
    {
        public ModelRepository(string remoteClassName) : base(remoteClassName)
        {
        }

        /// <summary>
        ///     Creates a new Repository, associating it with the named remote class.
        /// </summary>
        /// <param name="remoteClassName">The remote class name.</param>
        /// <param name="nameForRestUrl">
        ///     The pluralized class name to use in REST transport. Use <code>null</code> for the default
        ///     value, which is the plural form of className.
        /// </param>
        public ModelRepository(string remoteClassName, string nameForRestUrl)
            : base(remoteClassName)
        {
            NameForRestUrl = nameForRestUrl ?? remoteClassName.Pluralize(); //It uses Humanizer
        }

        /// <summary>
        ///     the name of the REST url
        /// </summary>
        public string NameForRestUrl { get; private set; }

        /// <summary>
        ///     Creates a <see cref="RestContract" /> representing this model type's custom  routes.Used to extend an
        ///     <see cref="AdapterBase" /> to support this model type.
        /// </summary>
        /// <returns>A <see cref="RestContract" /> for this model type.</returns>
        public override RestContract CreateContract()
        {
            var contract = base.CreateContract();

            contract.AddItem(new RestContractItem("/" + NameForRestUrl, "POST"), RemoteClassName + ".prototype.create");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl + "/:id", "PUT"),
                RemoteClassName + ".prototype.save");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl + "/:id", "DELETE"),
                RemoteClassName + ".prototype.remove");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl + "/:id", "GET"), RemoteClassName + ".findById");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl, "GET"), RemoteClassName + ".all");

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
            if (creationParameters.ContainsKey("id"))
            {
                var id = creationParameters["id"];
                if (id != null)
                {
                    model.Id = id;
                }
            }

            return model;
        }

        /// <summary>
        ///     Finds and downloads a single instance of this model type on and from the server with the given id.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        public async Task<RestResponse<T>> FindById(object id)
        {
            var result = new RestResponse<T>();
            var parameters = new Dictionary<string, object>
            {
                {"id", id}
            };

            try
            {
                var response = await InvokeStaticMethod("findById", parameters);

                if (response.IsSuccessStatusCode)
                {
                    result.Result = response.ReadAs<T>();
                }
                else
                {
                    result.Exception = response.Exception;
                }
            }
            catch (Exception exception)
            {
                result.Exception = exception;
                throw;
            }

            return result;
        }

        /// <summary>
        ///     Finds and downloads all models of this type on and from the server.
        /// </summary>
        public async Task<RestResponse<List<T>>> FindAll()
        {
            var result = new RestResponse<List<T>>();

            var response = await InvokeStaticMethod("all", null);

            if (response.IsSuccessStatusCode)
            {
                result.Result = response.ReadAs<List<T>>();
            }
            else
            {
                result.Exception = response.Exception;
            }

            return result;
        }
    }
}