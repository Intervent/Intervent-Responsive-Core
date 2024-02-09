using System.Runtime.Serialization;

namespace Intervent.Framework.Exceptions.Claims
{
    [Serializable]
    public class AutomateClaimException : Exception
    {
        public AutomateClaimException()
            : base()
        {

        }

        protected AutomateClaimException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public AutomateClaimException(string message)
            : base(message)
        {

        }

        public AutomateClaimException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
