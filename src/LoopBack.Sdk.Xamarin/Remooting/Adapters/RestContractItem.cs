namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    /// <summary>
    ///     A single item within a larger SLRESTContract, encapsulation a single route's verb and pattern, e.g. GET
    ///     /widgets/:id.
    /// </summary>
    public class RestContractItem
    {
        /// <summary>
        ///     Creates a new item encapsulating the given pattern and the default verb,<code>"POST"</code>
        /// </summary>
        /// <param name="pattern">The pattern corresponding to this route, e.g. <code>"/widgest/:id"</code></param>
        public RestContractItem(string pattern)
            : this(pattern, "POST")
        {
        }

        /// <summary>
        ///     Creates a new item encapsulating the given pattern and verb.
        /// </summary>
        /// <param name="pattern">The pattern corresponding to this route, e.g. <code>"/widgets/:id"</code>. </param>
        /// <param name="verb">The verb corresponding to this route, e.g. <code>"GET"</code>.</param>
        public RestContractItem(string pattern, string verb)
            : this(pattern, verb, AdapterBase.ParameterEncoding.JSON)
        {
        }

        private RestContractItem(string pattern, string verb, AdapterBase.ParameterEncoding parameterEncoding)
        {
            Pattern = pattern;
            Verb = verb;
            ParameterEncoding = parameterEncoding;
        }

        /// <summary>
        ///     he pattern corresponding to this route, e.g. <code>"/widgets/:id"</code>.
        /// </summary>
        public virtual string Pattern { get; }

        /// <summary>
        ///     The verb corresponding to this route, e.g. <code>"GET"</code>.
        /// </summary>
        public virtual string Verb { get; }

        /// <summary>
        /// </summary>
        public virtual AdapterBase.ParameterEncoding ParameterEncoding { get; }

        /// <summary>
        ///     Creates a new item encapsulating a route that expects multi-part request (e.g. file upload).
        /// </summary>
        /// <param name="pattern">The pattern corresponding to this route, e.g. <code>"/files/:id"</code>.</param>
        /// <param name="verb">The verb corresponding to this route, e.g. <code>"POST"</code>.</param>
        /// <returns>The <see cref="RestContractItem" /> created.</returns>
        public static RestContractItem CreateMultipart(string pattern, string verb)
        {
            return new RestContractItem(pattern, verb, AdapterBase.ParameterEncoding.FORM_MULTIPART);
        }
    }
}