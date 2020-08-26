using System;
using System.Runtime.Serialization;

namespace Nodum.Core
{
    public class NodePinException : Exception
    {
        public NodePinException()
        {
        }

        public NodePinException(string message) : base(message)
        {
        }

        public NodePinException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NodePinException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
