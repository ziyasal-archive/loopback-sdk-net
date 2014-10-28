namespace LoopBack.Sdk.Xamarin.Loopback
{
	public class ModelRepository<T>: RestRepository<T> where T:Model
	{
		public ModelRepository () : base(string.Empty)
		{
		}

	    public ModelRepository(string name, string nameForRestUrl, Model modelClass) : base(name)
	    {
	        
	    }
	}
}

