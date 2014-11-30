using System.Collections.Generic;
using FluentAssertions;
using Loopback.Sdk.Xamarin.Extensions;
using Loopback.Sdk.Xamarin.Remoting;
using Loopback.Sdk.Xamarin.Remoting.Adapters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Loopback.Sdk.Xamarin.Test.Remoting
{
    public class RemoteClassTests : TestBase
    {
        private IRemotingRestAdapter _adapter;
        private RemoteClass _contractClass;
        private RemoteRepository<ContractClass> _contractClassRepository;
        private RemoteClass _simpleContractClass;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateRemotingAdapter();

            _simpleContractClass = new RemoteClass();
            _contractClass = new RemoteClass();

            //TODO:
            //_simpleContractClass.SetRemoting("contract", "transform", new RemotingOptions
            //{
            //    Shared = true,
            //    Description = "Takes a string and returns an updated string.",
            //    Accepts = new AcceptOptions
            //    {
            //        Arg = "str",
            //        Type = "string",
            //        Description = "String to transform",
            //        Required = true,
            //        Http = new HttpAcceptOptions { Source = HttpAcceptSource.PATH }
            //    },
            //    Returns = new ReturnsOptions { Arg = "data", Type = "string" },
            //    Http = new HttpOptions { Verb = "GET", Path = "/customizedTransform" }
            //});

            _simpleContractClass.SetRemoting("/contract/customizedGetSecret", "GET", "contract.getSecret");
            _simpleContractClass.SetRemoting("/contract/customizedTransform", "GET", "contract.transform");
            _simpleContractClass.SetRemoting("/contract/geopoint", "GET", "contract.geopoint");
            _simpleContractClass.SetRemoting("/contract/list", "GET", "contract.list");

            _contractClass.SetRemoting("/ContractClass/:name/getName", "POST", "ContractClass.prototype.getName");
            _contractClass.SetRemoting("/ContractClass/:name/greet", "POST", "ContractClass.prototype.greet");

            _contractClassRepository = new RemoteRepository<ContractClass>("ContractClass")
            {
                Adapter = _adapter
            };
        }

        [Test]
        public async void Call_Remote_Class_Method_Test()
        {
            var verb = _simpleContractClass.GetVerbForMethod("contract.getSecret");
            var path = _simpleContractClass.GetUrlForMethod("contract.getSecret");
            var parameterEncoding = _simpleContractClass.GetParameterEncodingForMethod("contract.getSecret");

            var response = await _adapter.InvokeStaticMethod(null, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("shhh!");
        }

        [Test]
        public async void ClassGet_Test()
        {
            var parameters = TestSuite.BuildParameters("name", (object) "somename");

            var verb = _contractClass.GetVerbForMethod("ContractClass.prototype.getName");
            var path = _contractClass.GetUrlForMethod("ContractClass.prototype.getName", parameters);
            var parameterEncoding = _contractClass.GetParameterEncodingForMethod("ContractClass.prototype.getName");

            var response = await _adapter.InvokeStaticMethod(parameters, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("somename");
        }

        [Test]
        public async void ClassTransform_Test()
        {
            var combinedParameters = new Dictionary<string, object>();
            var constructorParameters = TestSuite.BuildParameters("name", (object) "somename");
            var parameters = TestSuite.BuildParameters("other", (object) "othername");

            combinedParameters.AddRange(constructorParameters);
            combinedParameters.AddRange(parameters);

            var verb = _contractClass.GetVerbForMethod("ContractClass.prototype.greet");
            var path = _contractClass.GetUrlForMethod("ContractClass.prototype.greet", combinedParameters);
            var parameterEncoding = _contractClass.GetParameterEncodingForMethod("ContractClass.prototype.greet");

            var response = await _adapter.InvokeInstanceMethod(combinedParameters, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("Hi, othername!");
        }

        [Test]
        public async void Custom_RequestHeader_Test()
        {
            var customAdapter = CreateRemotingAdapter();

            customAdapter.Client.DefaultRequestHeaders.Add("Authorization", "auth-token");

            var test = new RemoteClass();
            test.SetRemoting("/contract/get-auth", "GET", "contract.getAuthorizationHeader");

            var parameters = new Dictionary<string, object>();

            var verb = test.GetVerbForMethod("contract.getAuthorizationHeader");
            var path = test.GetUrlForMethod("contract.getAuthorizationHeader", parameters);
            var parameterEncoding = test.GetParameterEncodingForMethod("contract.getAuthorizationHeader");


            var response = await customAdapter.InvokeStaticMethod(parameters, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            data.Should().NotBeNull();
            data["data"].ToString().ShouldBeEquivalentTo("auth-token");
        }

        [Test]
        public async void Deeply_Nested_Parameter_Objects_Are_Flattened_Test()
        {
            var filter = new Dictionary<string, object>
            {
                {
                    "where", new Dictionary<string, object>
                    {
                        {"age", TestSuite.BuildParameters("gt", (object) 21)}
                    }
                }
            };


            var parameters = TestSuite.BuildParameters("filter", (object) filter);

            var verb = _simpleContractClass.GetVerbForMethod("contract.list");
            var path = _simpleContractClass.GetUrlForMethod("contract.list", parameters);
            var parameterEncoding = _simpleContractClass.GetParameterEncodingForMethod("contract.list");

            var response = await _adapter.InvokeStaticMethod(parameters, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            data.Should().NotBeNull();

            var expected = JsonConvert.SerializeObject(filter);

            data["data"].ToString().ShouldBeEquivalentTo(expected);
        }

        [Test]
        public async void Nested_Parameter_Objects_Are_Flattened_Test()
        {
            var parameters = TestSuite.BuildParameters("here", (object) new Dictionary<string, object>
            {
                {"lat", 10},
                {"lng", 20}
            });

            var verb = _simpleContractClass.GetVerbForMethod("contract.geopoint");
            var path = _simpleContractClass.GetUrlForMethod("contract.geopoint", parameters);
            var parameterEncoding = _simpleContractClass.GetParameterEncodingForMethod("contract.geopoint");

            var response = await _adapter.InvokeStaticMethod(parameters, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            data.Should().NotBeNull();
            data["lat"].ToString().ShouldBeEquivalentTo("10");
            data["lng"].ToString().ShouldBeEquivalentTo("20");
        }

        [Test]
        public async void PrototypeGet_Test()
        {
            RemoteClass contractClass =
                _contractClassRepository.CreateObject(TestSuite.BuildParameters("name", (object) "somename"));

            contractClass.SetRemoting("/ContractClass/:name/getName", "POST", "ContractClass.prototype.getName");

            var response = await contractClass.InvokeMethod("getName", null);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("somename");
        }

        [Test]
        public async void PrototypeStatic_Test()
        {
            var response = await _contractClassRepository.InvokeStaticMethod("getFavoritePerson", null);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("You");
        }

        [Test]
        public async void PrototypeTransform_Test()
        {
            RemoteClass test =
                _contractClassRepository.CreateObject(TestSuite.BuildParameters("name", (object) "somename"));

            var response = await test.InvokeMethod("greet", TestSuite.BuildParameters("other", (object) "othername"));

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("Hi, othername!");
        }

        [Test]
        public async void Transform_Test()
        {
            var parameters = TestSuite.BuildParameters("str", (object) "somevalue");

            var path = _simpleContractClass.GetUrlForMethod("contract.transform", parameters);
            var verb = _simpleContractClass.GetVerbForMethod("contract.transform");
            var parameterEncoding = _simpleContractClass.GetParameterEncodingForMethod("contract.transform");

            var response = await _adapter.InvokeStaticMethod(parameters, path, verb, parameterEncoding);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("transformed: somevalue");
        }

        [Test]
        public void RemotingOptionsTest()
        {
            var contract = new RemoteClass();

            contract.SetRemoting("contract", "transform", new RemotingOptions
            {
                Shared = true,
                Description = "Takes a string and returns an updated string.",
                Accepts = new AcceptOptions
                {
                    Arg = "str",
                    Type = "string",
                    Description = "String to transform",
                    Required = true,
                    Http = new HttpAcceptOptions {Source = HttpAcceptSource.PATH}
                },
                Returns = new ReturnsOptions {Arg = "data", Type = "string"},
                Http = new HttpOptions {Verb = "GET", Path = "/customizedTransform"}
            });

            var urlForMethod = contract.GetUrlForMethod("transform");
        }
    }
}