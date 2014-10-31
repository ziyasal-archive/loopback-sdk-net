using System.Collections.Generic;
using FluentAssertions;
using LoopBack.Sdk.Xamarin.Remooting;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LoopBack.Sdk.Xamarin.Tests.Remoot
{
    public class RestContractTests : TestBase
    {
        public string REST_SERVER_URL = "http://localhost:3001";

        private Remooting.Adapters.RestAdapter _adapter;
        private Repository<ContractClass> _testClass;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateAdapter();

            RestContract contract = _adapter.Contract;

            contract.AddItem(new RestContractItem("/contract/customizedGetSecret", "GET"), "contract.getSecret");
            contract.AddItem(new RestContractItem("/contract/customizedTransform", "GET"), "contract.transform");
            contract.AddItem(new RestContractItem("/contract/geopoint", "GET"), "contract.geopoint");
            contract.AddItem(new RestContractItem("/contract/list", "GET"), "contract.list");
            contract.AddItem(new RestContractItem("/ContractClass/:name/getName", "POST"), "ContractClass.prototype.getName");
            contract.AddItem(new RestContractItem("/ContractClass/:name/greet", "POST"), "ContractClass.prototype.greet");
            contract.AddItem(new RestContractItem("/contract/binary", "GET"), "contract.binary");

            _testClass = new Repository<ContractClass>("ContractClass")
            {
                Adapter = _adapter
            };
        }

        private Remooting.Adapters.RestAdapter CreateAdapter()
        {
            return new Remooting.Adapters.RestAdapter(null, REST_SERVER_URL);
        }

        [Test]
        public void ddItemsFromContract_Test()
        {
            RestContract parent = new RestContract();
            RestContract child = new RestContract();

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
        public void Get_Test()
        {
            _adapter.InvokeStaticMethod("contract.getSecret", null, response =>
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
            _adapter.InvokeStaticMethod("contract.transform", TestingHelper.BuildParameters("str", (object)"somevalue"), response =>
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
        public void TestClassGet_Test()
        {
            _adapter.InvokeStaticMethod("ContractClass.prototype.getName", TestingHelper.BuildParameters("name", (object)"somename"), response =>
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
        public void TestClassTransform_Test()
        {
            _adapter.InvokeInstanceMethod("ContractClass.prototype.greet", 
                TestingHelper.BuildParameters("name", (object)"somename"), 
                TestingHelper.BuildParameters("other",(object)"othername"), 
                response =>
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
            _testClass.InvokeStaticMethod("getFavoritePerson", null, 
                response =>
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
            VirtualObject test = _testClass.CreateObject(TestingHelper.BuildParameters("name", (object)"somename"));
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
            VirtualObject test = _testClass.CreateObject(TestingHelper.BuildParameters("name", (object)"somename"));
            test.InvokeMethod("greet", TestingHelper.BuildParameters("other", (object)"othername"), response =>
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
        public void NestedParameterObjectsAreFlattened_Test()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"lat", 10},
                {"lng", 20}
            };
            _adapter.InvokeStaticMethod("contract.geopoint", TestingHelper.BuildParameters("here", (object)parameters), response =>
            {
                JObject data = JObject.Parse(response);
                JToken token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("somename");
            }, exception =>
            {

            });
        }
    }
}
