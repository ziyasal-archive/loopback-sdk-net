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

        public RestRepository(string className, Type classType)
            : base(className, classType)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RestContract CreateContract()
        {
            return new RestContract();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RestAdapter GetRestAdapter()
        {
            return (RestAdapter)Adapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IContext GetApplicationContext()
        {
            return GetRestAdapter().ApplicationContext;
        }
    }
}
