using AGVSystemCommonNet6.Alarm;

namespace AGVSystemCommonNet6.Exceptions
{
    public class LoadButAGVNoCargoException : VMSExceptionAbstract
    {
        public override string Message => "AGV車上沒有貨物時不可執行放貨任務";
        public LoadButAGVNoCargoException() : base()
        {
        }
        public LoadButAGVNoCargoException(string message) : base(message)
        {
        }
        public override ALARMS Alarm_Code { get; set; } = ALARMS.CANNOT_DISPATCH_LOAD_TASK_WHEN_AGV_NO_CARGO;
    }
}
