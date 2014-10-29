using LoopBack.Sdk.Xamarin.Remooting;
using Newtonsoft.Json;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class File : VirtualObject
    {
        public string Name { get; set; }

        public string Url { get; set; }

        [JsonIgnore]
        public Container ContainerRef { get; set; }
    }
}