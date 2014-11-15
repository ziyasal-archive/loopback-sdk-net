using LoopBack.Sdk.Xamarin.Common;

namespace LoopBack.Sdk.Xamarin.Tests.Remooting
{
    public class DummyContextImpl : IContext
    {
		string _UserAgent;
		public string UserAgent {
			get {
				return _UserAgent;
			}
			set {
				_UserAgent = value;
			}
		}

		public DummyContextImpl(string UserAgent) {
			_UserAgent = UserAgent;
		}
    }
}