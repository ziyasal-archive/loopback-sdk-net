using System.Collections.Generic;
using System.Threading.Tasks;
using Loopback.Sdk.Xamarin.Extensions;
using Loopback.Sdk.Xamarin.Remoting.Adapters;
using Loopback.Sdk.Xamarin.Shared;
using Newtonsoft.Json.Linq;

namespace Loopback.Sdk.Xamarin.Loopback
{
    /// <summary>
    ///     A base class implementing <see cref="ModelRepository{T}" /> for the built-in User type.
    ///     <pre>
    ///         <code>UserRepository{MyUser} userRepo = new UserRepository{MyUser}("user", typeof(MyUser));
    ///  </code>
    ///     </pre>
    ///     Most application are extending the built-in User model and adds new properties
    ///     like address, etc. You should create your own Repository class
    ///     by extending this base class in such case.
    ///     <p>
    ///         <code>
    ///  public class Customer extends User {
    ///   // your custom properties and prototype (instance) methods
    /// }
    ///  </code>
    ///     </p>
    ///     <pre>
    ///         <code>
    ///  public class CustomerRepository: UserRepository{Customer}
    ///  {
    ///      public CustomerRepository():base("customer", null, typeof(Customer))
    ///      {
    ///      }
    ///      // your custom methods
    ///  }
    /// </code>
    ///     </pre>
    /// </summary>
    /// <typeparam name="T"> User implemenentation based on <see cref="User" /></typeparam>
    public class UserRepository<T> : ModelRepository<T> where T : User
    {
        private const string PROPERTY_CURRENT_USER_ID_KEY = "loopback.currentUserId";
        private readonly IStorageService _storageService;
        private AccessTokenRepository _accessTokenRepository;
        private object _currentUserId;
        private bool _isCurrentUserLoaded;

        public UserRepository(string remoteClassName, IStorageService storageService) : base(remoteClassName)
        {
            _storageService = storageService;
        }

        public UserRepository(string remoteClassName, string nameForRestUrl, IStorageService storageService) : base(remoteClassName, nameForRestUrl)
        {
            _storageService = storageService;
        }

        /// <summary>
        ///     Get the cached value of the currently logged in user.
        /// </summary>
        public T CachedCurrentUser { get; private set; }

        /// <summary>
        ///     Id of the currently logged in user. null when there is no user logged in.
        /// </summary>
        public object CurrentUserId
        {
            get
            {
                LoadCurrentUserIdIfNotLoaded();
                return _currentUserId;
            }
            set
            {
                _currentUserId = value;
                CachedCurrentUser = null;
                SaveCurrentUserId();
            }
        }

        /// <summary>
        ///     Creates a <see cref="T" /> user instance given an email and a password.
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <returns>A {T} user instance.</returns>
        public T CreateUser(string email, string password)
        {
            var dictionary = new Dictionary<string, object>
            {
                {"email", email},
                {"password", password}
            };

            var user = CreateObject(dictionary);
            return user;
        }

        /// <summary>
        ///     Creates a <see cref="RestContract" /> representing the user type's custom
        ///     routes. Used to extend an <see cref="AdapterBase" /> to support user. Calls
        ///     super <see cref="ModelRepository{T}" /> createContract first.
        /// </summary>
        /// <returns>A <see cref="RestContract" /> for this model type.</returns>
        public override RestContract CreateContract()
        {
            var contract = base.CreateContract();

            contract.AddItem(new RestContractItem("/" + NameForRestUrl + "/login?include=user", "POST"),
                RemoteClassName + ".login");
            contract.AddItem(new RestContractItem("/" + NameForRestUrl + "/logout", "POST"), RemoteClassName + ".logout");
            return contract;
        }

        /// <summary>
        ///     Creates a new {T} user given the email, password and optional parameters.
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        /// <param name="parameters">optional parameters</param>
        /// <returns>A new {T} user instance.</returns>
        public T CreateUser(string email, string password, Dictionary<string, object> parameters)
        {
            var allParams = new Dictionary<string, object>();
            allParams.AddRange(parameters);
            allParams.Add("email", email);
            allParams.Add("password", password);
            var user = CreateObject(allParams);

            return user;
        }

        /// <summary>
        ///     Login a user given an email and password. Creates a <see cref="AccessToken" /> and {T} user models if successful.
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        public async Task<LoginResponse<T>> LoginUser(string email, string password)
        {
            var result = new LoginResponse<T>();
            var parameters = new Dictionary<string, object>
            {
                {"email", email},
                {"password", password}
            };

            var response = await InvokeStaticMethod("login", parameters);

            if (response.IsSuccessStatusCode)
            {
                var creationParameters = response.Content.ToDictionaryFromJson();

                var token = GetAccessTokenRepository().CreateObject(creationParameters);
                GetRestAdapter().AccessToken = token.Id.ToString();

                var userJson = JObject.Parse(response.Content)["user"];
                var dictionaryFromJson = userJson.ToString().ToDictionaryFromJson();

                var user = userJson != null
                    ? CreateObject(dictionaryFromJson)
                    : null;

                CurrentUserId = token.UserId;
                CachedCurrentUser = user;

                result.Result = response.ReadAs<AccessToken>();
                result.User = user;
            }
            else
            {
                result.Exception = response.Exception;
            }

            return result;
        }

        /// <summary>
        ///     Logs the current user out of the server and removes the access token from the system.
        /// </summary>
        public async Task<RestResponse> Logout()
        {
            var response = await InvokeStaticMethod("logout", null);

            if (response.IsSuccessStatusCode)
            {
                var radapter = GetRestAdapter();
                radapter.ClearAccessToken();
                CurrentUserId = null;
            }

            return (RestResponse)response;
        }

        private AccessTokenRepository GetAccessTokenRepository()
        {
            return _accessTokenRepository ??
                   (_accessTokenRepository = GetRestAdapter().CreateRepository<AccessTokenRepository, AccessToken>());
        }

        private void SaveCurrentUserId()
        {
            _storageService.Save(PROPERTY_CURRENT_USER_ID_KEY, CurrentUserId.ToString());
        }

        private void LoadCurrentUserIdIfNotLoaded()
        {
            if (!_isCurrentUserLoaded)
            {
                CurrentUserId = _storageService.Get<object>(PROPERTY_CURRENT_USER_ID_KEY);
                _isCurrentUserLoaded = true;
            }
        }
    }
}