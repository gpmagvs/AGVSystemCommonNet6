using AGVSystemCommonNet6.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.Abstracts.CarComponent;

namespace AGVSystemCommonNet6.Alarm.VMS_ALARM
{
    public abstract class AGVAlarmReportable
    {

        public static Action<AlarmCodes> onAlarmOccur;
        public static event EventHandler OnAlarmResetAsNoneRequest;
        public AGVAlarmReportable()
        {
            OnAlarmResetAsNoneRequest += AGVAlarmReportable_OnAlarmResetAsNoneRequest;
        }

        private void AGVAlarmReportable_OnAlarmResetAsNoneRequest(object? sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                if (_current_alarm_code != AlarmCodes.None)
                    AlarmManager.ClearAlarm(_current_alarm_code);
                if (_current_warning_code != AlarmCodes.None)
                    AlarmManager.ClearAlarm(_current_warning_code);
                _current_alarm_code = AlarmCodes.None;
                _current_warning_code = AlarmCodes.None;
            });
        }

        public static void ResetAlarmCodes()
        {
            OnAlarmResetAsNoneRequest?.Invoke("", EventArgs.Empty);
        }

        public STATE CurrentAlarmState { get; private set; }
        public abstract string alarm_locate_in_name { get; }

        private AlarmCodes _current_alarm_code = AlarmCodes.None;
        private AlarmCodes _current_warning_code = AlarmCodes.None;

        public AlarmCodes Current_Warning_Code
        {
            set
            {
                if (_current_warning_code != value)
                {
                    if (value != AlarmCodes.None)
                    {
                        CurrentAlarmState = STATE.ABNORMAL;
                        AlarmManager.AddWarning(value);
                        LOG.WARN($"{alarm_locate_in_name} Warning: {value}");
                    }
                    _current_warning_code = value;
                }
            }
            get => _current_warning_code;
        }

        public AlarmCodes Current_Alarm_Code
        {
            set
            {
                if (_current_alarm_code != value)
                {
                    if (value != AlarmCodes.None)
                    {
                        CurrentAlarmState = STATE.ABNORMAL;
                        AlarmManager.AddAlarm(value, false);
                        LOG.ERROR($"{alarm_locate_in_name} Alarm: {value}");
                    }

                    _current_alarm_code = value;
                }
            }
            get => _current_alarm_code;
        }
    }
}
