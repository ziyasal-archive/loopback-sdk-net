using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Remooting;
using PCLStorage;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class Container : VirtualObject
    {
        public virtual string Name { set; get; }


        public virtual void Upload(IFile file, Action<File> callback)
        {
            FileRepository.Upload(file, callback);
        }

        public virtual void Upload(string fileName, byte[] content, string contentType, Action<File> callback)
        {
            FileRepository.Upload(fileName, content, contentType, callback);
        }
   
        public virtual File CreateFileObject(string name)
        {
            return FileRepository.CreateObject(new Dictionary<string, object>() { { "name", name } });
        }
   
        public virtual void GetFile(string fileName, Action<File> callback)
        {
            FileRepository.Get(fileName, callback);
        }
 
        public virtual void GetAllFiles(Action<File> callback)
        {
            FileRepository.GetAll(callback);
        }

        public virtual FileRepository FileRepository
        {
            get
            {
                RestAdapter adapter = ((RestAdapter)Repository.Adapter);
                FileRepository repo = adapter.CreateRepository<FileRepository,File>();
                repo.Container = this;
                return repo;
            }
        }
    }

}