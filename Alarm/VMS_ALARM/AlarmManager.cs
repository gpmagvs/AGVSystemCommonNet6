using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Tools.Database;
using Newtonsoft.Json;
using SQLite;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace AGVSystemCommonNet6.Alarm.VMS_ALARM
{
    public class AlarmManager
    {

        public static List<clsAlarmCode> AlarmList { get; private set; } = new List<clsAlarmCode>();
        public static ConcurrentDictionary<DateTime, clsAlarmCode> CurrentAlarms = new ConcurrentDictionary<DateTime, clsAlarmCode>();
        private static SQLiteConnection db;

        internal static event EventHandler OnAllAlarmClear;
        public static event EventHandler OnUnRecoverableAlarmOccur;
        public static void LoadAlarmList(string alarm_JsonFile)
        {

            if (File.Exists(alarm_JsonFile))
            {
                AlarmList = JsonConvert.DeserializeObject<List<clsAlarmCode>>(File.ReadAllText(alarm_JsonFile));
                LOG.INFO("Alarm List Loaded.");
            }
            else
                LOG.WARN("Alarm list not Loaded yet...Please confirm your file path setting(VCS:AlarmList_json_Path)");
        }
        public static void ClearAlarm(AlarmCodes Alarm_code)
        {
            var exist_al = CurrentAlarms.FirstOrDefault(i => i.Value.EAlarmCode == Alarm_code);
            if (exist_al.Value != null)
            {
                CurrentAlarms.TryRemove(exist_al);
            }

            if (CurrentAlarms.Count == 0)
            {
                OnAllAlarmClear?.Invoke("AlarmManager", EventArgs.Empty);
            }
        }


        public static void ClearAlarm()
        {
            var currentAlarmCodes = CurrentAlarms.Values.Select(alr => alr.EAlarmCode).ToList();
            foreach (var alarm_code in currentAlarmCodes)
            {
                ClearAlarm(alarm_code);
            }
        }

        public static void AddWarning(AlarmCodes Alarm_code)
        {
            clsAlarmCode warning = AlarmList.FirstOrDefault(a => a.EAlarmCode == Alarm_code);
            if (warning == null)
            {
                warning = new clsAlarmCode
                {
                    Code = (int)Alarm_code,
                    Description = Alarm_code.ToString(),
                    CN = Alarm_code.ToString(),
                };
            }

            clsAlarmCode warning_save = warning.Clone();
            warning_save.Time = DateTime.Now;
            warning_save.ELevel = clsAlarmCode.LEVEL.Warning;
            warning_save.IsRecoverable = true;
            var existAlar = (CurrentAlarms.FirstOrDefault(al => al.Value.EAlarmCode == Alarm_code));
            if (existAlar.Value != null)
            {
                CurrentAlarms.TryRemove(existAlar.Key, out _);
                CurrentAlarms.TryAdd(warning_save.Time, warning_save);
            }
            else
            {

                if (CurrentAlarms.TryAdd(warning_save.Time, warning_save))
                {
                    DBhelper.InsertAlarm(warning_save);
                }
            }
        }
        public static void AddAlarm(AlarmCodes Alarm_code, bool IsRecoverable)
        {
            clsAlarmCode alarm = AlarmList.FirstOrDefault(a => a.EAlarmCode == Alarm_code);
            if (alarm == null)
            {
                alarm = new clsAlarmCode
                {
                    Code = (int)Alarm_code,
                    Description = Alarm_code.ToString(),
                    CN = Alarm_code.ToString(),
                };
            }
            clsAlarmCode alarm_save = alarm.Clone();
            alarm_save.Time = DateTime.Now;
            alarm_save.ELevel = clsAlarmCode.LEVEL.Alarm;
            alarm_save.IsRecoverable = IsRecoverable;
            if (CurrentAlarms.TryAdd(alarm_save.Time, alarm_save))
            {
                if (Alarm_code != AlarmCodes.None)
                    DBhelper.InsertAlarm(alarm_save);
            }

            if (!IsRecoverable)
                OnUnRecoverableAlarmOccur?.Invoke(Alarm_code, EventArgs.Empty);
        }

    }
}
