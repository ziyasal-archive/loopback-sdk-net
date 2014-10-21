using System;
using Moq.Language.Flow;
using System.Threading.Tasks;
using Moq.Language;

namespace LoopBack.Sdk.Xamarin.Tests
{
	public static class MoqExtensions
	{
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(
			this IReturns<TMock, Task<TResult>> setup, TResult value)
			where TMock : class
		{
			return setup.Returns(Task.FromResult(value));
		}
	}
}