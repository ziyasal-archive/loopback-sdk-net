namespace Loopback.Sdk.Xamarin.Loopback
{
    public class LoginResponse<T> : RestResponse<AccessToken> where T : User
    {
        public T User { get; set; }
    }
}