namespace AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM
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

        public abstract string alarm_locate_in_name { get; }

        private AlarmCodes _current_alarm_code = AlarmCodes.None;
        private AlarmCodes _current_warning_code = AlarmCodes.None;

        public virtual AlarmCodes Current_Warning_Code
        {
            set
            {
                if (_current_warning_code != value)
                {
                    if (value != AlarmCodes.None)
                    {
                        AlarmManager.AddWarning(value);
                    }
                    else
                    {
                        AlarmManager.ClearAlarm(_current_warning_code);
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
                        AlarmManager.AddAlarm(value, false);
                    }

                    _current_alarm_code = value;
                }
            }
            get => _current_alarm_code;
        }
    }
}
