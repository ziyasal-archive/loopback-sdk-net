using LoopBack.Sdk.Xamarin.Remooting.Adapters;

namespace LoopBack.Sdk.Xamarin.Remooting
{
    public interface IRemoteRepository
    {
        AdapterBase Adapter { get; set; }
        string ClassName { get; }
    }
}