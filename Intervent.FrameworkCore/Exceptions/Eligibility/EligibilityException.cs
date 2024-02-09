using System.Runtime.Serialization;

namespace Framework.Exceptions.Eligibility
{
    [Serializable]
    public class EligibilityException : Exception
    {
        public EligibilityException()
            : base()
        {

        }

        protected EligibilityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public EligibilityException(string message)
            : base(message)
        {

        }

        public EligibilityException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
