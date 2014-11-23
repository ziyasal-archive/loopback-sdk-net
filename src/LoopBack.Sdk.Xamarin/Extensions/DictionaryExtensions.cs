using System;
using System.Collections.Generic;
using System.Reflection;
using Loopback.Sdk.Xamarin.Remoting;
using Newtonsoft.Json;

namespace Loopback.Sdk.Xamarin.Extensions
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

        public static T ToObject<T>(this Dictionary<string, object> source, T instance) where T : RemoteClass
        {
            var objectType = instance.GetType();

            foreach (var item in source)
            {
                var runtimeProperty = objectType.GetRuntimeProperty(item.Key);
                if (runtimeProperty != null)
                {
                    runtimeProperty.SetValue(instance, item.Value, null);
                }
                else
                {
                    AddPropertyToObjectWithValue(item, instance);
                }
            }

            return instance;
        }

        private static void AddPropertyToObjectWithValue(KeyValuePair<string, object> propertyPair, RemoteClass instance)
        {
            //HACK: Reflection emit not available in PCL
            //TODO:
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
                var ignoreAttribute = property.GetCustomAttribute<JsonIgnoreAttribute>();
                if (ignoreAttribute == null)
                {
                    AddPropertyToDictionary(property, source, dictionary);
                }
            }

            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyInfo property, object source,
            Dictionary<string, T> dictionary)
        {
            var value = property.GetValue(source, null);
            if (IsOfType<T>(value))
            {
                var propertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                var propertyName = property.Name;

                if (propertyAttribute != null)
                {
                    propertyName = propertyAttribute.PropertyName;
                }

                dictionary.Add(propertyName, (T) value);
            }
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }
    }
}