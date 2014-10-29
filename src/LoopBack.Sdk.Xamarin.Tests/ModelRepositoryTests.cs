using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LoopBack.Sdk.Xamarin.Tests
{
    [TestFixture]
    public class ModelRepositoryTests
    {
        [Test]
        public void Create_class_from_dictionary_test()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"name", "ziyasal"},
                {"age", 26}
            };

            var json = JsonConvert.SerializeObject(dictionary);
            var deserializeObject = JsonConvert.DeserializeObject<SampleModel>(json);

            Assert.NotNull(deserializeObject);
        }
    }
}