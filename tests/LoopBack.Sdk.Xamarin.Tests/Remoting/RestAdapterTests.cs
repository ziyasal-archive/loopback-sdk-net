using FluentAssertions;
using LoopBack.Sdk.Xamarin.Remoting;
using LoopBack.Sdk.Xamarin.Remoting.Adapters;
using LoopBack.Sdk.Xamarin.Shared;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LoopBack.Sdk.Xamarin.Tests.Remoting
{
    public class RestAdapterTests : TestBase
    {
        public string REST_SERVER_URL = "http://localhost:3001";
        private RestAdapter _adapter;
        private RemoteRepository<SimpleClass> _testClass;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateAdapter(new RestContext("loopback-xamarin/1.0"));
            _testClass = new RemoteRepository<SimpleClass>("SimpleClass")
            {
                Adapter = _adapter
            };
        }

        private RestAdapter CreateAdapter(IContext context)
        {
            return new RestAdapter(context, REST_SERVER_URL);
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
           await _adapter.InvokeStaticMethod("simple.getSecret", null, response =>
            {
                var data = JObject.Parse(response);
                var token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("shhh!");
            }, exception => { });
        }

        [Test]
        public async void PrototypeGet_Test()
        {
            RemoteClass test = _testClass.CreateObject(TestUtil.BuildParameters("name", (object)"somename"));
            await test.InvokeMethod("getName", null, response =>
            {
                var data = JObject.Parse(response);
                var token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("somename");
            }, exception => { });
        }

        [Test]
        public async void PrototypeStatic_Test()
        {
            await _testClass.InvokeStaticMethod("getFavoritePerson", null, response =>
            {
                var data = JObject.Parse(response);
                var token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("You");
            }, exception => { });
        }

        [Test]
        public async void PrototypeTransform_Test()
        {
            RemoteClass test =_testClass.CreateObject(TestUtil.BuildParameters("name",
                    (object)TestUtil.BuildParameters("somekey", (object)"somevalue")));
            await test.InvokeMethod("greet", TestUtil.BuildParameters("other", (object)"othername"), response =>
           {
               var data = JObject.Parse(response);
               var token = data["data"];
               token.Should().NotBeNull();
               token.ToString().ShouldBeEquivalentTo("Hi, othername!");
           }, exception => { });
        }

        [Test]
        public async void SimpleClassGet_Test()
        {
            await _adapter.InvokeInstanceMethod("SimpleClass.prototype.getName",
                TestUtil.BuildParameters("name", (object)"somename"), null, response =>
                {
                    var data = JObject.Parse(response);
                    var token = data["data"];
                    token.Should().NotBeNull();
                    token.ToString().ShouldBeEquivalentTo("somename");
                }, exception => { });
        }

        [Test]
        public async void SimpleClassTransform_Test()
        {
            await _adapter.InvokeInstanceMethod("SimpleClass.prototype.greet",
                TestUtil.BuildParameters("name", (object)"somename"),
                TestUtil.BuildParameters("other", (object)"othername"), response =>
                {
                    var data = JObject.Parse(response);
                    var token = data["data"];
                    token.Should().NotBeNull();
                    token.ToString().ShouldBeEquivalentTo("Hi, othername!");
                }, exception => { });
        }

        [Test]
        public async void Transform_Test()
        {
           await _adapter.InvokeStaticMethod("simple.transform", TestUtil.BuildParameters("str", (object)"somevalue"),
                response =>
                {
                    var data = JObject.Parse(response);
                    var token = data["data"];
                    token.Should().NotBeNull();
                    token.ToString().ShouldBeEquivalentTo("transformed: somevalue");
                }, exception => { });
        }
    }
}