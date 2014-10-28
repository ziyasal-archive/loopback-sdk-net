namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    public class RestContractItem
    {
        private readonly string _pattern;
        private readonly string _verb;
        private readonly Adapter.ParameterEncoding _parameterEncoding;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        public RestContractItem(string pattern)
            : this(pattern, "POST")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="verb"></param>
        public RestContractItem(string pattern, string verb)
            : this(pattern, verb, Adapter.ParameterEncoding.JSON)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="verb"></param>
        /// <returns></returns>

        public static RestContractItem CreateMultipart(string pattern, string verb)
        {
            return new RestContractItem(pattern, verb, Adapter.ParameterEncoding.FORM_MULTIPART);
        }

        private RestContractItem(string pattern, string verb, Adapter.ParameterEncoding parameterEncoding)
        {
            _pattern = pattern;
            _verb = verb;
            _parameterEncoding = parameterEncoding;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Pattern
        {
            get { return _pattern; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Verb
        {
            get { return _verb; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Adapter.ParameterEncoding ParameterEncoding
        {
            get { return _parameterEncoding; }
        }
    }
}