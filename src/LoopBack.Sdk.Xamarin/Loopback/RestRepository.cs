using LoopBack.Sdk.Xamarin.Remoting;
using LoopBack.Sdk.Xamarin.Remoting.Adapters;
using LoopBack.Sdk.Xamarin.Shared;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class RestRepository<T> : RemoteRepository<T> where T : RemoteClass
    {
        public RestRepository(string remoteClassName)
            : base(remoteClassName)
        {
        }

        public virtual RestContract CreateContract()
        {
            return new RestContract();
        }

        public RestAdapter GetRestAdapter()
        {
            return (RestAdapter) Adapter;
        }

        protected IContext GetApplicationContext()
        {
            return GetRestAdapter().ApplicationContext;
        }
    }
}