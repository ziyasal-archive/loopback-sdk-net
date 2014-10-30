using System;
using System.Collections.Generic;
using System.IO;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    /// <summary>
    /// A request parameter that is a (binary) stream.
    /// </summary>
    public class StreamParam
    {
        private readonly string _contentType;
        private readonly string _fileName;
        private readonly Stream _stream;

        public StreamParam(Stream stream, string fileName)
            : this(stream, fileName, null)
        {
        }

        public StreamParam(Stream stream, string fileName, string contentType)
        {
            _stream = stream;
            _fileName = fileName;
            _contentType = contentType;
        }

        public virtual void PutTo(Dictionary<string, object> requestParameters, string key)
        {
            //TODO:
            throw new NotImplementedException();
        }
    }
}