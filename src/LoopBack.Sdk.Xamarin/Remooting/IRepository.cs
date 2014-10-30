using LoopBack.Sdk.Xamarin.Remooting.Adapters;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    public interface IRepository
    {
        Adapter Adapter { get; set; }

        string ClassName { get; }
    }
}