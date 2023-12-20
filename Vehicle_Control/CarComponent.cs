using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using RosSharp.RosBridgeClient;

namespace AGVSystemCommonNet6.Vehicle_Control
{
    /// <summary>
    /// 車控發佈的 module_information 各元件狀態
    /// </summary>
    public abstract class CarComponent : AGVAlarmReportable
    {
        public enum COMPOENT_NAME
        {
            BATTERY,
            DRIVER,
            IMU,
            BARCODE_READER,
            GUID_SENSOR,
            CST_READER,
            NAVIGATION,
            VERTIVAL_DRIVER,
            SICK
        }
        public enum STATE
        {
            NORMAL,
            WARNING,
            ABNORMAL
        }

        private void AGVAlarmReportable_OnAlarmResetAsNoneRequest(object? sender, EventArgs e)
        {
            OnAlarmResetHandle();
        }
        public virtual void OnAlarmResetHandle()
        {

        }
        private Message _StateData;
        public DateTime lastUpdateTime { get; set; } = DateTime.MinValue;
        public abstract COMPOENT_NAME component_name { get; }

        public delegate Task<bool> AlarmHappendDelegate(AlarmCodes alarm);
        public AlarmHappendDelegate OnAlarmHappened { get; set; }

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


        public virtual void CheckStateDataContent()
        {
            if (_StateData == null)
            {
                return;
            }
        }
    }
}
