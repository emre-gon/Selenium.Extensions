using System;
using System.Runtime.Serialization;

namespace Sl.Selenium
{
    [Serializable]
    internal class UserError : Exception
    {
        public UserError()
        {
        }

        public UserError(string message) : base(message)
        {
        }

        public UserError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}