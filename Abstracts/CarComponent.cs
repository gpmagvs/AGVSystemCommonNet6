using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Alarm.VMS_ALARM;
using RosSharp.RosBridgeClient;

namespace AGVSystemCommonNet6.Abstracts
{
    public abstract class CarComponent
    {
        public enum COMPOENT_NAME
        {
            BATTERY, DRIVER, IMU, BARCODE_READER, GUID_SENSOR, CST_READER,
            NAVIGATION,
            SICK
        }
        public enum STATE
        {
            NORMAL,
            WARNING,
            ABNORMAL
        }
        private Message _StateData;
        public DateTime lastUpdateTime { get; set; } = DateTime.MinValue;
        public abstract COMPOENT_NAME component_name { get; }

        public object Data { get; }

        /// <summary>
        /// 異常碼
        /// </summary>
        public Dictionary<AlarmCodes, DateTime> ErrorCodes = new Dictionary<AlarmCodes, DateTime>();
        public Message StateData
        {
            get => _StateData;
            set
            {
                _StateData = value;
                CheckStateDataContent();
                lastUpdateTime = DateTime.Now;
            }
        }
        protected void AddAlarm(AlarmCodes alarm)
        {
            if (ErrorCodes.ContainsKey(alarm))
                ErrorCodes[alarm] = DateTime.Now;
            else
                ErrorCodes.Add(alarm, DateTime.Now);
        }
        protected void RemoveAlarm(AlarmCodes alarm)
        {
            bool removed = ErrorCodes.Remove(alarm);
            if (removed)
            {
                // Console.WriteLine($"[{alarm}] 已排除");
            }
        }
        public STATE State => CheckStateDataContent();

        public abstract STATE CheckStateDataContent();
    }
}
