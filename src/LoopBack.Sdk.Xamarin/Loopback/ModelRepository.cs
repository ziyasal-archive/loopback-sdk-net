using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Humanizer;
using Loopback.Sdk.Xamarin.Extensions;

namespace Loopback.Sdk.Xamarin.Loopback
{
    /// <summary>
    ///     A local representative of a single model type on the server, encapsulating the name of the model type
    ///     <see cref="Model" /> for easy  creation, discovery, and management.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelRepository<T> : RestRepository<T> where T : Model, new()
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
        public string NameForRestUrl { get; }

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

            SetRemoting(model);

            return model;
        }

        public override void SetRemoting(T model)
        {
            base.SetRemoting(model);

            model.SetRemoting("/" + NameForRestUrl, "POST", RemoteClassName + ".prototype.create");
            model.SetRemoting("/" + NameForRestUrl + "/:id", "PUT", RemoteClassName + ".prototype.save");
            model.SetRemoting("/" + NameForRestUrl + "/:id", "DELETE", RemoteClassName + ".prototype.remove");
            model.SetRemoting("/" + NameForRestUrl + "/:id", "GET", RemoteClassName + ".findById");
            model.SetRemoting("/" + NameForRestUrl, "GET", RemoteClassName + ".all");
        }

        /// <summary>
        ///     Finds and downloads a single instance of this model type on and from the server with the given id.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        public async Task<RestResponse<T>> FindById(object id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            var result = new RestResponse<T>();
            var parameters = new Dictionary<string, object>
            {
                {"id", id}
            };

            var response = await InvokeStaticMethod("findById", parameters);

            result.FillFrom(response);

            if (response.IsSuccessStatusCode)
            {
                result.Result = response.ReadAs<T>();
            }
            else
            {
                result.Exception = response.Exception;
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

            result.FillFrom(response);

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