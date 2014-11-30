using Newtonsoft.Json;

namespace Loopback.Sdk.Xamarin.Remoting
{
    public class RemotingOptions
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("accepts")]
        public AcceptOptions Accepts { get; set; }

        [JsonProperty("returns")]
        public ReturnsOptions Returns { get; set; }

        [JsonProperty("http")]
        public HttpOptions Http { get; set; }

        [JsonProperty("rest")]
        public RestOptions Rest { get; set; }

        [JsonProperty("shared")]
        public bool Shared { get; set; }
    }

    public class AcceptOptions
    {
        [JsonProperty("arg")]
        public string Arg { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("http")]
        public HttpAcceptOptions Http { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }
    }

    public enum HttpAcceptSource
    {
        [JsonProperty("body")] BODY,

        [JsonProperty("path")] PATH
    }

    public class HttpAcceptOptions
    {
        [JsonProperty("source")]
        public HttpAcceptSource Source { get; set; }
    }

    public class ReturnsOptions
    {
        [JsonProperty("arg")]
        public string Arg { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("root")]
        public bool Root { get; set; }
    }

    public class HttpOptions
    {
        [JsonProperty("verb")]
        public string Verb { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }

    public class RestOptions
    {
        [JsonProperty("after")]
        public string After { get; set; }

        [JsonProperty("before")]
        public string Before { get; set; }
    }
}