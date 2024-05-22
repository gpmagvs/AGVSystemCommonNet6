﻿using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using static AGVSystemCommonNet6.DATABASE.DatabaseCaches;

namespace AGVSystemCommonNet6.Alarm
{
    public class AlarmManagerCenter
    {
        public static string ALARM_CODE_FILE_PATH = @".\Resources\AGVS_AlarmCodes.json";
        public static string TROBLE_SHOOTING_FILE_PATH = Path.Combine(AGVSConfigulator.ConfigsFilesFolder, "AGVS_TrobleShooting.csv");
        public static Dictionary<ALARMS, clsAlarmCode> AlarmCodes = new Dictionary<ALARMS, clsAlarmCode>();
        public static Dictionary<int, clsAlarmCode> AlarmCodes2 = new Dictionary<int, clsAlarmCode>();
        public static Dictionary<string, clsAGVsTrobleShooting> AGVsTrobleShootings = new Dictionary<string, clsAGVsTrobleShooting>();
        private static AGVSDatabase database;
        public static List<clsAlarmDto> uncheckedAlarms
        {
            get
            {
                return DatabaseCaches.Alarms.UnCheckedAlarms;
            }
        }
        private static bool Initialized = false;
        public AlarmManagerCenter() { }

        public static void Initialize()
        {
            database = new AGVSDatabase();
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
                    await semaphoreSlim.WaitAsync();
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
                    semaphoreSlim.Release();
                }
                else
                    await AddAlarmAsync(alarmDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public static async Task AddAlarmAsync(clsAlarmDto alarmDto)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                if (!Initialized)
                    Initialize();
                database.tables.SystemAlarms.Add(alarmDto);
                await database.SaveChanges();
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
                    //TrobleShootingMethod = AGVsTrobleShootings[alarm.ToString()].EN_TrobleShootingDescription,
                    //TrobleShootingReference = AGVsTrobleShootings[alarm.ToString()].TrobleShootingFilePath
                };
                alarmDto.Time = DateTime.Now;
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
        public static async Task<clsAlarmDto> AddAlarmAsync(int alarm, ALARM_SOURCE source = ALARM_SOURCE.AGVS, ALARM_LEVEL level = ALARM_LEVEL.ALARM, string Equipment_Name = "", string location = "", string taskName = "")
        {

            try
            {
                clsAlarmCode alarmCodeData;
                string description_zh = "";
                string description_En = "";
                if (!AlarmCodes2.TryGetValue(alarm, out alarmCodeData))
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
                    //TrobleShootingMethod = AGVsTrobleShootings[alarm.ToString()].EN_TrobleShootingDescription,
                    //TrobleShootingReference = AGVsTrobleShootings[alarm.ToString()].TrobleShootingFilePath
                };
                alarmDto.Time = DateTime.Now;
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
                AlarmCodes2 = _AlarmCodes.ToDictionary(ac => ac.AlarmCode, ac => ac);
            }
            else
            {
                //TODO
            }
        }

        private static void LoadTrobleShootingDescription()
        {
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Resources")) == true)
            {
                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "AGVS_TrobleShooting.csv")) == true)
                {
                    File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "AGVS_TrobleShooting.csv"), TROBLE_SHOOTING_FILE_PATH, true);
                }
            }
            UadateAGVsTrobleShootings(ref AGVsTrobleShootings);
        }

        public static void UadateAGVsTrobleShootings(ref Dictionary<string, clsAGVsTrobleShooting> AGVsTrobleShootings)
        {
            if (File.Exists(TROBLE_SHOOTING_FILE_PATH))
            {
                string? _AllTrobleShootingDescription = File.ReadAllText(TROBLE_SHOOTING_FILE_PATH, Encoding.GetEncoding("big5"));
                List<string>? _TrobleShootingDescription = _AllTrobleShootingDescription.Split("\r\n").ToList();
                _TrobleShootingDescription.RemoveAt(0);
                foreach (string TrobleShooting in _TrobleShootingDescription)
                {
                    string[] TrobleShootingCase = TrobleShooting.Split(',');
                    if (TrobleShootingCase.Count() != 4)
                        continue;
                    if (AGVsTrobleShootings.ContainsKey(TrobleShootingCase[0]))
                        continue;
                    AGVsTrobleShootings.Add(TrobleShootingCase[0], new clsAGVsTrobleShooting()
                    {
                        Alarm = TrobleShootingCase[0],
                        EN_TrobleShootingDescription = TrobleShootingCase[1],
                        ZH_TrobleShootingDescription = TrobleShootingCase[2],
                        TrobleShootingFilePath = TrobleShootingCase[3]
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
                    EN_TrobleShootingDescription = "Ask GPM for Help",
                    ZH_TrobleShootingDescription = "請洽GPM"
                });

                LOG.WARN($"有未記載的Alarm Code : {item.ToString()}");
            }

            FileStream fs = new FileStream(TROBLE_SHOOTING_FILE_PATH, FileMode.Create);

            using (StreamWriter sr = new StreamWriter(fs, Encoding.GetEncoding("big5")))
            {
                sr.WriteLine("Alarm,En_TrobleShootingDescription,ZH_TrobleShootingDescription,TrobleShootingFilePath");
                foreach (var item in AGVsTrobleShootings)
                {
                    sr.WriteLine($"{item.Value.Alarm},{item.Value.EN_TrobleShootingDescription},{item.Value.ZH_TrobleShootingDescription},{item.Value.TrobleShootingFilePath}");
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
                    AlarmCode = (int)alarm_enum,
                    Description_En = alarm_enum.ToString(),
                    Description_Zh = alarm_enum.ToString()
                };
            }
        }

        public static async Task ResetAlarmAsync(clsAlarmDto alarm, bool resetAllSameCode = false)
        {

            try
            {
                await semaphoreSlim.WaitAsync();
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
            catch (Exception)
            {

                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }


        }

        private static object _lockObj = new object();

        public static async Task<int> RemoveAlarm(clsAlarmDto alarmDto)
        {

            try
            {
                await semaphoreSlim.WaitAsync();
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
            finally
            {
                semaphoreSlim.Release();
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
                //    alarm.TrobleShootingMethod = AGVsTrobleShootings[((Alarm.ALARMS)alarm.AlarmCode).ToString()].EN_TrobleShootingDescription;
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
                List<string> list = new List<string> { "發生時間,EQ名稱,警報碼,警報描述,警報類型,任務名稱 ,發生地點,持續時間,清除警報人員" };
                list.AddRange(_alarms.Select(alarm => $"{alarm.Time},{alarm.Equipment_Name},{alarm.AlarmCode},{alarm.Description},{alarm.Level},{alarm.Task_Name},{alarm.OccurLocation},{alarm.Duration},{alarm.ResetAalrmMemberName}"));
                //List<string> list = _alarms.Select(alarm => $"{alarm.Time},{alarm.Task_Name},{alarm.Equipment_Name},,{alarm.AlarmCode}{alarm.Description_En},{alarm.Description_Zh},{alarm.Duration},{alarm.OccurLocation},{alarm.Level}").ToList();
                File.WriteAllLines(FilePath, list, Encoding.UTF8);
            };
            return FilePath;
        }
        public static async Task SetAlarmCheckedAsync(string eQName, int alarm_code, string checker_name = "")
        {
            try
            {
                await semaphoreSlim.WaitAsync();
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
            finally
            {
                semaphoreSlim.Release();
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

        public static async Task SetAlarmsAllCheckedByEquipmentName(string name)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                using var agvsDb = new AGVSDatabase();
                using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    foreach (var _alarm in dbhelper._context.SystemAlarms.Where(alarm => alarm.Equipment_Name == name))
                    {
                        _alarm.Checked = true;
                    }

                    dbhelper._context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                semaphoreSlim.Release();
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

        public static async Task UpdateAlarmDuration(string name, int alarm_ID)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    var alarms = dbhelper._context.SystemAlarms.Where(alarm => alarm.Equipment_Name == name && alarm.AlarmCode == alarm_ID).ToArray();
                    alarms.Last().Duration = int.Parse(Math.Round((DateTime.Now - alarms.Last().Time).TotalSeconds) + "");
                    dbhelper._context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                semaphoreSlim.Release();
            }

        }
    }

}
