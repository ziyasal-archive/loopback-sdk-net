using Loopback.Sdk.Xamarin.Loopback.Adapters;
using NUnit.Framework;

namespace Loopback.Sdk.Xamarin.Test.Loopback
{
    public class RestAdapterTests : TestBase
    {
        private IRestAdapter _adapter;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateAdapter();
            TestSuite.SetupFor("MyModel");
        }

        [Test]
        public void Test()
        {
        }
    }
}