using Loopback.Sdk.Xamarin.Remoting.Adapters;

namespace Loopback.Sdk.Xamarin.Remoting
{
    public interface IRemoteRepository
    {
        IRemotingRestAdapter Adapter { get; set; }
        string RemoteClassName { get; }
    }
}