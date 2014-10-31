using FluentAssertions;
using LoopBack.Sdk.Xamarin.Remooting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LoopBack.Sdk.Xamarin.Tests.Remooting
{
    public class RestAdapterTests : TestBase
    {
        public string REST_SERVER_URL = "http://localhost:3001";

        private Xamarin.Remooting.Adapters.RestAdapter _adapter;
        private Repository<SimpleClass> _testClass;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateAdapter();
            _testClass = new Repository<SimpleClass>("SimpleClass")
            {
                Adapter = _adapter
            };
        }

        private Xamarin.Remooting.Adapters.RestAdapter CreateAdapter()
        {
            return new Xamarin.Remooting.Adapters.RestAdapter(null, REST_SERVER_URL);
        }

        [Test]
        public void Connected_Test()
        {
            bool connected = _adapter.IsConnected();
            connected.Should().BeTrue();
        }

        [Test]
        public void Get_Test()
        {
            _adapter.InvokeStaticMethod("simple.getSecret", null, response =>
           {
               JObject data = JObject.Parse(response);
               JToken token = data["data"];
               token.Should().NotBeNull();
               token.ToString().ShouldBeEquivalentTo("shhh!");
           }, exception =>
           {

           });
        }

        [Test]
        public void Transform_Test()
        {
            _adapter.InvokeStaticMethod("simple.transform", TestUtil.BuildParameters("str", (object)"somevalue"), response =>
           {
               JObject data = JObject.Parse(response);
               JToken token = data["data"];
               token.Should().NotBeNull();
               token.ToString().ShouldBeEquivalentTo("transformed: somevalue");
           }, exception =>
           {

           });
        }

        [Test]
        public void SimpleClassGet_Test()
        {
            _adapter.InvokeInstanceMethod("SimpleClass.prototype.getName", TestUtil.BuildParameters("name", (object)"somename"), null, response =>
            {
                JObject data = JObject.Parse(response);
                JToken token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("somename");
            }, exception =>
            {

            });
        }

        [Test]
        public void SimpleClassTransform_Test()
        {
            _adapter.InvokeInstanceMethod("SimpleClass.prototype.greet",
                TestUtil.BuildParameters("name", (object)"somename"),
                TestUtil.BuildParameters("other", (object)"othername"), response =>
           {
               JObject data = JObject.Parse(response);
               JToken token = data["data"];
               token.Should().NotBeNull();
               token.ToString().ShouldBeEquivalentTo("Hi, othername!");
           }, exception =>
           {

           });
        }

        [Test]
        public void PrototypeStatic_Test()
        {
            _testClass.InvokeStaticMethod("getFavoritePerson", null, response =>
             {
                 JObject data = JObject.Parse(response);
                 JToken token = data["data"];
                 token.Should().NotBeNull();
                 token.ToString().ShouldBeEquivalentTo("You");
             }, exception =>
             {

             });
        }

        [Test]
        public void PrototypeGet_Test()
        {
            VirtualObject test = _testClass.CreateObject(TestUtil.BuildParameters("name", (object)"somename"));
            test.InvokeMethod("getName", null, response =>
            {
                JObject data = JObject.Parse(response);
                JToken token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("somename");
            }, exception =>
            {

            });
        }

        [Test]
        public void PrototypeTransform_Test()
        {
            VirtualObject test = _testClass.CreateObject(TestUtil.BuildParameters("name", (object)TestUtil.BuildParameters("somekey", (object)"somevalue")));
            test.InvokeMethod("greet", TestUtil.BuildParameters("other", (object)"othername"), response =>
             {
                 JObject data = JObject.Parse(response);
                 JToken token = data["data"];
                 token.Should().NotBeNull();
                 token.ToString().ShouldBeEquivalentTo("Hi, othername!");
             }, exception =>
             {

             });
        }
    }
}