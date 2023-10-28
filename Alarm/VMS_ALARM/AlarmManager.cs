using AGVSystemCommonNet6.GPMRosMessageNet.Messages;
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
        public static bool Active { get; set; } = true;

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
                if (!File.Exists(alarm_JsonFile))
                {
                    File.Copy(Path.Combine(Environment.CurrentDirectory, "src/AlarmList.json"), alarm_JsonFile);
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
            var existAlar = (CurrentAlarms.FirstOrDefault(al => al.Value.EAlarmCode == Alarm_code));
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
        public static AlarmCodes ConvertAGVCAlarmCode(AlarmCodeMsg alarm_code, out clsAlarmCode.LEVEL Level)
        {
            int code = alarm_code.AlarmCode;
            int level = alarm_code.Level;
            Level = level == 1 ? clsAlarmCode.LEVEL.Warning : clsAlarmCode.LEVEL.Alarm;

            if (code == 1)
                return AlarmCodes.Motion_control_Wrong_Received_Msg;
            else if (code == 2)
                return AlarmCodes.Motion_control_Wrong_Extend_Path;
            else if (code == 3)
                return AlarmCodes.Motion_control_Out_Of_Line_While_Forwarding_End;
            else if (code == 4)
                return AlarmCodes.Motion_control_Out_Of_Line_While_Tracking_End_Point;
            else if (code == 5)
                return AlarmCodes.Motion_control_Out_Of_Line_While_Moving;
            else if (code == 6)
                return AlarmCodes.Motion_control_Out_Of_Line_While_Secondary;
            else if (code == 7)
                return AlarmCodes.Motion_control_Missing_Tag_On_End_Point;
            else if (code == 8)
                return AlarmCodes.Motion_control_Missing_Tag_While_Moving;
            else if (code == 9)
                return AlarmCodes.Motion_control_Missing_Tag_While_Secondary;
            else if (code == 10)
                return AlarmCodes.Motion_control_Wrong_Initial_Position_In_Secondary;
            else if (code == 11)
                return AlarmCodes.Motion_control_Wrong_Initial_Angle_In_Secondary;
            else if (code == 12)
                return AlarmCodes.Motion_control_Wrong_Unknown_Code;
            else if (code == 13)
                return AlarmCodes.Map_Recognition_Rate_Too_Low;
            else
                return AlarmCodes.Motion_control_Wrong_Unknown_Code;
        }


    }
}
