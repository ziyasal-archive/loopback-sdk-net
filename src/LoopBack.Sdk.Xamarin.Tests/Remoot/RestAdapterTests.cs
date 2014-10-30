using System;
using System.Collections.Generic;
using FluentAssertions;
using LoopBack.Sdk.Xamarin.Remooting;
using LoopBack.Sdk.Xamarin.Remooting.Adapters;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace LoopBack.Sdk.Xamarin.Tests.Remoot
{
    public class RestAdapterTests : TestBase
    {
        public string REST_SERVER_URL = "http://localhost:3001";

        public static Dictionary<string, T> BuildParameters<T>(string name, T value)
        {
            Dictionary<string, T> parameters = new Dictionary<String, T>();
            parameters.Add(name, value);
            return parameters;
        }


        private RestAdapter adapter;
        private Repository<SimpleClass> testClass;

        protected override void FinalizeSetUp()
        {
            adapter = CreateAdapter();
            testClass = new Repository<SimpleClass>("SimpleClass")
            {
                Adapter = adapter
            };
        }

        private RestAdapter CreateAdapter()
        {
            return new RestAdapter(null, REST_SERVER_URL);
        }


        [Test]
        public void Connected_Test()
        {
            bool connected = adapter.IsConnected();
            connected.Should().BeTrue();
        }

        [Test]
        public void Get_Test()
        {
            adapter.InvokeStaticMethod("simple.getSecret", null, response =>
           {
               JObject data = JObject.Parse(response);
               JToken token = data["data"];
               token.Should().NotBeNull();
               token.ToString().ShouldAllBeEquivalentTo("shhh!");
           }, exception =>
           {

           });
        }

        [Test]
        public void Transform_Test()
        {
            adapter.InvokeStaticMethod("simple.transform", BuildParameters("str", (object)"somevalue"), response =>
           {
               JObject data = JObject.Parse(response);
               JToken token = data["data"];
               token.Should().NotBeNull();
               token.ToString().ShouldAllBeEquivalentTo("transformed: undefined");
           }, exception =>
           {

           });
        }

        [Test]
        public void SimpleClassGet_Test()
        {
            //TODO: fix!
            adapter.InvokeInstanceMethod("SimpleClass.prototype.getName", BuildParameters("name", (object)"somename"), null, response =>
            {
                JObject data = JObject.Parse(response);
                JToken token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldAllBeEquivalentTo("somename");
            }, exception =>
            {

            });
        }

        [Test]
        public void SimpleClassTransform_Test()
        {
            adapter.InvokeInstanceMethod("SimpleClass.prototype.greet",
                BuildParameters("name", (object)"somename"),
                BuildParameters("other", (object)"othername"), response =>
           {
               JObject data = JObject.Parse(response);
               JToken token = data["data"];
               token.Should().NotBeNull();
               token.ToString().ShouldAllBeEquivalentTo("Hi, othername!");
           }, exception =>
           {

           });
        }

        [Test]
        public void PrototypeStatic_Test()
        {
            testClass.InvokeStaticMethod("getFavoritePerson", null, response =>
             {
                 JObject data = JObject.Parse(response);
                 JToken token = data["data"];
                 token.Should().NotBeNull();
                 token.ToString().ShouldAllBeEquivalentTo("You");
             }, exception =>
             {

             });
        }

        [Test]
        public void PrototypeGet_Test()
        {
            //TODO: fix!
            VirtualObject test = testClass.CreateObject(BuildParameters("name", (object)"somename"));
            test.InvokeMethod("getName", null, response =>
            {
                JObject data = JObject.Parse(response);
                JToken token = data["data"];
                token.Should().NotBeNull();
                token.ToString().ShouldAllBeEquivalentTo("somename");
            }, exception =>
            {

            });
        }

        [Test]
        public void PrototypeTransform_Test()
        {
            VirtualObject test = testClass.CreateObject(BuildParameters("name", (object)BuildParameters("somekey", (object)"somevalue")));
            test.InvokeMethod("greet", BuildParameters("other", (object)"othername"), response =>
             {
                 JObject data = JObject.Parse(response);
                 JToken token = data["data"];
                 token.Should().NotBeNull();
                 token.ToString().ShouldAllBeEquivalentTo("Hi, othername!");
             }, exception =>
             {

             });
        }
    }
}