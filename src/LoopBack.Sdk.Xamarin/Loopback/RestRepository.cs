using Loopback.Sdk.Xamarin.Loopback.Adapters;
using Loopback.Sdk.Xamarin.Remoting;
using Loopback.Sdk.Xamarin.Remoting.Adapters;

namespace Loopback.Sdk.Xamarin.Loopback
{
    public class RestRepository<T> : RemoteRepository<T> where T : RemoteClass
    {
        public RestRepository(string remoteClassName) : base(remoteClassName)
        {
        }

        /// <summary>
        ///     Creates new RestContact
        /// </summary>
        /// <returns> Return new <see cref="RestContract" /> instance. </returns>
        public virtual RestContract CreateContract()
        {
            return new RestContract();
        }

        /// <summary>
        ///     Returns Repository adapter as <see cref="RestAdapter" />
        /// </summary>
        /// <returns></returns>
        public IRestAdapter GetRestAdapter()
        {
            return (IRestAdapter) Adapter;
        }
    }
}