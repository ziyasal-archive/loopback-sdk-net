using System.Collections.Generic;
using FluentAssertions;
using Loopback.Sdk.Xamarin.Extensions;
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
            var connected = _adapter.IsConnected();
            connected.Should().BeTrue();
        }

        [Test]
        public async void Get_Test()
        {
            var test = new RemoteClass();

            var verb = test.GetVerbForMethod("simple.getSecret");
            var path = test.GetUrlForMethod("simple.getSecret");
            var parameterEncoding = test.GetParameterEncodingForMethod("simple.getSecret");
            var response = await _adapter.InvokeStaticMethod(null, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("shhh!");
        }

        [Test]
        public async void PrototypeGet_Test()
        {
            RemoteClass test =
                _simpleClassRepository.CreateObject(TestSuite.BuildParameters("name", (object) "somename"));

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
                _simpleClassRepository.CreateObject(TestSuite.BuildParameters("name", (object) "somevalue"));

            var response = await test.InvokeMethod("greet", TestSuite.BuildParameters("other", (object) "othername"));

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("Hi, othername!");
        }

        [Test]
        public async void SimpleClassGet_Test()
        {
            var test = new RemoteClass();
            var parameters = TestSuite.BuildParameters("name", (object) "somename");

            var verb = test.GetVerbForMethod("SimpleClass.prototype.getName");
            var path = test.GetUrlForMethod("SimpleClass.prototype.getName", parameters);
            var parameterEncoding = test.GetParameterEncodingForMethod("SimpleClass.prototype.getName");

            var response =
                await
                    _adapter.InvokeInstanceMethod(parameters, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("somename");
        }

        [Test]
        public async void SimpleClassTransform_Test()
        {
            var test = new RemoteClass();

            var combinedParameters = new Dictionary<string, object>();
            var constructorParameters = TestSuite.BuildParameters("name", (object) "somename");
            var parameters = TestSuite.BuildParameters("other", (object) "othername");

            combinedParameters.AddRange(constructorParameters);
            combinedParameters.AddRange(parameters);

            var verb = test.GetVerbForMethod("SimpleClass.prototype.greet");
            var path = test.GetUrlForMethod("SimpleClass.prototype.greet", combinedParameters);
            var parameterEncoding = test.GetParameterEncodingForMethod("SimpleClass.prototype.greet");

            var response =
                await
                    _adapter.InvokeInstanceMethod(combinedParameters, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("Hi, othername!");
        }

        [Test]
        public async void Transform_Test()
        {
            var test = new RemoteClass();
            var parameters = TestSuite.BuildParameters("str", (object) "somevalue");

            var verb = test.GetVerbForMethod("simple.transform");
            var path = test.GetUrlForMethod("simple.transform", parameters);
            var parameterEncoding = test.GetParameterEncodingForMethod("simple.getSecret");

            var response = await _adapter.InvokeStaticMethod(parameters, path, verb, parameterEncoding);


            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("transformed: somevalue");
        }
    }
}