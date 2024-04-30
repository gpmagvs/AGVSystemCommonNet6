using AGVSystemCommonNet6.Alarm;
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
        public static Dictionary<string, clsVCSTrobleShooting> VCSTrobleShootings = new Dictionary<string, clsVCSTrobleShooting>();
        public static string TROBLE_SHOOTING_FILE_PATH = @"C:\AGVS\VCS_TrobleShooting.csv";
        public static bool Active { get; set; } = false;

        public static ConcurrentDictionary<DateTime, clsAlarmCode> CurrentAlarms = new ConcurrentDictionary<DateTime, clsAlarmCode>()
        {
        };
        public static List<AlarmCodes> AlwaysReoverableAlarms = new List<AlarmCodes>() {
            AlarmCodes.Read_Cst_ID_Fail_Service_Done_But_Topic_No_CSTID,
            AlarmCodes.Cst_ID_Not_Match,
            AlarmCodes.Read_Cst_ID_Fail
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

        public static void ClearAlarm(AlarmCodes[] Alarm_codes)
        {
            foreach (AlarmCodes al in Alarm_codes)
            {
                ClearAlarm(al);
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
        public static void AddWarning(IEnumerable<AlarmCodes> Alarm_codes)
        {
            foreach (var item in Alarm_codes)
            {
                AddWarning(item);
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
        public static void RecordAlarm(AlarmCodes Alarm_code)
        {
            if (!Active)
                return;
            AddAlarm(Alarm_code, true);
        }
        public static void AddAlarm(IEnumerable<Tuple<AlarmCodes, bool>> Alarm_codes)
        {
            foreach (var item in Alarm_codes)
            {
                AddAlarm(item.Item1, item.Item2);
            }
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

                if (!IsRecoverable && !AlwaysReoverableAlarms.Contains(Alarm_code))
                    OnUnRecoverableAlarmOccur?.Invoke(Alarm_code, Alarm_code);
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }

        }

        public static void LoadVCSTrobleShootings()
        {
            //if (File.Exists(TROBLE_SHOOTING_FILE_PATH))
            //{
            //    string? _AllTrobleShootingDescription = File.ReadAllText(TROBLE_SHOOTING_FILE_PATH);
            //    List<string>? _TrobleShootingDescription = _AllTrobleShootingDescription.Split("\r\n").ToList();
            //    _TrobleShootingDescription.RemoveAt(0);
            //    foreach (string TrobleShooting in _TrobleShootingDescription)
            //    {
            //        string[] TrobleShootingCase = TrobleShooting.Split(',');
            //        if (TrobleShootingCase.Count() < 3)
            //            continue;
            //        if (VCSTrobleShootings.ContainsKey(TrobleShootingCase[0]))
            //            continue;
            //        VCSTrobleShootings.Add(TrobleShootingCase[0], new clsVCSTrobleShooting()
            //        {
            //            Alarm = TrobleShootingCase[0],
            //            TrobleShootingDescription = TrobleShootingCase[1],
            //            TrobleShootingFilePath = TrobleShootingCase[2]
            //        });
            //    }
            //}
            //else
            //{
            //    Directory.CreateDirectory(Path.GetDirectoryName(TROBLE_SHOOTING_FILE_PATH));

            //    FileStream fs = new FileStream(TROBLE_SHOOTING_FILE_PATH, FileMode.Append);

            //    using (StreamWriter sr = new StreamWriter(fs))
            //    {
            //        sr.WriteLine("Alarm,TrobleShootingDescription,TrobleShootingFilePath");

            //        var Alarms = Enum.GetValues(typeof(AlarmCodes)).Cast<AlarmCodes>();

            //        foreach (var item in Alarms)
            //        {
            //            string AlarmDescription = item.ToString();
            //            sr.WriteLine($"{AlarmDescription},,");
            //            if (VCSTrobleShootings.ContainsKey(AlarmDescription) == false)
            //            {
            //                VCSTrobleShootings.Add(AlarmDescription, new clsVCSTrobleShooting()
            //                {
            //                    Alarm = item.ToString(),
            //                });
            //            }
            //        }
            //    }
            //}

            UpdateVCSTrobleShootings(ref VCSTrobleShootings);
        }

        public static void UpdateVCSTrobleShootings(ref Dictionary<string, clsVCSTrobleShooting> VCSTrobleShootings)
        {
            if (File.Exists(TROBLE_SHOOTING_FILE_PATH))
            {
                string? _AllTrobleShootingDescription = File.ReadAllText(TROBLE_SHOOTING_FILE_PATH);
                List<string>? _TrobleShootingDescription = _AllTrobleShootingDescription.Split("\r\n").ToList();
                _TrobleShootingDescription.RemoveAt(0);
                foreach (string TrobleShooting in _TrobleShootingDescription)
                {
                    string[] TrobleShootingCase = TrobleShooting.Split(',');
                    if (TrobleShootingCase.Count() < 3)
                        continue;
                    if (VCSTrobleShootings.ContainsKey(TrobleShootingCase[0]))
                        continue;
                    VCSTrobleShootings.Add(TrobleShootingCase[0], new clsVCSTrobleShooting()
                    {
                        Alarm = TrobleShootingCase[0],
                        TrobleShootingDescription = TrobleShootingCase[1],
                        TrobleShootingFilePath = TrobleShootingCase[2]
                    });
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TROBLE_SHOOTING_FILE_PATH));
            }

            var Alarms = Enum.GetValues(typeof(AlarmCodes)).Cast<AlarmCodes>();

            foreach (var item in Alarms)
            {
                string AlarmDescription = item.ToString();

                if (VCSTrobleShootings.ContainsKey(AlarmDescription))
                    continue;

                VCSTrobleShootings.Add(AlarmDescription, new clsVCSTrobleShooting()
                {
                    Alarm = item.ToString(),
                });
            }

            FileStream fs = new FileStream(TROBLE_SHOOTING_FILE_PATH, FileMode.Create);

            using (StreamWriter sr = new StreamWriter(fs))
            {
                sr.WriteLine("Alarm,TrobleShootingDescription,TrobleShootingFilePath");
                foreach (var item in VCSTrobleShootings)
                {
                    sr.WriteLine($"{item.Value.Alarm},{item.Value.TrobleShootingDescription},{item.Value.TrobleShootingFilePath}");
                }
            }
        }
    }
}
