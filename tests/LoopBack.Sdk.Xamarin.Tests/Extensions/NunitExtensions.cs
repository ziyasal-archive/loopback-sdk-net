﻿using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LoopBack.Sdk.Xamarin.Tests.Extensions
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