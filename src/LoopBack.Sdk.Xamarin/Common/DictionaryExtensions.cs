using System;
using System.Collections.Generic;
using System.Reflection;

namespace LoopBack.Sdk.Xamarin.Common
{
    public static class DictionaryExtensions
    {
        public static T ToObject<T>(this Dictionary<string, object> source, T instance) where T : class
        {
            Type someObjectType = instance.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
                someObjectType.GetRuntimeProperty(item.Key).SetValue(instance, item.Value, null);
            }

            return instance;
        }

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (var element in source)
                target.Add(element);
        }

        public static Dictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static Dictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source",
                    "Unable to convert object to a dictionary. The source object is null.");
            }

            var dictionary = new Dictionary<string, T>();
            foreach (var property in source.GetType().GetRuntimeProperties())
            {
                AddPropertyToDictionary(property, source, dictionary);
            }

            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyInfo property, object source, Dictionary<string, T> dictionary)
        {
            var value = property.GetValue(source, null);
            if (IsOfType<T>(value))
            {
                dictionary.Add(property.Name, (T)value);
            }
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }
    }
}