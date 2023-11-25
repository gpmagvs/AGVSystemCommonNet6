using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm.VMS_ALARM;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Log;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm
{
    public class AlarmManagerCenter
    {
        public static string ALARM_CODE_FILE_PATH = @"C:\AGVS\AGVS_AlarmCodes.json";
        public static Dictionary<ALARMS, clsAlarmCode> AlarmCodes = new Dictionary<ALARMS, clsAlarmCode>();
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
                var alarmExist = dbhelper.tables.SystemAlarms.FirstOrDefault(alarm => alarm.Time == alarmDto.Time);
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
        public static async Task AddAlarmAsync(clsAlarmDto alarmDto)
        {
            if (!Initialized)
                Initialize();

            try
            {
                using var db = new AGVSDatabase();
                db.tables.SystemAlarms.Add(alarmDto);
                await db.SaveChanges();
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(async () =>
                 {
                     await Task.Delay(100);
                     await AddAlarmAsync(alarmDto);
                 });
            }
        }

        public static async Task AddAlarmAsync(ALARMS alarm, ALARM_SOURCE source = ALARM_SOURCE.AGVS, ALARM_LEVEL level = ALARM_LEVEL.ALARM, string Equipment_Name = "", string location = "", string taskName = "")
        {
            if (!Initialized)
                Initialize();

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
                    Source = source
                };
                await AddAlarmAsync(alarmDto);
            }
            catch (Exception ex)
            {
                LOG.ERROR("AddAlarmAsync", ex);
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

        public static void ResetAlarm(clsAlarmDto alarm, bool resetAllSameCode = false)
        {
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                if (resetAllSameCode)
                {
                    var alarms_same_code = dbhelper._context.SystemAlarms.Where(_alarm => _alarm.AlarmCode == alarm.AlarmCode && _alarm.Checked == false);
                    foreach (clsAlarmDto? alarm_ in alarms_same_code)
                    {
                        alarm_.Checked = true;
                        alarm_.ResetAalrmMemberName = alarm.ResetAalrmMemberName;
                        UpdateAlarmAsync(alarm_);
                    }
                }
                else
                {
                    if (dbhelper._context.Set<clsAlarmDto>().FirstOrDefault(a => a == alarm && a.Checked == false) != null)
                    {
                        alarm.Checked = true;
                        UpdateAlarmAsync(alarm);
                    }
                }
            }

        }

        public static void RemoveAlarm(clsAlarmDto alarmDto)
        {
            try
            {
                using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    dbhelper._context.Set<clsAlarmDto>().Remove(alarmDto);
                    dbhelper._context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            };
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string AGV_Name, string TaskName)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, @"SaveLog\\Alarm");
            Directory.CreateDirectory(folder);
            string FilePath = Path.Combine(folder, "AlarmQuery_" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".csv");
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _alarms = dbhelper._context.Set<clsAlarmDto>().Where(alarm => alarm.Time >= startTime && alarm.Time <= endTime
                                    && (AGV_Name == "ALL" ? (true) : (alarm.Equipment_Name == AGV_Name)) && (TaskName == null ? (true) : (alarm.Task_Name.Contains(TaskName)))
                );

                List<string> list = _alarms.Select(alarm => $"{alarm.Time},{alarm.Task_Name},{alarm.Equipment_Name},,{alarm.AlarmCode}{alarm.Description_En},{alarm.Description_Zh},{alarm.Duration},{alarm.OccurLocation},{alarm.Level}").ToList();
                File.WriteAllLines(FilePath, list, Encoding.UTF8);
            };
            return FilePath;
        }
        public static async Task SetAlarmCheckedAsync(string eQName, int alarm_code, string checker_name = "")
        {
            try
            {

                using (var dbhelper = new AGVSDatabase())
                {
                    var alarms = dbhelper.tables.SystemAlarms.Where(alarm => alarm.Equipment_Name == eQName && alarm.AlarmCode == (int)alarm_code).ToArray();
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
