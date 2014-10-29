using System;
using System.Collections.Generic;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;
using PCLStorage;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class FileRepository : RestRepository<File>
    {
        private static String TAG = "FileRepository";

        public Container Container { get; set; }

        public FileRepository()
            : base("file", typeof(File))
        {
        }

        public string GetContainerName()
        {
            return Container.Name;
        }

        public override RestContract CreateContract()
        {
            RestContract contract = new RestContract();

            const string basePath = "/containers/:container";
            string className = GetClassName();

            contract.AddItem(new RestContractItem(basePath + "/files/:name", "GET"), className + ".get");

            contract.AddItem(new RestContractItem(basePath + "/files", "GET"), className + ".getAll");

            contract.AddItem(RestContractItem.CreateMultipart(basePath + "/upload", "POST"), className + ".upload");

            contract.AddItem(new RestContractItem(basePath + "/download/:name", "GET"), className + ".prototype.download");

            contract.AddItem(new RestContractItem(basePath + "/files/:name", "DELETE"), className + ".prototype.delete");

            return contract;
        }

        public override File CreateObject(Dictionary<string, object> creationParameters)
        {
            File file = base.CreateObject(creationParameters);
            file.ContainerRef = Container;
            return file;
        }

        public void Upload(IFile file, Action<File> callback)
        {
            throw new NotImplementedException();
        }

        public void Upload(string fileName, byte[] content, string contentType, Action<File> callback)
        {
            throw new NotImplementedException();
        }

        public void Get(string fileName, Action<File> callback)
        {
            throw new NotImplementedException();
        }

        public void GetAll(Action<File> callback)
        {
            throw new NotImplementedException();
        }
    }
}