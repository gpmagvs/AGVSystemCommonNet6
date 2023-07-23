using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Alarm.VMS_ALARM;
using AGVSystemCommonNet6.Log;
using RosSharp.RosBridgeClient;

namespace AGVSystemCommonNet6.Abstracts
{
    public abstract class CarComponent: AGVAlarmReportable
    {
        public enum COMPOENT_NAME
        {
            BATTERY, DRIVER, IMU, BARCODE_READER, GUID_SENSOR, CST_READER,
            NAVIGATION,
            SICK,
            FORK
        }
        public enum STATE
        {
            NORMAL,
            WARNING,
            ABNORMAL
        }

        public CarComponent()
        {

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
       

        public abstract void CheckStateDataContent();
    }
}
