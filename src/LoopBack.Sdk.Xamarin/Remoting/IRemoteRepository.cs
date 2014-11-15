using LoopBack.Sdk.Xamarin.Remoting.Adapters;

namespace LoopBack.Sdk.Xamarin.Remoting
{
    public interface IRemoteRepository
    {
        AdapterBase Adapter { get; set; }
        string ClassName { get; }
    }
}