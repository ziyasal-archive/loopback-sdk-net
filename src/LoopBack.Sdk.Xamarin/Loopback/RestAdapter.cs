using System;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class RestAdapter : Remooting.Adapters.RestAdapter
    {
        public const string SHARED_PREFERENCES_NAME = "RestAdapter";
        public const string PROPERTY_ACCESS_TOKEN = "accessToken";
        private readonly IContext context;

        public RestAdapter(IContext context, string url)
            : base(context, url)
        {
            if (context == null)
                throw new ArgumentNullException("context must be not null");
            this.context = context;
            //AccessToken = loadAccessToken();
        }

        public virtual string AccessToken
        {
            set
            {
                //saveAccessToken(value);
                Client.DefaultRequestHeaders.Add("Authorization", value);
            }
        }

        public virtual IContext ApplicationContext
        {
            get { return context; }
        }

        public virtual void ClearAccessToken()
        {
            Client.DefaultRequestHeaders.Add("Authorization", string.Empty);
        }

        public virtual ModelRepository<Model> CreateRepository(string name)
        {
            return CreateRepository<Model>(name, null,null);
        }

        public virtual ModelRepository<Model> CreateRepository(string name, string nameForRestUrl)
        {
            return CreateRepository<Model>(name, nameForRestUrl,null);
        }

        public virtual ModelRepository<T> CreateRepository<T>(string name, string nameForRestUrl,Type modelClass)
            where T : Model
        {
            var repository = new ModelRepository<T>(name, nameForRestUrl, modelClass);
            AttachModelRepository(repository);
            return repository;
        }

        public virtual TRepository CreateRepository<TRepository, T>()
            where TRepository : RestRepository<T>, new()
            where T : VirtualObject
        {
            TRepository repository;
            try
            {
                repository = new TRepository
                {
                    Adapter = this
                };
            }
            catch (Exception e)
            {
                var ex = new ArgumentException("", e);
                throw ex;
            }

            AttachModelRepository(repository);

            return repository;
        }

        private void AttachModelRepository<T>(RestRepository<T> repository) where T : VirtualObject
        {
            Contract.AddItemsFromContract(repository.CreateContract());
            repository.Adapter = this;
        }
    }
}