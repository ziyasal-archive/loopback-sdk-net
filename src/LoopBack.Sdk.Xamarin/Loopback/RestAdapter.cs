using System;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    /// <summary>
    /// An extension to the vanilla <see cref="RestAdapter"/> to make working with <see cref="Model"/> s easier.
    /// </summary>
    public class RestAdapter : Remooting.Adapters.RestAdapter
    {
        public static string SHARED_PREFERENCES_NAME = "RestAdapter";
        public static string PROPERTY_ACCESS_TOKEN = "accessToken";
        private readonly IContext _context;

        public RestAdapter(IContext context, string url)
            : base(context, url)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "context must be not null");

            }

            _context = context;
            AccessToken = LoadAccessToken();
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

        public virtual IContext ApplicationContext
        {
            get { return _context; }
        }

        public virtual void ClearAccessToken()
        {
            Client.DefaultRequestHeaders.Add("Authorization", string.Empty);
        }

        /// <summary>
        /// Creates a new <see cref="ModelRepository{T}"/> representing the named model type.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <returns> A new repository instance.</returns>
        public virtual ModelRepository<Model> CreateRepository(string name)
        {
            return CreateRepository<Model>(name, null, null);
        }


        /// <summary>
        /// Creates a new <see cref="ModelRepository{T}"/>  representing the named model type.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <param name="nameForRestUrl">The model name to use in REST URL, usually the plural form of `name`.</param>
        /// <returns>A new repository instance.</returns>
        public virtual ModelRepository<Model> CreateRepository(string name, string nameForRestUrl)
        {
            return CreateRepository<Model>(name, nameForRestUrl, null);
        }

        /// <summary>
        /// Creates a new <see cref="ModelRepository{T}"/> representing the named model type.
        /// </summary>
        /// <typeparam name="T">The model type that inherited from <see cref="Model"/>.</typeparam>
        /// <param name="name">The model name.</param>
        /// <param name="nameForRestUrl">The model name to use in REST URL, usually the plural form of `name`.</param>
        /// <param name="modelClass">modelClass The model class. The class must have a public no-argument constructor.</param>
        /// <returns>A new repository instance.</returns>
        public virtual ModelRepository<T> CreateRepository<T>(string name, string nameForRestUrl, Type modelClass)
            where T : Model
        {
            var repository = new ModelRepository<T>(name, nameForRestUrl);
            AttachModelRepository(repository);
            return repository;
        }

        /// <summary>
        /// Creates a new <see cref="ModelRepository{T}"/> from the given subclass.
        /// </summary>
        /// <typeparam name="TRepository">A subclass of <see cref="RestRepository{T}"/> to use. The class must have a public no-argument constructor.</typeparam>
        /// <typeparam name="T">The model calss that inherited from <see cref="Model"/></typeparam>
        /// <returns>A new repository instance.</returns>
        public virtual TRepository CreateRepository<TRepository, T>()
            where TRepository : RestRepository<T>, new()
            where T : RemoteClass
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

        private void AttachModelRepository<T>(RestRepository<T> repository) where T : RemoteClass
        {
            Contract.AddItemsFromContract(repository.CreateContract());
            repository.Adapter = this;
        }

        private void SaveAccessToken(string accessToken)
        {
            //TODO: SharedPreferences
        }

        private string LoadAccessToken()
        {
            //TODO: SharedPreferences
            return null;
        }
    }
}