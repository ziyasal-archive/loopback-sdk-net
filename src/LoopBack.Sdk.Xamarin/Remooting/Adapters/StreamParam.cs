using System;
using System.Collections.Generic;
using System.IO;

namespace LoopBack.Sdk.Xamarin.Remooting.Adapters
{
    public class StreamParam
    {
        private readonly Stream _stream;
        private readonly string _fileName;
        private readonly string _contentType;

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

        public virtual void PutTo(Dictionary<string,object> requestParameters, string key)
        {
            throw new NotImplementedException();
        }
    }
}
