using LoopBack.Sdk.Xamarin.Remooting.Adapters;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    public interface IRepository
    {
        /// <summary>
        /// 
        /// </summary>
        Adapter Adapter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetClassName();
    }
}