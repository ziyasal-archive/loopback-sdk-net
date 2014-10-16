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
            Dictionary<string, object> dictionary = new Dictionary<string, object>
            {
                {"name", "ziyasal"},
                {"age", 26}
            };

            string json = JsonConvert.SerializeObject(dictionary);
            SampleModel deserializeObject = JsonConvert.DeserializeObject<SampleModel>(json);

            Assert.NotNull(deserializeObject);
        }
    }
}

