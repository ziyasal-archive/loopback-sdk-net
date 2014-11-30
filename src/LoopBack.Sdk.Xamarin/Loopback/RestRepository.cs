using Loopback.Sdk.Xamarin.Loopback.Adapters;
using Loopback.Sdk.Xamarin.Remoting;

namespace Loopback.Sdk.Xamarin.Loopback
{
    public class RestRepository<T> : RemoteRepository<T> where T : RemoteClass, new()
    {
        public RestRepository(string remoteClassName) : base(remoteClassName)
        {
        }

        /// <summary>
        ///     Creates new RestContact
        /// </summary>
        /// <returns> Return new <see cref="Model" /> instance. </returns>
        public virtual void SetRemoting(T model)
        {
        }

        /// <summary>
        ///     Returns Repository adapter as <see cref="Adapters.RestAdapter" />
        /// </summary>
        /// <returns></returns>
        public IRestAdapter GetRestAdapter()
        {
            return (IRestAdapter) Adapter;
        }
    }
}