using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;
using Newtonsoft.Json.Linq;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class ContainerRepository : RestRepository<Container>
    {
        public ContainerRepository(string className) : base(className)
        {
        }

        public ContainerRepository() : base("container")
        {
        }

        /// <summary>
        ///     Creates a <see cref="RestContract" /> representing the user type's custom
        ///     routes.Used to extend an<see cref="Adapter" /> to support user.Calls base <see cref="ModelRepository{T}" />
        ///     createContract first.
        /// </summary>
        /// <returns></returns>
        public override RestContract CreateContract()
        {
            var contract = base.CreateContract();


            var basePath = "/" + GetNameForRestUrl();
            contract.AddItem(new RestContractItem(basePath, "POST"), ClassName + ".create");

            contract.AddItem(new RestContractItem(basePath, "GET"), ClassName + ".getAll");

            contract.AddItem(new RestContractItem(basePath + "/:name", "GET"), ClassName + ".get");

            contract.AddItem(new RestContractItem(basePath + "/:name", "DELETE"), ClassName + ".prototype.remove");

            return contract;
        }

        /// <summary>
        ///     Create a new container.
        /// </summary>
        /// <param name="name">The name of the container, must be unique.</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void Create(string name, Action<Container> onSuccess, Action<Exception> onError)
        {
            var parameters = new Dictionary<string, object>
            {
                {"name", name}
            };

            InvokeStaticMethod("create", parameters, JsonObjectParserHandler(onSuccess, onError), onError);
        }

        private Action<string> JsonObjectParserHandler(Action<Container> onSuccess, Action<Exception> onError)
        {
            return response =>
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

                    var container = CreateObject(creationParameters);
                    onSuccess(container);
                }
                catch (Exception ex)
                {
                    onError(ex);
                }
            };
        }

        /// <summary>
        ///     Get a named container
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void Get(string containerName, Action<Container> onSuccess, Action<Exception> onError)
        {
            var creationParameters = new Dictionary<string, object>
            {
                {"name", containerName}
            };

            InvokeStaticMethod("get", creationParameters, JsonObjectParserHandler(onSuccess, onError), onError);
        }

        /// <summary>
        ///     List all containers.
        /// </summary>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void GetAll(Action<List<Container>> onSuccess, Action<Exception> onError)
        {
            //WARNING: REusable Action
            InvokeStaticMethod("getAll", null, response =>
            {
                var result = new List<Container>();
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

        private string GetNameForRestUrl()
        {
            return "containers";
        }
    }
}