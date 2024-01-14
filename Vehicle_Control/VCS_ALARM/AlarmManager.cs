using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Vehicle_Control.VCSDatabase;
using Newtonsoft.Json;
using SQLite;
using System.Collections.Concurrent;

namespace AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM
{
    public class AlarmManager
    {

        public static List<clsAlarmCode> AlarmList { get; private set; } = new List<clsAlarmCode>();
        public static bool Active { get; set; } = false;

        public static ConcurrentDictionary<DateTime, clsAlarmCode> CurrentAlarms = new ConcurrentDictionary<DateTime, clsAlarmCode>()
        {
        };
        private static SQLiteConnection db;

        internal static event EventHandler OnAllAlarmClear;
        public static event EventHandler<AlarmCodes> OnUnRecoverableAlarmOccur;
        public static bool LoadAlarmList(string alarm_JsonFile, out string message)
        {
            LOG.INFO("Alarm List File to load :" + alarm_JsonFile);
            message = string.Empty;
            try
            {
                bool alarm_json_file_exist = File.Exists(alarm_JsonFile);
                string default_alarm_json_file_path = Path.Combine(Environment.CurrentDirectory, "src/AlarmList.json");
                FileInfo fiinfo = new FileInfo(default_alarm_json_file_path);
                FileInfo fiinfo_on_param_folder = new FileInfo(alarm_JsonFile);
                bool isAlarmDefaultUpdated = fiinfo.LastWriteTime > fiinfo_on_param_folder.LastWriteTime;
                if (!alarm_json_file_exist || isAlarmDefaultUpdated)
                {
                    File.Copy(default_alarm_json_file_path, alarm_JsonFile, true);
                    LOG.TRACE($"Copy New AlarmList.json file to {alarm_JsonFile}");
                }
                AlarmList = JsonConvert.DeserializeObject<List<clsAlarmCode>>(File.ReadAllText(alarm_JsonFile));
                LOG.INFO($"Alarm List Loaded !.{AlarmList.Count}");
                return true;
            }
            catch (Exception ex)
            {
                LOG.Critical($"Alarm Code List load fail. {ex.Message} ", ex);
                Environment.Exit(0);
                return false;
            }
        }
        public static void ClearAlarm(AlarmCodes Alarm_code)
        {
            var exist_al = CurrentAlarms.FirstOrDefault(i => i.Value.EAlarmCode == Alarm_code);
            if (exist_al.Value != null)
            {
                CurrentAlarms.TryRemove(exist_al);
            }
        }
        public static void ClearAlarm(int alarm_code)
        {
            var exist_al = CurrentAlarms.FirstOrDefault(al => al.Value.Code == alarm_code);
            if (exist_al.Value != null)
            {
                ClearAlarm(exist_al.Value.EAlarmCode);
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
            if (!Active)
                return;
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
            var existAlar = CurrentAlarms.FirstOrDefault(al => al.Value.EAlarmCode == Alarm_code);
            if (existAlar.Value != null)
                CurrentAlarms.TryRemove(existAlar.Key, out _);
            CurrentAlarms.TryAdd(warning_save.Time, warning_save);
            DBhelper.InsertAlarm(warning_save);
        }

        /// <summary>
        /// 新增一筆Alarm (可復歸)
        /// </summary>
        /// <param name="Alarm_code"></param>
        public static void AddAlarm(AlarmCodes Alarm_code)
        {
            if (!Active)
                return;
            AddAlarm(Alarm_code, true);
        }
        public static void AddAlarm(AlarmCodes Alarm_code, bool IsRecoverable)
        {
            if (!Active)
                return;
            if (Alarm_code == AlarmCodes.None)
                IsRecoverable = true;

            if (CurrentAlarms.Count > 0)
            {
                bool isRepeatAlarm = CurrentAlarms.Any(al => al.Value.EAlarmCode == Alarm_code && (DateTime.Now - al.Key).TotalMilliseconds < 100);
                if (isRepeatAlarm)
                {
                    return;
                }
            }
            try
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
                LOG.Critical($"Add Alarm_{alarm.Code}:{alarm.CN}({alarm.Description})");
                clsAlarmCode alarm_save = alarm.Clone();
                alarm_save.Time = DateTime.Now;
                alarm_save.ELevel = clsAlarmCode.LEVEL.Alarm;
                alarm_save.IsRecoverable = IsRecoverable;
                if (CurrentAlarms.TryAdd(alarm_save.Time, alarm_save))
                {
                    if (Alarm_code != AlarmCodes.None)
                        DBhelper.InsertAlarm(alarm_save);
                }

                if (!IsRecoverable && Alarm_code != AlarmCodes.Cst_ID_Not_Match && Alarm_code != AlarmCodes.Read_Cst_ID_Fail && Alarm_code != AlarmCodes.Read_Cst_ID_Fail_Service_Done_But_Topic_No_CSTID)
                    OnUnRecoverableAlarmOccur?.Invoke(Alarm_code, Alarm_code);
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }

        }
    }
}
