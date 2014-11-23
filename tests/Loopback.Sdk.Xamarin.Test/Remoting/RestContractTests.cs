using System.Collections.Generic;
using FluentAssertions;
using Loopback.Sdk.Xamarin.Remoting;
using Loopback.Sdk.Xamarin.Remoting.Adapters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Loopback.Sdk.Xamarin.Test.Remoting
{
    public class RestContractTests : TestBase
    {
        private IRemotingRestAdapter _adapter;
        private RemoteRepository<ContractClass> _contractClassRepository;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateRemotingAdapter();

            var contract = _adapter.Contract;

            contract.AddItem(new RestContractItem("/contract/customizedGetSecret", "GET"), "contract.getSecret");
            contract.AddItem(new RestContractItem("/contract/customizedTransform", "GET"), "contract.transform");
            contract.AddItem(new RestContractItem("/contract/geopoint", "GET"), "contract.geopoint");
            contract.AddItem(new RestContractItem("/contract/list", "GET"), "contract.list");
            contract.AddItem(new RestContractItem("/ContractClass/:name/getName", "POST"),
                "ContractClass.prototype.getName");

            contract.AddItem(new RestContractItem("/ContractClass/:name/greet", "POST"), "ContractClass.prototype.greet");
            contract.AddItem(new RestContractItem("/contract/binary", "GET"), "contract.binary");

            _contractClassRepository = new RemoteRepository<ContractClass>("ContractClass")
            {
                Adapter = _adapter
            };
        }

        [Test]
        public void Add_Items_From_Contract_Test()
        {
            var parent = new RestContract();
            var child = new RestContract();

            parent.AddItem(new RestContractItem("/wrong/route", "OOPS"), "wrong.route");
            child.AddItem(new RestContractItem("/test/route", "GET"), "test.route");
            child.AddItem(new RestContractItem("/new/route", "POST"), "new.route");

            parent.AddItemsFromContract(child);

            string testRoute = parent.GetUrlForMethod("test.route");
            string testRouteVerb = parent.GetVerbForMethod("test.route");
            string newRoute = parent.GetUrlForMethod("new.route");
            string newRouteVerb = parent.GetVerbForMethod("new.route");

            testRoute.ShouldBeEquivalentTo("/test/route");
            testRouteVerb.ShouldBeEquivalentTo("GET");

            newRoute.ShouldBeEquivalentTo("/new/route");
            newRouteVerb.ShouldBeEquivalentTo("POST");
        }

        [Test]
        public async void Call_Remote_Class_Method_Test()
        {
            var response = await
                _adapter.InvokeStaticMethod("contract.getSecret", null);

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("shhh!");
        }

        [Test]
        public async void ClassGet_Test()
        {
            var response = await
                _adapter.InvokeStaticMethod("ContractClass.prototype.getName",
                    TestSuite.BuildParameters("name", (object) "somename"));

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("somename");
        }

        [Test]
        public async void ClassTransform_Test()
        {
            var response = await
                _adapter.InvokeInstanceMethod("ContractClass.prototype.greet",
                    TestSuite.BuildParameters("name", (object) "somename"),
                    TestSuite.BuildParameters("other", (object) "othername"));

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

            customAdapter.Contract.AddItem(new RestContractItem("/contract/get-auth", "GET"),
                "contract.getAuthorizationHeader");

            var response = await
                customAdapter.InvokeStaticMethod("contract.getAuthorizationHeader", new Dictionary<string, object>());

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

            var response = await
                _adapter.InvokeStaticMethod("contract.list", TestSuite.BuildParameters("filter", (object) filter));

            var data = JObject.Parse(response.Content);
            data.Should().NotBeNull();

            var expected = JsonConvert.SerializeObject(filter);

            data["data"].ToString().ShouldBeEquivalentTo(expected);
        }

        [Test]
        public async void Nested_Parameter_Objects_Are_Flattened_Test()
        {
            var parameters = new Dictionary<string, object>
            {
                {"lat", 10},
                {"lng", 20}
            };

            var response = await
                _adapter.InvokeStaticMethod("contract.geopoint", TestSuite.BuildParameters("here", (object) parameters));

            var data = JObject.Parse(response.Content);
            data.Should().NotBeNull();
            data["lat"].ToString().ShouldBeEquivalentTo("10");
            data["lng"].ToString().ShouldBeEquivalentTo("20");
        }

        [Test]
        public async void PrototypeGet_Test()
        {
            RemoteClass test =
                _contractClassRepository.CreateObject(TestSuite.BuildParameters("name", (object) "somename"));

            var response = await test.InvokeMethod("getName", null);

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
            var response =
                await
                    _adapter.InvokeStaticMethod("contract.transform",
                        TestSuite.BuildParameters("str", (object) "somevalue"));

            var data = JObject.Parse(response.Content);
            var token = data["data"];
            token.Should().NotBeNull();
            token.ToString().ShouldBeEquivalentTo("transformed: somevalue");
        }
    }
}