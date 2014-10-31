using System;
using System.Collections.Generic;
using FluentAssertions;
using LoopBack.Sdk.Xamarin.Remooting;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LoopBack.Sdk.Xamarin.Tests.Remoot
{
    public class RestAdapterTests : TestBase
    {
        public string REST_SERVER_URL = "http://localhost:3001";

        public static Dictionary<string, T> BuildParameters<T>(string name, T value)
        {
            Dictionary<string, T> parameters = new Dictionary<String, T>
            {
                {name, value}
            };

            return parameters;
        }


        private Remooting.Adapters.RestAdapter _adapter;
        private Repository<SimpleClass> _testClass;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateAdapter();
            _testClass = new Repository<SimpleClass>("SimpleClass")
            {
                Adapter = _adapter
            };
        }

        private Remooting.Adapters.RestAdapter CreateAdapter()
        {
            return new Remooting.Adapters.RestAdapter(null, REST_SERVER_URL);
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
            _adapter.InvokeStaticMethod("simple.transform", BuildParameters("str", (object)"somevalue"), response =>
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
            _adapter.InvokeInstanceMethod("SimpleClass.prototype.getName", BuildParameters("name", (object)"somename"), null, response =>
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
                BuildParameters("name", (object)"somename"),
                BuildParameters("other", (object)"othername"), response =>
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
            VirtualObject test = _testClass.CreateObject(BuildParameters("name", (object)"somename"));
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
            VirtualObject test = _testClass.CreateObject(BuildParameters("name", (object)BuildParameters("somekey", (object)"somevalue")));
            test.InvokeMethod("greet", BuildParameters("other", (object)"othername"), response =>
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