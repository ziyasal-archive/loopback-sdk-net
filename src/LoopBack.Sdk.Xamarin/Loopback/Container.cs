using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Remooting;
using PCLStorage;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class Container : VirtualObject
    {
        public virtual string Name { set; get; }


        public virtual void Upload(IFile file, Action<File> onSuccess, Action<Exception> onError)
        {
            FileRepository.Upload(file, onSuccess, onError);
        }

        public virtual void Upload(string fileName, byte[] content, string contentType, Action<File> onSuccess, Action<Exception> onError)
        {
            FileRepository.Upload(fileName, content, contentType, onSuccess, onError);
        }

        public virtual File CreateFileObject(string name)
        {
            return FileRepository.CreateObject(new Dictionary<string, object> { { "name", name } });
        }

        public virtual void GetFile(string fileName, Action<File> onSuccess, Action<Exception> onError)
        {
            FileRepository.Get(fileName, onSuccess, onError);
        }

        public virtual void GetAllFiles(Action<List<File>> onSuccess, Action<Exception> onError)
        {
            FileRepository.GetAll(onSuccess, onError);
        }

        public virtual FileRepository FileRepository
        {
            get
            {
                RestAdapter adapter = ((RestAdapter)Repository.Adapter);
                FileRepository repo = adapter.CreateRepository<FileRepository, File>();
                repo.Container = this;
                return repo;
            }
        }
    }

}