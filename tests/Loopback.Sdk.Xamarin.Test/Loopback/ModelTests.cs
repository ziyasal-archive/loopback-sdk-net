using System;
using System.Collections.Generic;
using FluentAssertions;
using Loopback.Sdk.Xamarin.Extensions;
using Loopback.Sdk.Xamarin.Loopback;
using Loopback.Sdk.Xamarin.Loopback.Adapters;
using Loopback.Sdk.Xamarin.Shared;
using NUnit.Framework;

namespace Loopback.Sdk.Xamarin.Test.Loopback
{
    public class ModelTests : TestBase
    {
        private ModelRepository<Model> _repository;
        private IRestAdapter _adapter;

        protected override void FinalizeSetUp()
        {
            _adapter = CreateAdapter(new RestContext("loopback-xamarin/1.0"));
            _repository = _adapter.CreateRepository("MyModel");
        }

        [Test]
        public async void Create_And_Remove_Test()
        {
            TestSuite.SetupFor("MyModel");

            var parameters = new Dictionary<string, object>
            {
                {"name", "Foobar"}
            };

            var model = _repository.CreateObject(parameters);

            model.Get("name").ToString().ShouldBeEquivalentTo("Foobar");

            var response = await model.Save();

            response.IsSuccessStatusCode.ShouldBeEquivalentTo(true);
            model.Id.Should().NotBeNull();
        }

        [Test]
        public async void FindById_Test()
        {
            TestSuite.SetupFor("MyModel");

            var parameters = new Dictionary<string, object>
            {
                {"name", "Foobar"}
            };
            var model = _repository.CreateObject(parameters);

            var response = await model.Save();
            response.IsSuccessStatusCode.ShouldBeEquivalentTo(true);

            model.Id.Should().NotBeNull();

            var findResponse = await _repository.FindById(model.Id);

            findResponse.IsSuccessStatusCode.ShouldBeEquivalentTo(true);
        }

        [Test]
        public async void Remove_Test()
        {
            TestSuite.SetupFor("MyModel");

            var parameters = new Dictionary<string, object>
            {
                {"name", "Foobar"}
            };
            var model = _repository.CreateObject(parameters);

            var createResponse = await model.Save();

            createResponse.IsSuccessStatusCode.ShouldBeEquivalentTo(true);

            var response=await model.Destroy();

            response.IsSuccessStatusCode.ShouldBeEquivalentTo(true);

            model.Id.Should().BeNull();

            var findResponse =  await _repository.FindById(model.Id);
            findResponse.IsSuccessStatusCode.ShouldBeEquivalentTo(false);
        }
    }
}