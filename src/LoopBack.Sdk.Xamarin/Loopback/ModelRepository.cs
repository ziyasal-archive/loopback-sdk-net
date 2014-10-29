using System;

namespace LoopBack.Sdk.Xamarin.Loopback
{
    public class ModelRepository<T> : RestRepository<T> where T : Model
    {
        public ModelRepository(string className) : this(className, null)
        {
        }

        public ModelRepository(string className, Type modelClass) : this(className, null, modelClass)
        {
        }

        public ModelRepository(string className, string nameForRestUrl, Type modelClass)
            : base(className, modelClass ?? typeof (Model))
        {
            //NameForRestUrl = nameForRestUrl ?? English.plural (className);
        }

        public string NameForRestUrl { get; set; }
    }
}