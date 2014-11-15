using System.Collections.Generic;

namespace LoopBack.Sdk.Xamarin.Tests.Remooting
{
    public static class TestUtil
    {
        public static Dictionary<string, T> BuildParameters<T>(string name, T value)
        {
            var parameters = new Dictionary<string, T>
            {
                {name, value}
            };

            return parameters;
        }
    }
}