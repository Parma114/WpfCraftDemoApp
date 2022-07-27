using System;
using System.Runtime.Serialization;

namespace WpfCraftDemoApp.Exceptions
{
    [Serializable]
    public class NullOrWhiteSpaceException : Exception
    {
        public NullOrWhiteSpaceException()
        {
        }

        public NullOrWhiteSpaceException(string message) : base(message)
        {
        }

        public NullOrWhiteSpaceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NullOrWhiteSpaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}