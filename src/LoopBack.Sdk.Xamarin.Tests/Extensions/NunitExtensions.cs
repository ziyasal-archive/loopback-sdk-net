using System;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LoopBack.Sdk.Xamarin.Tests
{
	public static class NunitExtensions
	{
		public static async Task ThrowsAsync<TException>(Func<Task> func) where TException : Exception
		{
			var expected = typeof (TException);
			Type actual = null;

			try
			{
				await func();
			}
			catch (TException exception)
			{
				actual = exception.GetType();
			}

			Assert.AreEqual(expected, actual);
		}
	}
}