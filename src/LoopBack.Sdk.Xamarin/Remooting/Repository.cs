using System;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    public class Repository<T> : IRepository where T : VirtualObject
    {
        public Adapter Adapter { get; set; }

        public Repository(string className)
        {

        }

        public Repository(string className, Type classType)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetClassName()
        {
            throw new NotImplementedException();
        }
    }
}