using AGVSystemCommonNet6.Alarm;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AGVSystemCommonNet6.Exceptions
{
    public abstract class VMSExceptionAbstract : Exception
    {
        public override string Message { get; } = "";
        public VMSExceptionAbstract()
        {
        }
        public VMSExceptionAbstract(string message)
        {
            Message = message;
        }
        public abstract ALARMS Alarm_Code { get; set; }
    }

    public class VMSException : VMSExceptionAbstract
    {
        public override ALARMS Alarm_Code { get; set; } = ALARMS.SYSTEM_ERROR;
        public VMSException() : base()
        {

        }
        public VMSException(string message) : base(message)
        {

        }
    }
}
