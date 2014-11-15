using System;
using System.Collections.Generic;
using System.IO;
using LoopBack.Sdk.Xamarin.Extensions;
using LoopBack.Sdk.Xamarin.Remoting.Adapters;
using Newtonsoft.Json.Linq;
using PCLStorage;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class FileRepository : RestRepository<File>
    {
        private static string TAG = "FileRepository";

        public FileRepository() : base("file")
        {
        }

        public Container Container { get; set; }

        public string GetContainerName()
        {
            return Container.Name;
        }

        /// <summary>
        ///     Creates a <see cref="RestContract" /> representing the user type's custom  routes. Used to extend an
        ///     <see cref="Adapter" /> to support user.
        ///     Calls base <see cref="ModelRepository{T}" /> createContract first.
        /// </summary>
        /// <returns>A <see cref="RestContract" /> for this model type</returns>
        public override RestContract CreateContract()
        {
            var contract = base.CreateContract();

            const string basePath = "/containers/:container";


            contract.AddItem(new RestContractItem(basePath + "/files/:name", "GET"), ClassName + ".get");

            contract.AddItem(new RestContractItem(basePath + "/files", "GET"), ClassName + ".getAll");

            contract.AddItem(RestContractItem.CreateMultipart(basePath + "/upload", "POST"), ClassName + ".upload");

            contract.AddItem(new RestContractItem(basePath + "/download/:name", "GET"),
                ClassName + ".prototype.download");

            contract.AddItem(new RestContractItem(basePath + "/files/:name", "DELETE"), ClassName + ".prototype.delete");

            return contract;
        }

        public override File CreateObject(Dictionary<string, object> creationParameters)
        {
            var file = base.CreateObject(creationParameters);
            file.ContainerRef = Container;
            return file;
        }

        private Action<string> UploadResponseParserHandler(Action<File> onSuccess, Action<Exception> onError)
        {
            return response =>
            {
                try
                {
                    //TODO: Test
                    var data = JObject.Parse(response);
                    var files = data["files"];
                    var fileJson = files.First;

                    var creationParams = fileJson.ToString().ToDictionaryFromJson();
                    var fileObj = CreateObject(creationParams);
                    onSuccess(fileObj);
                }
                catch (Exception ex)
                {
                    onError(ex);
                    //TODO:Log
                }
            };
        }

        /// <summary>
        ///     Upload a new file
        /// </summary>
        /// <param name="fileName">The file name, must be unique within the container.</param>
        /// <param name="content">Content of the file.</param>
        /// <param name="contentType">Content type (optional).</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void Upload(string fileName, byte[] content, string contentType, Action<File> onSuccess,
            Action<Exception> onError)
        {
            Upload(fileName, new MemoryStream(content), contentType, onSuccess, onError);
        }

        /// <summary>
        ///     Upload a new file
        /// </summary>
        /// <param name="fileName">The file name, must be unique within the container.</param>
        /// <param name="content">Content of the file.</param>
        /// <param name="contentType">Content type (optional).</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void Upload(string fileName, Stream content, string contentType, Action<File> onSuccess,
            Action<Exception> onError)
        {
            var streamParam = new StreamParam(content, fileName, contentType);
            var parameters = new Dictionary<string, object>
            {
                {"container", GetContainerName()},
                {"file", streamParam}
            };

            InvokeStaticMethod("upload", parameters, UploadResponseParserHandler(onSuccess, onError), onError);
        }

        /// <summary>
        ///     Upload a new file
        /// </summary>
        /// <param name="name">The file name, must be unique within the container.</param>
        /// <param name="file">Content of the file.</param>
        /// <param name="contentType">Content type (optional).</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void Upload(string name, IFile file, string contentType, Action<File> onSuccess,
            Action<Exception> onError)
        {
            var allText = file.ReadAllTextAsync();
            var content = allText.Result.GetBytes();
            Upload(name, content, contentType, onSuccess, onError);
        }

        /// <summary>
        ///     Upload a new file
        /// </summary>
        /// <param name="file">The local file to upload.</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void Upload(IFile file, Action<File> onSuccess, Action<Exception> onError)
        {
            var allText = file.ReadAllTextAsync();
            var content = allText.Result.GetBytes();
            Upload(file.Name, content, null, onSuccess, onError);
        }

        /// <summary>
        ///     Get file by name
        /// </summary>
        /// <param name="fileName">The name of the file to get.</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void Get(string fileName, Action<File> onSuccess, Action<Exception> onError)
        {
            //TODO: Test
            var parameters = new Dictionary<string, object>
            {
                {"container", GetContainerName()},
                {"name", fileName}
            };

            InvokeStaticMethod("get", parameters, response =>
            {
                //TODO: valid json
                var creationParams = response.ToDictionaryFromJson();
                var file = CreateObject(creationParams);
                onSuccess(file);
            }, onError);
        }

        /// <summary>
        ///     List all files in the container.
        /// </summary>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public void GetAll(Action<List<File>> onSuccess, Action<Exception> onError)
        {
            //TODO: Test
            var dictionary = new Dictionary<string, object>
            {
                {"container", GetContainerName()}
            };

            //WARNING: REusable Action
            InvokeStaticMethod("getAll", dictionary, response =>
            {
                var result = new List<File>();
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