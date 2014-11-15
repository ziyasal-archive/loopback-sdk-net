using LoopBack.Sdk.Xamarin.Shared;

namespace LoopBack.Sdk.Xamarin.Tests.Remoting
{
    public class DummyContextImpl : IContext
    {
        public DummyContextImpl(string UserAgent)
        {
            this.UserAgent = UserAgent;
        }

        public string UserAgent { get; set; }
    }
}