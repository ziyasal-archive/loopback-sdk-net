using System;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class RestRepository<T> : Repository<T> where T : VirtualObject
    {
        public RestRepository(string className)
            : base(className)
        {
        }

        public RestRepository(string className, Type objectClass)
            : base(className, objectClass)
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