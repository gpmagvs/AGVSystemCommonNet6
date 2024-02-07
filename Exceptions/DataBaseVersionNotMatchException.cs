using System.Runtime.Serialization;

namespace AGVSystemCommonNet6.Exceptions
{
    [Serializable]
    internal class DataBaseVersionNotMatchException : Exception
    {
        public DataBaseVersionNotMatchException()
        {
        }

        public DataBaseVersionNotMatchException(string? message) : base(message)
        {
        }

        public DataBaseVersionNotMatchException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DataBaseVersionNotMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}