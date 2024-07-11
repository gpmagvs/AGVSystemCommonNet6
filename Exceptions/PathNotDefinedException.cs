using AGVSystemCommonNet6.Alarm;

namespace AGVSystemCommonNet6.Exceptions
{
    public class PathNotDefinedException : VMSExceptionAbstract
    {
        public PathNotDefinedException() : base()
        {

        }
        public PathNotDefinedException(string message) : base(message)
        {
        }

        public override ALARMS Alarm_Code { get; set; } = ALARMS.Path_Not_Exist_In_Route;
    }
}
