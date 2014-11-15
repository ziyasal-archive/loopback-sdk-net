using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace LoopBack.Sdk.Xamarin.Common
{
    public static class DictionaryExtensions
    {
        //public static Dictionary<string, object> ToDictionaryFromJson(this string json)
        //{
        //    var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        //    var values2 = new Dictionary<string, object>();
        //    foreach (KeyValuePair<string, object> d in values)
        //    {
        //        if (d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject"))
        //        {
        //            values2.Add(d.Key, ToDictionaryFromJson(d.Value.ToString()));
        //        }
        //        else
        //        {
        //            values2.Add(d.Key, d.Value);
        //        }
        //    }
        //    return values2;
        //}

        public static Dictionary<string, object> ToDictionaryFromJson(this string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public static T ToObject<T>(this Dictionary<string, object> source, T instance) where T : class
        {
            var someObjectType = instance.GetType();

            foreach (var item in source)
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

        private static void AddPropertyToDictionary<T>(PropertyInfo property, object source,
            Dictionary<string, T> dictionary)
        {
            var value = property.GetValue(source, null);
            if (IsOfType<T>(value))
            {
                dictionary.Add(property.Name, (T) value);
            }
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }
    }
}