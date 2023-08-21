using AGVSystemCommonNet6.Alarm;

namespace AGVSystemCommonNet6.Exceptions
{
    public class NoPathForNavigatorException : VMSExceptionAbstract
    {
        public NoPathForNavigatorException() : base()
        {

        }
        public NoPathForNavigatorException(string message) : base(message)
        {
        }

        public override ALARMS Alarm_Code { get; set; } = ALARMS.TRAFFIC_BLOCKED_NO_PATH_FOR_NAVIGATOR;
    }
}
