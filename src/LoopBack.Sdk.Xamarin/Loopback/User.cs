using System.Collections.Generic;

namespace Loopback.Sdk.Xamarin.Loopback
{
    /// <summary>
    ///     A local representative of a single user instance on the server. Derived from Model,
    ///     the data is immediately accessible locally, but can be saved, destroyed, etc. from the server easily.
    /// </summary>
    public class User : Model
    {
        public User(RestRepository<User> repository, Dictionary<string, object> creationParamerters)
            : base(repository, creationParamerters)
        {
        }

        public User()
            : this(null, null)
        {
        }

        public string Realm { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Status { get; set; }
    }
}