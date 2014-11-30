using Loopback.Sdk.Xamarin.Loopback.Adapters;
using Loopback.Sdk.Xamarin.Remoting.Adapters;
using Loopback.Sdk.Xamarin.Shared;
using Loopback.Sdk.Xamarin.Test.Loopback;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestAdapter = Loopback.Sdk.Xamarin.Remoting.Adapters.RestAdapter;

namespace Loopback.Sdk.Xamarin.Test
{
    [TestFixture]
    public class TestBase
    {
        protected IFixture FixtureRepository { get; set; }
        protected bool VerifyAll { get; set; }

        [SetUp]
        public void Setup()
        {
            FixtureRepository = new Fixture();
            FinalizeSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            FinalizeTearDown();
        }

        protected void EnableCustomization(ICustomization customization)
        {
            customization.Customize(FixtureRepository);
        }

        protected virtual void FinalizeTearDown()
        {
        }

        protected virtual void FinalizeSetUp()
        {
        }

        public static IRemotingRestAdapter CreateRemotingAdapter(IContext context = null)
        {
            context = context ?? new RestContext("loopback-xamarin/1.0");
            return new RestAdapter(context, TestSuite.REMOTING_REST_SERVER_URL);
        }

        public static IRestAdapter CreateAdapter(IContext context = null)
        {
            context = context ?? new RestContext("loopback-xamarin/1.0");
            return new Xamarin.Loopback.Adapters.RestAdapter(context, TestSuite.REST_SERVER_URL,
                new DummyStorageService());
        }
    }
}