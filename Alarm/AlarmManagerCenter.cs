using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace AGVSystemCommonNet6.Alarm
{
    public class AlarmManagerCenter
    {
        public static string ALARM_CODE_FILE_PATH = @"C:\AGVS\AGVS_AlarmCodes.json";
        public static string TROBLE_SHOOTING_FILE_PATH = @"C:\AGVS\AGVS_TrobleShooting.csv";
        public static Dictionary<ALARMS, clsAlarmCode> AlarmCodes = new Dictionary<ALARMS, clsAlarmCode>();
        public static Dictionary<string, clsAGVsTrobleShooting> AGVsTrobleShootings = new Dictionary<string, clsAGVsTrobleShooting>();
        public static List<clsAlarmDto> uncheckedAlarms
        {
            get
            {
                using (var dbhelper = new AGVSDatabase())
                {
                    return dbhelper.tables.SystemAlarms.AsNoTracking().Where(al => !al.Checked).ToList();
                }
            }
        }
        private static bool Initialized = false;
        public AlarmManagerCenter() { }

        public static void Initialize()
        {
            LoadAlarmCodes();
            LoadTrobleShootingDescription();
            Initialized = true;
            SetAllAlarmChecked();
        }
        public static async void SetAllAlarmChecked()
        {
            try
            {

                using var db = new AGVSDatabase();
                foreach (var alarm in db.tables.SystemAlarms.Where(al => !al.Checked))
                {
                    alarm.Checked = true;
                }
                int num = await db.SaveChanges();
                LOG.TRACE($"{num} alarms set as checked");
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }

        }
        public static async Task UpdateAlarmAsync(clsAlarmDto alarmDto)
        {
            try
            {
                var dbhelper = new AGVSDatabase();
                var alarmExist = dbhelper.tables.SystemAlarms.FirstOrDefault(alarm => alarm.Time == alarmDto.Time || alarm.AlarmCode == alarmDto.AlarmCode);
                if (alarmExist != null)
                {
                    foreach (var prop in alarmDto.GetType().GetProperties())
                    {
                        var val = prop.GetValue(alarmDto);
                        var val_old = prop.GetValue(alarmExist);
                        if (val_old.ToString() != val.ToString() && prop.PropertyType != typeof(DateTime))
                        {

                            alarmExist.GetType().GetProperty(prop.Name).SetValue(alarmExist, val);
                        }
                    }

                    //dbhelper._context.SystemAlarms.Update(alarmDto);
                    await dbhelper.SaveChanges();
                }
                else
                    await AddAlarmAsync(alarmDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public static async Task AddAlarmAsync(clsAlarmDto alarmDto)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (!Initialized)
                    Initialize();
                using var db = new AGVSDatabase();
                db.tables.SystemAlarms.Add(alarmDto);
                await db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { semaphoreSlim.Release(); }
        }
        static object AlarmLockObject = new object();

        public static async Task<clsAlarmDto> AddAlarmAsync(ALARMS alarm, ALARM_SOURCE source = ALARM_SOURCE.AGVS, ALARM_LEVEL level = ALARM_LEVEL.ALARM, string Equipment_Name = "", string location = "", string taskName = "")
        {

            try
            {

                clsAlarmCode alarmCodeData;
                string description_zh = "";
                string description_En = "";
                if (!AlarmCodes.TryGetValue(alarm, out alarmCodeData))
                {
                    description_zh = description_En = alarm.ToString();
                }
                else
                {
                    description_zh = alarmCodeData.Description_Zh;
                    description_En = alarmCodeData.Description_En;
                }
                clsAlarmDto alarmDto = new clsAlarmDto()
                {
                    Equipment_Name = Equipment_Name,
                    Description_Zh = description_zh,
                    Description_En = description_En,
                    Level = level,
                    AlarmCode = (int)alarm,
                    OccurLocation = location == null ? "" : location,
                    Task_Name = taskName == null ? "" : taskName,
                    Time = DateTime.Now,
                    Source = source,
                    //TrobleShootingMethod = AGVsTrobleShootings[alarm.ToString()].TrobleShootingDescription,
                    //TrobleShootingReference = AGVsTrobleShootings[alarm.ToString()].TrobleShootingFilePath
                };
                lock (AlarmLockObject)
                {
                    alarmDto.Time = DateTime.Now;
                }

                await AddAlarmAsync(alarmDto);
                LOG.WARN($"AGVS Alarm Add : {alarmDto.ToJson(Formatting.None)}");
                return alarmDto;
            }
            catch (Exception ex)
            {
                LOG.ERROR("AddAlarmAsync", ex);
                return null;
            }
            finally
            {

            }
        }


        private static void LoadAlarmCodes()
        {
            if (File.Exists(ALARM_CODE_FILE_PATH))
            {
                clsAlarmCode[]? _AlarmCodes = JsonConvert.DeserializeObject<clsAlarmCode[]>(File.ReadAllText(ALARM_CODE_FILE_PATH));
                AlarmCodes = _AlarmCodes.ToDictionary(ac => ac.AlarmCode, ac => ac);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ALARM_CODE_FILE_PATH));
                AlarmCodes.Add(ALARMS.VMS_DISCONNECT, new clsAlarmCode
                {
                    AlarmCode = ALARMS.VMS_DISCONNECT,
                    Description_En = "VMS Disconnect",
                    Description_Zh = "車載管理系統斷線"
                });
                File.WriteAllText(ALARM_CODE_FILE_PATH, JsonConvert.SerializeObject(AlarmCodes.Values.ToArray(), Formatting.Indented));
            }
        }

        private static void LoadTrobleShootingDescription()
        {
            //if (File.Exists(TROBLE_SHOOTING_FILE_PATH))
            //{
            //    string? _AllTrobleShootingDescription = File.ReadAllText(TROBLE_SHOOTING_FILE_PATH);
            //    List<string>? _TrobleShootingDescription = _AllTrobleShootingDescription.Split("\r\n").ToList();
            //    _TrobleShootingDescription.RemoveAt(0);
            //    foreach (string TrobleShooting in _TrobleShootingDescription)
            //    {
            //        string[] TrobleShootingCase = TrobleShooting.Split(',');
            //        string TrobleCaseName = TrobleShootingCase[0];
            //        if (TrobleShootingCase.Count() < 3)
            //            continue;
            //        if (AGVsTrobleShootings.ContainsKey(TrobleShootingCase[0]))
            //            continue;
            //        AGVsTrobleShootings.Add(TrobleShootingCase[0], new clsAGVsTrobleShooting()
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

            //        var Alarms = Enum.GetValues(typeof(ALARMS)).Cast<ALARMS>();

            //        foreach (var item in Alarms)
            //        {
            //            string AlarmDescription = item.ToString();
            //            sr.WriteLine($"{AlarmDescription},,");
            //            if (AGVsTrobleShootings.ContainsKey(AlarmDescription) == false)
            //            {
            //                AGVsTrobleShootings.Add(AlarmDescription, new clsAGVsTrobleShooting()
            //                {
            //                    Alarm = item.ToString(),
            //                });
            //            }
            //        }
            //    }
            //}

            UadateAGVsTrobleShootings(ref AGVsTrobleShootings);
        }

        public static void UadateAGVsTrobleShootings(ref Dictionary<string, clsAGVsTrobleShooting> AGVsTrobleShootings)
        {
            if (File.Exists(TROBLE_SHOOTING_FILE_PATH))
            {
                string? _AllTrobleShootingDescription = File.ReadAllText(TROBLE_SHOOTING_FILE_PATH, Encoding.BigEndianUnicode);
                List<string>? _TrobleShootingDescription = _AllTrobleShootingDescription.Split("\r\n").ToList();
                _TrobleShootingDescription.RemoveAt(0);
                foreach (string TrobleShooting in _TrobleShootingDescription)
                {
                    string[] TrobleShootingCase = TrobleShooting.Split(',');
                    if (TrobleShootingCase.Count() != 3)
                        continue;
                    if (AGVsTrobleShootings.ContainsKey(TrobleShootingCase[0]))
                        continue;
                    AGVsTrobleShootings.Add(TrobleShootingCase[0], new clsAGVsTrobleShooting()
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

            var Alarms = Enum.GetValues(typeof(ALARMS)).Cast<ALARMS>();

            foreach (var item in Alarms)
            {
                string AlarmDescription = item.ToString();

                if (AGVsTrobleShootings.ContainsKey(AlarmDescription))
                    continue;

                AGVsTrobleShootings.Add(AlarmDescription, new clsAGVsTrobleShooting()
                {
                    Alarm = item.ToString(),
                });
            }

            FileStream fs = new FileStream(TROBLE_SHOOTING_FILE_PATH, FileMode.Create);

            using (StreamWriter sr = new StreamWriter(fs))
            {
                sr.WriteLine("Alarm,TrobleShootingDescription,TrobleShootingFilePath");
                foreach (var item in AGVsTrobleShootings)
                {
                    sr.WriteLine($"{item.Value.Alarm},{item.Value.TrobleShootingDescription},{item.Value.TrobleShootingFilePath}");
                }
            }
        }

        public static clsAlarmCode GetAlarmCode(ALARMS alarm_enum)
        {
            try
            {

                if (!Initialized)
                    Initialize();
                return AlarmCodes[alarm_enum];
            }
            catch (Exception ex)
            {
                return new clsAlarmCode
                {
                    AlarmCode = alarm_enum,
                    Description_En = alarm_enum.ToString(),
                    Description_Zh = alarm_enum.ToString()
                };
            }
        }

        public static async Task ResetAlarmAsync(clsAlarmDto alarm, bool resetAllSameCode = false)
        {
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                if (resetAllSameCode)
                {
                    var alarms_same_code = dbhelper._context.SystemAlarms.Where(_alarm => _alarm.Checked == false && _alarm.AlarmCode == alarm.AlarmCode);
                    foreach (clsAlarmDto? alarm_ in alarms_same_code)
                    {
                        alarm_.Checked = true;
                        alarm_.ResetAalrmMemberName = alarm.ResetAalrmMemberName;
                        await UpdateAlarmAsync(alarm_);
                    }
                }
                else
                {
                    if (dbhelper._context.Set<clsAlarmDto>().FirstOrDefault(a => a.Checked == false && (a == alarm || a.AlarmCode == alarm.AlarmCode)) != null)
                    {
                        alarm.Checked = true;
                        await UpdateAlarmAsync(alarm);
                    }
                }
            }

        }

        private static object _lockObj = new object();

        public static int RemoveAlarm(clsAlarmDto alarmDto)
        {
            lock (_lockObj)
            {
                try
                {
                    using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                    {
                        dbhelper._context.Set<clsAlarmDto>().Remove(alarmDto);
                        return dbhelper._context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    LOG.ERROR(ex);
                    return -1;
                }
            }
        }
        public static void SqlSelect(clsAlarmDto alarmquery)
        {
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                dbhelper._context.Set<clsAlarmDto>().ToList();
            }
        }

        public static void AlarmQuery(out int count, int currentpage, DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, out List<clsAlarmDto> alarms, string AlarmType = "ALL")
        {
            count = 0;
            alarms = new List<clsAlarmDto>();
            ALARM_LEVEL level_to_query = AlarmType.ToUpper() == "ALARM" ? ALARM_LEVEL.ALARM : ALARM_LEVEL.WARNING;
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _alarms = dbhelper._context.Set<clsAlarmDto>().OrderByDescending(alarm => alarm.Time).Where(alarm => alarm.Time >= startTime
                                    && alarm.Time <= endTime
                                    && (AGV_Name == "ALL" ? (true) : (alarm.Equipment_Name == AGV_Name))
                                    && (TaskName == null ? (true) : (alarm.Task_Name.Contains(TaskName)))
                                    && (AlarmType == "ALL" ? (true) : (alarm.Level == level_to_query))
                );
                count = _alarms.Count();
                alarms = _alarms.Skip((currentpage - 1) * 19).Take(19).ToList();

                //foreach (var alarm in alarms)
                //{
                //    alarm.TrobleShootingMethod = AGVsTrobleShootings[((Alarm.ALARMS)alarm.AlarmCode).ToString()].TrobleShootingDescription;
                //    alarm.TrobleShootingReference = AGVsTrobleShootings[((Alarm.ALARMS)alarm.AlarmCode).ToString()].TrobleShootingFilePath;
                //}
            };
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string fileName)
        {
            return SaveTocsv(startTime, endTime, "ALL", null, fileName);
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, string fileName = null)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, @"SaveLog\\Alarm");
            var _fileName = fileName is null ? DateTime.Now.ToString("yyyy-MM-dd-HH") + ".csv" : fileName;
            Directory.CreateDirectory(folder);
            string FilePath = Path.Combine(folder, "AlarmQuery_" + _fileName);
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _alarms = dbhelper._context.Set<clsAlarmDto>().Where(alarm => alarm.Time >= startTime && alarm.Time <= endTime
                                    && (AGV_Name == "ALL" ? (true) : (alarm.Equipment_Name == AGV_Name)) && (TaskName == null ? (true) : (alarm.Task_Name.Contains(TaskName)))
                );

                List<string> list = _alarms.Select(alarm => $"{alarm.Time},{alarm.Task_Name},{alarm.Equipment_Name},,{alarm.AlarmCode}{alarm.Description_En},{alarm.Description_Zh},{alarm.Duration},{alarm.OccurLocation},{alarm.Level}").ToList();
                File.WriteAllLines(FilePath, list, Encoding.GetEncoding("big5"));
            };
            return FilePath;
        }
        public static async Task SetAlarmCheckedAsync(string eQName, int alarm_code, string checker_name = "")
        {
            try
            {

                using (var dbhelper = new AGVSDatabase())
                {
                    var alarms = dbhelper.tables.SystemAlarms.Where(alarm => !alarm.Checked && alarm.Equipment_Name == eQName && alarm.AlarmCode == (int)alarm_code).ToArray();
                    foreach (var item in alarms)
                    {
                        item.Checked = true;
                    }
                    await dbhelper.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }
        }
        public static async Task SetAlarmCheckedAsync(string eQName, ALARMS alarm_code, string checker_name = "")
        {
            await SetAlarmCheckedAsync(eQName, (int)alarm_code, checker_name);
        }

        public static async Task SetAlarmsCheckedAsync(string eQName, List<ALARMS> unchecked_alarms, string checker_name = "")
        {
            foreach (var _alarm in unchecked_alarms)
            {
                await SetAlarmCheckedAsync(eQName, (int)_alarm, checker_name);
            }
        }

        public static void SetAlarmsAllCheckedByEquipmentName(string name)
        {
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                foreach (var _alarm in dbhelper._context.SystemAlarms.Where(alarm => alarm.Equipment_Name == name))
                {
                    _alarm.Checked = true;
                }

                dbhelper._context.SaveChanges();
            }
        }

        public static clsAlarmDto[] GetAlarmsByEqName(string name)
        {
            clsAlarmDto[] alarms = new clsAlarmDto[0];
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                alarms = dbhelper._context.SystemAlarms.Where(alarm => alarm.Equipment_Name == name).ToArray();
            }
            return alarms;
        }


        public static void UpdateAlarmDuration(string name, AGVSystemCommonNet6.AGVDispatch.Model.clsAlarmCode alarm)
        {

        }

        public static void UpdateAlarmDuration(string name, int alarm_ID)
        {
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var alarms = dbhelper._context.SystemAlarms.Where(alarm => alarm.Equipment_Name == name && alarm.AlarmCode == alarm_ID).ToArray();
                alarms.Last().Duration = int.Parse(Math.Round((DateTime.Now - alarms.Last().Time).TotalSeconds) + "");
                dbhelper._context.SaveChanges();
            }
        }
    }

}
