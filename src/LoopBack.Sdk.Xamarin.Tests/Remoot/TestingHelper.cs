using System.Collections.Generic;

namespace LoopBack.Sdk.Xamarin.Tests.Remoot
{
    public static class TestingHelper
    {
        public static Dictionary<string, T> BuildParameters<T>(string name, T value)
        {
            Dictionary<string, T> parameters = new Dictionary<string, T>
            {
                {name, value}
            };

            return parameters;
        }
    }
}