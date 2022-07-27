using System;
using System.Runtime.Serialization;

namespace WpfCraftDemoApp.Exceptions
{
    [Serializable]
    public class CustomHttpResponseException : Exception
    {
        public CustomHttpResponseException()
        {
        }

        public CustomHttpResponseException(string message) : base(message)
        {
        }

        public CustomHttpResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomHttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}