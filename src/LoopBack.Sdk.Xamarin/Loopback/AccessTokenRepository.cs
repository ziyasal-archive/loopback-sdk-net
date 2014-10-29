using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopBack.Sdk.Xamarin.Loopback
{
	public class AccessTokenRepository : ModelRepository<AccessToken>
    {
		public AccessTokenRepository ():base("accessToken",typeof(AccessToken))
		{
			
		}
    }
}
