using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Remoting;
using PCLStorage;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class Container : RemoteClass
    {
        public virtual string Name { set; get; }

        public virtual FileRepository FileRepository
        {
            get
            {
                var adapter = ((RestAdapter) RemoteRepository.Adapter);
                var repo = adapter.CreateRepository<FileRepository, File>();
                repo.Container = this;
                return repo;
            }
        }

        /// <summary>
        ///     Upload a new file
        /// </summary>
        /// <param name="file">Content of the file.</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public virtual void Upload(IFile file, Action<File> onSuccess, Action<Exception> onError)
        {
            FileRepository.Upload(file, onSuccess, onError);
        }

        /// <summary>
        ///     Upload a new file
        /// </summary>
        /// <param name="fileName">The file name, must be unique within the container.</param>
        /// <param name="content">Content of the file.</param>
        /// <param name="contentType">Content type (optional).</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public virtual void Upload(string fileName, byte[] content, string contentType, Action<File> onSuccess,
            Action<Exception> onError)
        {
            FileRepository.Upload(fileName, content, contentType, onSuccess, onError);
        }

        /// <summary>
        ///     Create a new File object associated with this container.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>The <see cref="File" /> object created</returns>
        public virtual File CreateFileObject(string name)
        {
            return FileRepository.CreateObject(new Dictionary<string, object> {{"name", name}});
        }

        /// <summary>
        ///     Get data of a File object.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public virtual void GetFile(string fileName, Action<File> onSuccess, Action<Exception> onError)
        {
            FileRepository.Get(fileName, onSuccess, onError);
        }

        /// <summary>
        ///     List all files in the container.
        /// </summary>
        /// <param name="onSuccess">The onSuccess to invoke when the execution finished with success</param>
        /// <param name="onError">The onSuccess to invoke when the execution finished with error</param>
        public virtual void GetAllFiles(Action<List<File>> onSuccess, Action<Exception> onError)
        {
            FileRepository.GetAll(onSuccess, onError);
        }
    }
}