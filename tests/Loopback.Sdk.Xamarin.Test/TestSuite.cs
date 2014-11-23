using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using ModernHttpClient;
using Newtonsoft.Json;

namespace Loopback.Sdk.Xamarin.Test
{
    public static class TestSuite
    {
        public static string REMOTING_REST_SERVER_URL = "http://localhost:3001";
        public static string REST_SERVER_URL = "http://localhost:3838/api";

        private static readonly Dictionary<string, string> RemoteModelDefinitionCache =
            new Dictionary<string, string>();

        static TestSuite()
        {
            InitModelDefinitionCache();
        }

        private static void InitModelDefinitionCache()
        {
            const string setupFn = @"function(app, cb) {
                                 app.models.MyModel.create({ name: 'My Test Model' },
                                  function(err, mymodel) {
                                    if (err) return cb(err);
                                    cb(null, { mymodel: mymodel });
                                  });
                                }";

            var myModelDef = new KeyValuePair<string, object>("MyModel", new
            {
                properties = new {name = "string", required = true}
            });

            var myModelPayloadWithSetupFn = new
            {
                models = new Dictionary<string, object>
                {
                    {myModelDef.Key, myModelDef.Value},
                    {"setupFn", setupFn}
                }
            };

            var myModelPayload = new
            {
                models = new Dictionary<string, object>
                {
                    {myModelDef.Key, myModelDef.Value}
                }
            };

            RemoteModelDefinitionCache["MyModel"] = JsonConvert.SerializeObject(myModelPayload);
            RemoteModelDefinitionCache["MyModelWithSetupFn"] = JsonConvert.SerializeObject(myModelPayloadWithSetupFn);
        }

        public static Dictionary<string, T> BuildParameters<T>(string name, T value)
        {
            var parameters = new Dictionary<string, T>
            {
                {name, value}
            };

            return parameters;
        }

        public static string GetModelDefinitionJson(string modelName)
        {
            if (!string.IsNullOrEmpty(modelName))
                return RemoteModelDefinitionCache[modelName];

            throw new Exception("Model definition not found!");
        }

        public static bool SetupFor(string model)
        {
            var json = GetModelDefinitionJson(model);


            var client = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = new Uri(REST_SERVER_URL)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Post, "setup")
            {
                Content = new StringContent(json)
            };

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httpResponseMessage = client.SendAsync(request).Result;

            return httpResponseMessage.IsSuccessStatusCode;
        }
    }
}