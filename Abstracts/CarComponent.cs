using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Alarm.VMS_ALARM;
using AGVSystemCommonNet6.Log;
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

        private AlarmCodes _current_alarm_code = AlarmCodes.None;
        public AlarmCodes current_alarm_code
        {
            set
            {
                if (_current_alarm_code != value)
                {
                    _current_alarm_code = value;
                    if (value != AlarmCodes.None)
                    {
                        State = STATE.ABNORMAL;
                        AlarmManager.AddWarning(value);
                        LOG.WARN($"{component_name} Alarm: {current_alarm_code}");
                    }
                    else
                        State = STATE.NORMAL;
                }
            }
            get => _current_alarm_code;
        }

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
       
        public STATE State { get; private set; }

        public abstract void CheckStateDataContent();
    }
}
