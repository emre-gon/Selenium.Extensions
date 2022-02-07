using System;
using System.Runtime.Serialization;

namespace Selenium.Extensions
{
    [Serializable]
    public class DriverCreationException : Exception
    {
        private Exception exc;

        public DriverCreationException()
        {
        }

        public DriverCreationException(Exception exc)
        {
            this.exc = exc;
        }

        public DriverCreationException(string message) : base(message)
        {
        }

        public DriverCreationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DriverCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}