using System;
using System.Runtime.Serialization;

namespace WpfCraftDemoApp.Exceptions
{
    [Serializable]
    public class SpecialCharacterException : Exception
    {
        public SpecialCharacterException()
        {
        }

        public SpecialCharacterException(string message) : base(message)
        {
        }

        public SpecialCharacterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SpecialCharacterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}