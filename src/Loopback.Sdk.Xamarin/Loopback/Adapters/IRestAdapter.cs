using System;
using Loopback.Sdk.Xamarin.Remoting;
using Loopback.Sdk.Xamarin.Remoting.Adapters;

namespace Loopback.Sdk.Xamarin.Loopback.Adapters
{
    public interface IRestAdapter : IRemotingRestAdapter
    {
        string AccessToken { set; }
        void ClearAccessToken();

        /// <summary>
        ///     Creates a new <see cref="ModelRepository{T}" /> representing the named model type.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <returns> A new repository instance.</returns>
        ModelRepository<Model> CreateRepository(string name);

        /// <summary>
        ///     Creates a new <see cref="ModelRepository{T}" />  representing the named model type.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <param name="nameForRestUrl">The model name to use in REST URL, usually the plural form of `name`.</param>
        /// <returns>A new repository instance.</returns>
        ModelRepository<Model> CreateRepository(string name, string nameForRestUrl);

        /// <summary>
        ///     Creates a new <see cref="ModelRepository{T}" /> representing the named model type.
        /// </summary>
        /// <typeparam name="T">The model type that inherited from <see cref="Model" />.</typeparam>
        /// <param name="name">The model name.</param>
        /// <param name="nameForRestUrl">The model name to use in REST URL, usually the plural form of `name`.</param>
        /// <param name="modelClass">modelClass The model class. The class must have a public no-argument constructor.</param>
        /// <returns>A new repository instance.</returns>
        ModelRepository<T> CreateRepository<T>(string name, string nameForRestUrl, Type modelClass)
            where T : Model;

        /// <summary>
        ///     Creates a new <see cref="ModelRepository{T}" /> from the given subclass.
        /// </summary>
        /// <typeparam name="TRepository">
        ///     A subclass of <see cref="RestRepository{T}" /> to use. The class must have a public
        ///     no-argument constructor.
        /// </typeparam>
        /// <typeparam name="T">The model calss that inherited from <see cref="Model" /></typeparam>
        /// <returns>A new repository instance.</returns>
        TRepository CreateRepository<TRepository, T>()
            where TRepository : RestRepository<T>, new()
            where T : RemoteClass;
    }
}