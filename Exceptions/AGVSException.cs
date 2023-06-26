using AGVSystemCommonNet6.Alarm;

namespace AGVSystemCommonNet6.Exceptions
{
    public abstract class VMSExceptionAbstract : Exception
    {
        public override string Message { get; }
        public VMSExceptionAbstract()
        {
        }
        public VMSExceptionAbstract(string message)
        {
            Message = message;
        }
        public abstract ALARMS Alarm_Code { get; }
    }
}
