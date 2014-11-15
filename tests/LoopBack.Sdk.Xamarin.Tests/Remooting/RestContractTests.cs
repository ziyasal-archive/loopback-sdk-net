using System.Collections.Generic;
using FluentAssertions;
using LoopBack.Sdk.Xamarin.Common;
using LoopBack.Sdk.Xamarin.Remooting;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LoopBack.Sdk.Xamarin.Tests.Remooting
{
    public class RestContractTests : TestBase
    {
        public string REST_SERVER_URL = "http://localhost:3001";
        private RestAdapter _adapter;
        private RemoteRepository<ContractClass> _testClass;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateAdapter();

            var contract = _adapter.Contract;

            contract.AddItem(new RestContractItem("/contract/customizedGetSecret", "GET"), "contract.getSecret");
            contract.AddItem(new RestContractItem("/contract/customizedTransform", "GET"), "contract.transform");
            contract.AddItem(new RestContractItem("/contract/geopoint", "GET"), "contract.geopoint");
            contract.AddItem(new RestContractItem("/contract/list", "GET"), "contract.list");
            contract.AddItem(new RestContractItem("/ContractClass/:name/getName", "POST"),
                "ContractClass.prototype.getName");
            contract.AddItem(new RestContractItem("/ContractClass/:name/greet", "POST"), "ContractClass.prototype.greet");
            contract.AddItem(new RestContractItem("/contract/binary", "GET"), "contract.binary");

            _testClass = new RemoteRepository<ContractClass>("ContractClass")
            {
                Adapter = _adapter
            };
        }

        private RestAdapter CreateAdapter(IContext context = null)
        {
            return new RestAdapter(context, REST_SERVER_URL);
        }

        [Test]
        public void AddItemsFromContract_Test()
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
        public void BinaryResponseBody()
        {
            _adapter.InvokeStaticMethod("contract.binary", null, (bytes, contentType) =>
            {
                //TODO: Fix
                //contentType.ShouldBeEquivalentTo("application/octet-stream");

                // The values are hard-coded in test-server/contract.js
                Assert.Equals(new byte[] {1, 2, 3}, bytes);
            }, exception => { });
        }

        [Test]
        public void CustomRequestHeader_Test()
        {
            var customAdapter = CreateAdapter(new DummyContextImpl("loopback-xamarin/1.0"));

            customAdapter.Client.DefaultRequestHeaders.Add("Authorization", "auth-token");

            customAdapter.Contract.AddItem(new RestContractItem("/contract/get-auth", "GET"),
                "contract.getAuthorizationHeader");
            customAdapter.InvokeStaticMethod("contract.getAuthorizationHeader", new Dictionary<string, object>(),
                response =>
                {
                    var data = JObject.Parse(response);
                    data.Should().NotBeNull();
                    data["data"].ToString().ShouldBeEquivalentTo("auth-token");
                }, exception => { });
        }

        [Test]
        public void DeeplyNestedParameterObjectsAreFlattened_Test()
        {
            // In this test, we do not check for the exact value of query-string,
            // but ensure that the value created by the android library
            // is correctly parsed by the strong-remoting server.
            // This way the test stays relevant (and passing) even if
            // the query-string format changes in the future.

            var filter = new Dictionary<string, object>
            {
                {
                    "where", new Dictionary<string, object>
                    {
                        {
                            "age", new Dictionary<string, object>
                            {
                                {"gt", 21}
                            }
                        }
                    }
                }
            };
            _adapter.InvokeStaticMethod("contract.list", TestUtil.BuildParameters("filter", (object) filter),
                response =>
                {
                    var data = JObject.Parse(response);
                    data.Should().NotBeNull();
                    data["data"].ToString().ShouldBeEquivalentTo("{\"where\":{\"age\":{\"gt\":21}}}");
                }, exception => { });
        }

        [Test]
        public void Get_Test()
        {
            _adapter.InvokeStaticMethod("contract.getSecret", null, response =>
            {
                var data = JObject.Parse(response);
                var token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("shhh!");
            }, exception => { });
        }

        [Test]
        public void NestedParameterObjectsAreFlattened_Test()
        {
            var parameters = new Dictionary<string, object>
            {
                {"lat", 10},
                {"lng", 20}
            };
            _adapter.InvokeStaticMethod("contract.geopoint", TestUtil.BuildParameters("here", (object) parameters),
                response =>
                {
                    var data = JObject.Parse(response);
                    data.Should().NotBeNull();
                    data["lat"].ToString().ShouldBeEquivalentTo("10");
                    data["lng"].ToString().ShouldBeEquivalentTo("20");
                }, exception => { });
        }

        [Test]
        public void PrototypeGet_Test()
        {
            RemoteClass test = _testClass.CreateObject(TestUtil.BuildParameters("name", (object) "somename"));
            test.InvokeMethod("getName", null, response =>
            {
                var data = JObject.Parse(response);
                var token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("somename");
            }, exception => { });
        }

        [Test]
        public void PrototypeStatic_Test()
        {
            _testClass.InvokeStaticMethod("getFavoritePerson", null,
                response =>
                {
                    var data = JObject.Parse(response);
                    var token = data["data"];
                    token.Should().NotBeNull();
                    token.ToString().ShouldBeEquivalentTo("You");
                }, exception => { });
        }

        [Test]
        public void PrototypeTransform_Test()
        {
            RemoteClass test = _testClass.CreateObject(TestUtil.BuildParameters("name", (object) "somename"));
            test.InvokeMethod("greet", TestUtil.BuildParameters("other", (object) "othername"), response =>
            {
                var data = JObject.Parse(response);
                var token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldBeEquivalentTo("Hi, othername!");
            }, exception => { });
        }

        [Test]
        public void TestClassGet_Test()
        {
            _adapter.InvokeStaticMethod("ContractClass.prototype.getName",
                TestUtil.BuildParameters("name", (object) "somename"), response =>
                {
                    var data = JObject.Parse(response);
                    var token = data["data"];
                    token.Should().NotBeNull();
                    token.ToString().ShouldBeEquivalentTo("somename");
                }, exception => { });
        }

        [Test]
        public void TestClassTransform_Test()
        {
            _adapter.InvokeInstanceMethod("ContractClass.prototype.greet",
                TestUtil.BuildParameters("name", (object) "somename"),
                TestUtil.BuildParameters("other", (object) "othername"),
                response =>
                {
                    var data = JObject.Parse(response);
                    var token = data["data"];
                    token.Should().NotBeNull();
                    token.ToString().ShouldBeEquivalentTo("Hi, othername!");
                }, exception => { });
        }

        [Test]
        public void Transform_Test()
        {
            _adapter.InvokeStaticMethod("contract.transform", TestUtil.BuildParameters("str", (object) "somevalue"),
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