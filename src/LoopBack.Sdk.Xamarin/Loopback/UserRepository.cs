namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class UserRepository<U>:ModelRepository<U> where U:User
    {
		public UserRepository ():base("user",typeof(User))
		{
			
		}
    }
}
