using FluentAssertions;
using Loopback.Sdk.Xamarin.Remoting;
using Loopback.Sdk.Xamarin.Remoting.Adapters;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Loopback.Sdk.Xamarin.Test.Remoting
{
    public class RestAdapterTests : TestBase
    {
        private IRemotingRestAdapter _adapter;
        private RemoteRepository<SimpleClass> _simpleClassRepository;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateRemotingAdapter();
            _simpleClassRepository = new RemoteRepository<SimpleClass>("SimpleClass")
            {
                Adapter = _adapter
            };
        }

        [Test]
        public void Connected_Test()
        {
            bool connected = _adapter.IsConnected();
            connected.Should().BeTrue();
        }

        [Test]
        public async void Get_Test()
        {
            var response = await _adapter.InvokeStaticMethod("simple.getSecret", null);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("shhh!");
        }

        [Test]
        public async void PrototypeGet_Test()
        {
            RemoteClass test =
                _simpleClassRepository.CreateObject(TestSuite.BuildParameters("name", (object)"somename"));

            var response = await test.InvokeMethod("getName", null);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("somename");
        }

        [Test]
        public async void PrototypeStatic_Test()
        {
            var response = await _simpleClassRepository.InvokeStaticMethod("getFavoritePerson", null);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("You");
        }

        [Test]
        public async void PrototypeTransform_Test()
        {
            RemoteClass test =
                _simpleClassRepository.CreateObject(TestSuite.BuildParameters("name", (object)"somevalue"));

            var response = await test.InvokeMethod("greet", TestSuite.BuildParameters("other", (object)"othername"));

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("Hi, othername!");
        }

        [Test]
        public async void SimpleClassGet_Test()
        {
            var response =
                await
                    _adapter.InvokeInstanceMethod("SimpleClass.prototype.getName",
                        TestSuite.BuildParameters("name", (object)"somename"), null);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("somename");
        }

        [Test]
        public async void SimpleClassTransform_Test()
        {
            var response =
                await
                    _adapter.InvokeInstanceMethod("SimpleClass.prototype.greet",
                        TestSuite.BuildParameters("name", (object)"somename"),
                        TestSuite.BuildParameters("other", (object)"othername"));

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("Hi, othername!");
        }

        [Test]
        public async void Transform_Test()
        {
            var response = await
                _adapter.InvokeStaticMethod("simple.transform", TestSuite.BuildParameters("str", (object)"somevalue"));

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("transformed: somevalue");
        }
    }
}