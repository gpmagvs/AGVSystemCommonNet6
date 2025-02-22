﻿using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.GPMRosMessageNet.Messages;
using AGVSystemCommonNet6.Microservices.AGVS;
using AGVSystemCommonNet6.Microservices.MCS;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data;
using System.Text;
using static AGVSystemCommonNet6.DATABASE.DatabaseCaches;

namespace AGVSystemCommonNet6.Alarm
{
    public class AlarmManagerCenter
    {
        //public static string ALARM_CODE_FILE_PATH = @".\Resources\AGVS_AlarmCodes.json";
        public static string ALARM_CODE_FILE_PATH = Path.Combine(AGVSConfigulator.ConfigsFilesFolder, "AGVS_AlarmCodes.json");
        public static string TROBLE_SHOOTING_FILE_PATH = Path.Combine(AGVSConfigulator.ConfigsFilesFolder, "AGVS_TrobleShooting.csv");
        public static Dictionary<ALARMS, clsAlarmCode> AlarmCodes = new Dictionary<ALARMS, clsAlarmCode>();
        public static Dictionary<string, clsAGVsTrobleShooting> AGVsTrobleShootings = new Dictionary<string, clsAGVsTrobleShooting>();
        private static FileSystemWatcher _alarmCodeJsonFileWatcher;
        private static AGVSDatabase database;
        private static IMapper mapper;
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static List<clsAlarmDto> uncheckedAlarms
        {
            get
            {
                return DatabaseCaches.Alarms.UnCheckedAlarms;
            }
        }

        private static bool Initialized = false;
        static AlarmManagerCenter()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<clsAlarmDto, clsAlarmDto>());
            mapper = config.CreateMapper();
        }

        public static async Task InitializeAsync()
        {
            database = new AGVSDatabase();
            LoadAlarmCodes();
            LoadTrobleShootingDescription();
            Initialized = true;
            await SetAllAlarmChecked();
        }
        public static async Task SetAllAlarmChecked()
        {
            try
            {

                if (!await WaitAlarmTableUnLocked())
                    return;
                using var db = new AGVSDatabase();
                foreach (var alarm in db.tables.SystemAlarms.Where(al => !al.Checked))
                {
                    alarm.Checked = true;
                }
                int num = await db.SaveChanges();
                logger.Trace($"{num} alarms set as checked");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

        public static async Task UpdateAlarmDuration(clsAlarmDto alarmDto)
        {
            bool waitAddTimeout = true;
            try
            {
                waitAddTimeout = !await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));
                if (waitAddTimeout)
                    return;
                if (!await WaitAlarmTableUnLocked())
                    return;
                using (var db = new AGVSDatabase())
                {
                    clsAlarmDto alarmInDatabase = db.tables.SystemAlarms.FirstOrDefault(alarm => alarm.Time == alarmDto.Time);
                    if (alarmInDatabase == null || alarmDto.Duration == alarmInDatabase.Duration)
                        return;

                    mapper.Map(alarmDto, alarmInDatabase);
                    db.tables.Entry(alarmInDatabase).State = EntityState.Modified;
                    await db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!waitAddTimeout)
                    semaphoreSlim.Release();
            }
        }

        public static async Task UpdateAlarmAsync(clsAlarmDto alarmDto)
        {
            bool waitAddTimeout = true;
            try
            {
                waitAddTimeout = !await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));
                if (waitAddTimeout)
                    return;
                var dbhelper = new AGVSDatabase();
                var alarmExist = dbhelper.tables.SystemAlarms.FirstOrDefault(alarm => alarm.Time == alarmDto.Time);
                if (alarmExist != null)
                {
                    if (!await WaitAlarmTableUnLocked())
                        return;
                    mapper.Map(alarmDto, alarmExist);
                    dbhelper.tables.Entry(alarmExist).State = EntityState.Modified;
                    await dbhelper.SaveChanges();
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
                if (!waitAddTimeout)
                    semaphoreSlim.Release();
            }
        }
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public static async Task AddAlarmAsync(clsAlarmDto alarmDto)
        {
            bool waitAddTimeout = true;
            try
            {
                waitAddTimeout = !await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));
                if (waitAddTimeout)
                    return;
                if (!Initialized)
                    await InitializeAsync();
                if (alarmDto.AlarmCode == 0)
                    return;
                if (!await WaitAlarmTableUnLocked())
                    return;
                database.tables.SystemAlarms.Add(alarmDto);
                await database.SaveChanges();
                MCSCIMService.AlarmReport((ushort)alarmDto.AlarmCode, alarmDto.Description);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (!waitAddTimeout)
                    semaphoreSlim.Release();
            }
        }
        static object AlarmLockObject = new object();

        public static async Task<clsAlarmDto> AddAlarmAsync(ALARMS alarm, ALARM_SOURCE source = ALARM_SOURCE.AGVS, ALARM_LEVEL level = ALARM_LEVEL.ALARM, string Equipment_Name = "", string location = "", string taskName = "")
        {

            try
            {
                if (alarm == ALARMS.NONE)
                    return new clsAlarmDto();
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
                    Equipment_Name = Equipment_Name == null ? "" : Equipment_Name,
                    Description_Zh = description_zh == null ? "" : description_zh,
                    Description_En = description_En == null ? "" : description_En,
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
                logger.Warn($"AGVS Alarm Add : {alarmDto.ToJson(Formatting.None)}");
                return alarmDto;
            }
            catch (Exception ex)
            {
                logger.Error("AddAlarmAsync", ex);
                return null;
            }
            finally
            {
            }
        }

        private static void LoadAlarmCodes()
        {
            Task.Run(() =>
            {
                AlarmCodeTableLoder alarmCodeTableHelper = new AlarmCodeTableLoder();
                clsAlarmCode[] alarmCodesReadIn = alarmCodeTableHelper.ReadAlarmCodeTable();
                AlarmCodes = alarmCodesReadIn.ToDictionary(ac => ac.AlarmCode, ac => ac);
                InitAlarmCodeJsonFileWatcher();
            });

        }

        private static bool _IsAlarmCodeFileTooOld()
        {
            if (!File.Exists(ALARM_CODE_FILE_PATH))
                return false;

            FileInfo _info = new FileInfo(ALARM_CODE_FILE_PATH);
            return _info.LastWriteTime < new DateTime(2024, 10, 24, 0, 0, 0);
        }

        private static void InitAlarmCodeJsonFileWatcher()
        {
            try
            {
                if (_alarmCodeJsonFileWatcher != null)
                    return;
                _alarmCodeJsonFileWatcher = new FileSystemWatcher(AGVSConfigulator.ConfigsFilesFolder, "AGVS_AlarmCodes.json");
                _alarmCodeJsonFileWatcher.Changed += _alarmCodeJsonFileWatcher_Changed;
                _alarmCodeJsonFileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
            }
        }

        private static void _alarmCodeJsonFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _alarmCodeJsonFileWatcher.EnableRaisingEvents = false;
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                LoadAlarmCodes();
                Console.WriteLine($"Alarm code description file content has changed and system reload it done. => {e.FullPath}");
                await Task.Delay(500);
                _alarmCodeJsonFileWatcher.EnableRaisingEvents = true;

            });
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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

                logger.Warn($"有未記載的Alarm Code : {item.ToString()}");
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
                    InitializeAsync().GetAwaiter().GetResult();
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

            bool waitAddTimeout = true;
            try
            {
                waitAddTimeout = !await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));
                if (waitAddTimeout)
                    return;
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
                if (!waitAddTimeout)
                    semaphoreSlim.Release();
            }


        }

        private static object _lockObj = new object();

        public static async Task<int> RemoveAlarm(clsAlarmDto alarmDto)
        {

            bool waitAddTimeout = true;
            try
            {
                waitAddTimeout = !await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));
                if (waitAddTimeout)
                    return 0;
                if (!await WaitAlarmTableUnLocked())
                    return 0;
                int changedNum = 0;
                using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    dbhelper._context.Set<clsAlarmDto>().Remove(alarmDto);
                    changedNum = dbhelper._context.SaveChanges();
                }
                MCSCIMService.AlarmClear((ushort)alarmDto.AlarmCode, alarmDto.Description);
                return changedNum;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return -1;
            }
            finally
            {
                if (!waitAddTimeout)
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
        public static void QueryAlarmWithKeyword(int currentpage, DateTime startTime, DateTime endTime, string? keyword, out int count, out List<clsAlarmDto> alarms)
        {
            count = 0;
            alarms = new();

            using (AGVSDatabase agvsDatabse = new())
            {
                var _alarms = agvsDatabse.tables.SystemAlarms.AsNoTracking()
                                               .Where(alarm => alarm.Time>=startTime && alarm.Time<=endTime)
                                               .OrderByDescending(alarm=>alarm.Time)
                                               .ToList()
                                               .Where(alarm => KeywordSearch(alarm, keyword))
                                               .ToList();

                count =  _alarms.Count();
                alarms = _alarms.Skip((currentpage - 1) * 19).Take(19).ToList();
            }
        }
        public static void AlarmQuery(out int count, int currentpage, DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, string Alarm_description, out List<clsAlarmDto> alarms, string AlarmType = "ALL")
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
                                    && (Alarm_description == null || Alarm_description == "ALL" ? (true) : (alarm.Description_Zh.Contains(Alarm_description)))

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

        private static bool KeywordSearch(clsAlarmDto alarm, string? alarm_description)
        {
            if (string.IsNullOrEmpty(alarm_description))
                return true;
            string description = (alarm.Description+alarm.AlarmCode+alarm.OccurLocation+alarm.Task_Name+alarm.Time.ToString("yyyy/MM/dd HH:mm:ss")+alarm.Time.ToString("yyyy-MM-dd HH:mm:ss")+alarm.Level+alarm.Equipment_Name+alarm.TrobleShootingMethod+alarm.TrobleShootingReference).ToLower();
            return description.Contains(alarm_description.ToLower());
            //&& (Alarm_description == null || Alarm_description == "ALL" ? (true) : (alarm.Description_Zh.Contains(Alarm_description)))
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string fileName)
        {
            return SaveTocsv(startTime, endTime, "ALL", null, fileName);
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, string fileName = null)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, AGVSConfigulator.SysConfigs.clsAGVS_Print_Data.SavePath + "Alarm");
            try
            {
                Directory.CreateDirectory(folder);

            }
            catch (Exception)
            {
                folder = Path.GetTempPath();
            }
            var _fileName = fileName is null ? DateTime.Now.ToString("yyyy-MM-dd-HH") + ".csv" : fileName;
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
        public static string AutoSaveTocsv(DateTime startTime, DateTime endTime, string fileName)
        {
            return AutoSaveTocsv(startTime, endTime, "ALL", null, fileName);
        }
        public static string AutoSaveTocsv(DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, string fileName = null)
        {
            string YesterdayDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            var folder = Path.Combine(Environment.CurrentDirectory, AGVSConfigulator.SysConfigs.AutoSendDailyData.SavePath + YesterdayDate);
            var _fileName = fileName is null ? YesterdayDate + ".csv" : fileName;
            Directory.CreateDirectory(folder);
            string FilePath = Path.Combine(folder, "Alarm" + _fileName);
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
        public static async Task SetAlarmCheckedAsync(string eQName, int alarm_code, string checker_name = "", string location = null)
        {
            bool waitAddTimeout = true;
            try
            {
                waitAddTimeout = !await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));
                if (waitAddTimeout)
                    return;
                using (var dbhelper = new AGVSDatabase())
                {
                    var alarms = dbhelper.tables.SystemAlarms.Where(alarm => !alarm.Checked && alarm.Equipment_Name == eQName && alarm.AlarmCode == (int)alarm_code && (location == null ? true : alarm.OccurLocation == location)).ToArray();
                    foreach (var item in alarms)
                    {
                        item.Checked = true;
                        MCSCIMService.AlarmClear((ushort)item.AlarmCode, item.Description);
                    }
                    await dbhelper.SaveChanges();
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if (!waitAddTimeout)
                    semaphoreSlim.Release();
            }
        }
        public static async Task SetAlarmCheckedAsync(string eQName, ALARMS alarm_code, string checker_name = "")
        {
            await SetAlarmCheckedAsync(eQName, (int)alarm_code, checker_name);
        }
        public static async Task SetAlarmCheckedAsync(string eQName, string location, ALARMS alarm_code, string checker_name = "")
        {
            await SetAlarmCheckedAsync(eQName, (int)alarm_code, checker_name, location);
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
            bool waitAddTimeout = true;
            try
            {
                waitAddTimeout = !await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));
                if (waitAddTimeout)
                    return;
                using var agvsDb = new AGVSDatabase();
                using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    foreach (var _alarm in dbhelper._context.SystemAlarms.Where(alarm => alarm.Equipment_Name == name))
                    {
                        _alarm.Checked = true;

                        MCSCIMService.AlarmClear((ushort)_alarm.AlarmCode, _alarm.Description);
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
                if (!waitAddTimeout)
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
            bool waitAddTimeout = true;
            try
            {
                waitAddTimeout = !await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));
                if (waitAddTimeout)
                    return;
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
                if (!waitAddTimeout)
                    semaphoreSlim.Release();
            }

        }

        private static async Task<bool> WaitAlarmTableUnLocked()
        {
            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            while (database.tables.IsAlarmTableLocking())
            {
                await Task.Delay(100);
                if (cts.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Alarm Table is locking, wait for 1 second and TIMEOUT.");
                    return false;
                }
            }
            return true;
        }
    }

}
