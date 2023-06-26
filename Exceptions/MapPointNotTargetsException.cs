using AGVSystemCommonNet6.Alarm;

namespace AGVSystemCommonNet6.Exceptions
{


    public class MapPointNotTargetsException : VMSExceptionAbstract
    {
        public MapPointNotTargetsException() : base()
        {
        }

        public MapPointNotTargetsException(string message) : base(message)
        {
        }

        public override ALARMS Alarm_Code => ALARMS.MAP_POINT_NO_TARGETS;
    }
}
