using AGVSystemCommonNet6.Alarm;

namespace AGVSystemCommonNet6.Exceptions
{
    public class AGVStatusDownException : VMSExceptionAbstract
    {
        public override string Message => "AGV狀態異常(DOWN)";
        public AGVStatusDownException() : base()
        {
        }
        public AGVStatusDownException(string message) : base(message)
        {
        }
        public override ALARMS Alarm_Code { get; set; } = ALARMS.AGV_STATUS_DOWN;
    }
}
