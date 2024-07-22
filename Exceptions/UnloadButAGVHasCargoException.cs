using AGVSystemCommonNet6.Alarm;

namespace AGVSystemCommonNet6.Exceptions
{
    public class UnloadButAGVHasCargoException : VMSExceptionAbstract
    {
        public override string Message => "AGV車上有貨物或有帳籍資料時不可執行取貨任務";
        public UnloadButAGVHasCargoException() : base()
        {
        }
        public UnloadButAGVHasCargoException(string message) : base(message)
        {
        }
        public override ALARMS Alarm_Code { get; set; } = ALARMS.CANNOT_DISPATCH_UNLOAD_TASK_WHEN_AGV_HAS_CARGO;
    }
}
