using System;
using System.Collections.Generic;
using System.IO;
using LoopBack.Sdk.Xamarin.Remooting;
using Newtonsoft.Json;
using PCLStorage;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class File : RemoteClass
    {
        /// <summary>
        /// The name of the file, e.g. "image.gif"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URL of the file.
        /// </summary>
        public string Url { get; set; }

        [JsonIgnore]
        public Container ContainerRef { get; set; }

        /// <summary>
        /// Name of the container this file belongs to.
        /// </summary>
        /// <returns>The container name</returns>
        public string GetContainer()
        {
            return ContainerRef.Name;
        }

        /// <summary>
        /// Download content of this file.
        /// </summary>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void Download(Action<byte[], string> onSuccess, Action<Exception> onError)
        {
            InvokeMethod("download", GetCommonParams(), onSuccess, onError);
        }

        /// <summary>
        /// Download content of this file to a local file.
        /// </summary>
        /// <param name="localFile">Path to the local file.</param>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void Download(IFile localFile, Action onSuccess, Action<Exception> onError)
        {
            Download((fileContent, contentType) =>
            {
                try
                {
                    //TODO: Write to filesytem.

                    onSuccess();
                }
                catch (IOException ex)
                {
                    //TODO:Log
                    onError(ex);
                }
            }, onError);
        }

        /// <summary>
        /// Delete this file.
        /// </summary>
        /// <param name="onSuccess">The callback to invoke when the execution finished with success</param>
        /// <param name="onError">The callback to invoke when the execution finished with error</param>
        public void Delete(Action onSuccess, Action<Exception> onError)
        {
            InvokeMethod("delete", GetCommonParams(), responseContent =>
            {
                onSuccess();
            }, onError);
        }


        /// <summary>
        /// Download content of this file.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetCommonParams()
        {
            Dictionary<string, object> result = new Dictionary<string, object>
            {
                { "container",GetContainer()},
                {"name",Name}
            };

            return result;
        }
    }
}