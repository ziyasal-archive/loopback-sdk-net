using System;
using Loopback.Sdk.Xamarin.Remoting;
using Loopback.Sdk.Xamarin.Shared;

namespace Loopback.Sdk.Xamarin.Loopback.Adapters
{
    public class RestAdapter : Remoting.Adapters.RestAdapter, IRestAdapter
    {
        private const string ACCESS_TOKEN_KEY = "loopback.accessToken";
        private readonly IStorageService _storageService;

        public RestAdapter(IContext context, string url, IStorageService storageService)
            : base(context, url)
        {
            _storageService = storageService;
            if (context == null)
            {
                throw new ArgumentNullException("context", "context must be not null");
            }

            AccessToken = LoadAccessToken();
        }

        public RestAdapter(string url)
            : base(url)
        {
        }

        public virtual string AccessToken
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    SaveAccessToken(value);
                    Client.DefaultRequestHeaders.Add("Authorization", value);
                }
            }
        }

        public virtual void ClearAccessToken()
        {
            Client.DefaultRequestHeaders.Add("Authorization", string.Empty);
        }

        /// <summary>
        ///     Creates a new <see cref="ModelRepository{T}" /> representing the named model type.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <returns> A new repository instance.</returns>
        public virtual ModelRepository<Model> CreateRepository(string name)
        {
            return CreateRepository<Model>(name, null, null);
        }

        /// <summary>
        ///     Creates a new <see cref="ModelRepository{T}" />  representing the named model type.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <param name="nameForRestUrl">The model name to use in REST URL, usually the plural form of `name`.</param>
        /// <returns>A new repository instance.</returns>
        public virtual ModelRepository<Model> CreateRepository(string name, string nameForRestUrl)
        {
            return CreateRepository<Model>(name, nameForRestUrl, null);
        }

        /// <summary>
        ///     Creates a new <see cref="ModelRepository{T}" /> representing the named model type.
        /// </summary>
        /// <typeparam name="T">The model type that inherited from <see cref="Model" />.</typeparam>
        /// <param name="name">The model name.</param>
        /// <param name="nameForRestUrl">The model name to use in REST URL, usually the plural form of `name`.</param>
        /// <param name="modelClass">modelClass The model class. The class must have a public no-argument constructor.</param>
        /// <returns>A new repository instance.</returns>
        public virtual ModelRepository<T> CreateRepository<T>(string name, string nameForRestUrl, Type modelClass)
            where T : Model, new()
        {
            var repository = new ModelRepository<T>(name, nameForRestUrl);
            AttachModelRepository(repository);
            return repository;
        }

        /// <summary>
        ///     Creates a new <see cref="ModelRepository{T}" /> from the given subclass.
        /// </summary>
        /// <typeparam name="TRepository">
        ///     A subclass of <see cref="RestRepository{T}" /> to use. The class must have a public
        ///     no-argument constructor.
        /// </typeparam>
        /// <typeparam name="T">The model calss that inherited from <see cref="Model" /></typeparam>
        /// <returns>A new repository instance.</returns>
        public virtual TRepository CreateRepository<TRepository, T>()
            where TRepository : RestRepository<T>, new()
            where T : RemoteClass, new()
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

        private void AttachModelRepository<T>(RestRepository<T> repository) where T : RemoteClass, new()
        {
            //TODO: 
            //Contract.AddItemsFromContract(repository.CreateContract());
            repository.Adapter = this;
        }

        private void SaveAccessToken(string accessToken)
        {
            _storageService.Save(ACCESS_TOKEN_KEY, accessToken);
        }

        private string LoadAccessToken()
        {
            return _storageService.Get<string>(ACCESS_TOKEN_KEY);
        }
    }
}