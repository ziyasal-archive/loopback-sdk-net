using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class RestRepository<T> : RemoteRepository<T> where T : RemoteClass
    {
        public RestRepository(string className)
            : base(className)
        {
        }

        public virtual RestContract CreateContract()
        {
            return new RestContract();
        }

        public RestAdapter GetRestAdapter()
        {
            return (RestAdapter)Adapter;
        }

        protected IContext GetApplicationContext()
        {
            return GetRestAdapter().ApplicationContext;
        }
    }
}