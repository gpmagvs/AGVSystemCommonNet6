using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.MAP;
using AGVSystemCommonNet6.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using static SQLite.SQLite3;

namespace AGVSystemCommonNet6.DATABASE.Helpers
{
    public class TaskDatabaseHelper : DBHelperAbstract
    {
        private bool disposedValue;
        private DbSet<clsTaskDto> TaskSet => dbhelper._context.Tasks;
        public TaskDatabaseHelper() : base()
        {
        }
        public async Task<List<clsTaskDto>> GetALL()
        {
            return await TaskSet.ToListAsync();
        }


        public List<clsTaskDto> GetALLInCompletedTask(bool notracking = false)
        {
            var _incomplete_tasks = TaskSet.Where(tsk => tsk.State == TASK_RUN_STATUS.WAIT || tsk.State == TASK_RUN_STATUS.NAVIGATING).OrderByDescending(t => t.RecieveTime);
            if (notracking)
                return _incomplete_tasks.AsNoTracking().ToList();
            return _incomplete_tasks.ToList();
        }

        public List<clsTaskDto> GetALLCompletedTask(int num = 20, bool notracking = false)
        {
            TASK_RUN_STATUS[] endTaskSTatus = new TASK_RUN_STATUS[] { TASK_RUN_STATUS.FAILURE, TASK_RUN_STATUS.CANCEL, TASK_RUN_STATUS.ACTION_FINISH, TASK_RUN_STATUS.NO_MISSION };
            var _complete_tasks = TaskSet.Where(tsk => endTaskSTatus.Contains(tsk.State)).OrderByDescending(t => t.RecieveTime).Take(num);
            if (notracking)
                return _complete_tasks.AsNoTracking().ToList();
            return _complete_tasks.ToList();
        }

        /// <summary>
        /// 新增一筆任務資料
        /// </summary>
        /// <param name="taskState"></param>
        /// <returns></returns>
        virtual public async Task<int> Add(clsTaskDto taskState)
        {
            try
            {
                Console.WriteLine($"{JsonConvert.SerializeObject(taskState, Formatting.Indented)}");
                TaskSet.Add(taskState);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> Update(clsTaskDto taskState)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                using (var dbcontext = new DbContextHelper(base.connection_str))
                    try
                    {
                        var task = dbcontext._context.Tasks.FirstOrDefault(task => task.TaskName == taskState.TaskName);
                        if (task == null)
                            return false;
                        var typeA = typeof(clsTaskDto);
                        var typeB = typeof(clsTaskDto);
                        var propertiesB = typeB.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var propertyB in propertiesB)
                        {

                            if (propertyB.Name != "TaskName")
                            {
                                var valueB = propertyB.GetValue(taskState, null);
                                try
                                {
                                    propertyB.SetValue(task, valueB, null);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        dbcontext._context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        AlarmManagerCenter.AddAlarmAsync(ALARMS.Task_Status_Cant_Save_To_Database);
                        transaction.Rollback();
                        return false;
                    }
            }
            return true;

        }


        public bool DeleteTask(string task_name)
        {
            try
            {
                clsTaskDto? taskExist = TaskSet.FirstOrDefault(tsk => tsk.TaskName == task_name);
                if (taskExist != null)
                {
                    taskExist.State = TASK_RUN_STATUS.CANCEL;
                    taskExist.FinishTime = DateTime.Now;
                    taskExist.FailureReason = "User Canceled";
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public (int total, List<clsTaskDto> tasksQueryOut, int CompleteNum, int FailNum, int CancelNum) TaskQuery(TaskQueryCondition conditions)
        {
            Map _useMap = null;
            try
            {
                _useMap = MapManager.LoadMapFromFile(AGVSConfigulator.SysConfigs.PATHES_STORE[SystemConfigs.PATH_ENUMS.CURRENT_MAP_FILE_PATH], out _, false, false);
            }
            catch (Exception ex)
            {
            }
            TASK_RUN_STATUS TaskResult = conditions.TaskResult;
            string AGVName = conditions.AGVName;
            string TaskName = conditions.TaskName;
            string FailReason = conditions.Description;
            DateTime StartTime = conditions.StartTime;
            DateTime EndTime = conditions.EndTime;
            ACTION_TYPE ActionType = conditions.ActionType;
            int Page = conditions.CurrentPage < 1 ? 1 : conditions.CurrentPage;
            int TotalNum = 0;
            int FailNum = 0;
            int CanceledNum = 0;
            int CompletedNum = 0;
            string Source = conditions.Source;
            string Destine = conditions.Destine;
            string _GetDisplayNameOfTagStr(string tagStr)
            {
                if (int.TryParse(tagStr, out int tag))
                {
                    MapPoint _pt = _useMap.Points.Values.FirstOrDefault(pt => pt.TagNumber == tag);
                    if (_pt == null)
                        return tagStr;

                    return _pt.Graph.Display;
                }
                else
                {
                    return tagStr;
                }
            }
            List<clsTaskDto> tasksQueryOut = new();
            using (DbContextHelper dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                IQueryable<clsTaskDto> _TaskQuery = dbhelper._context.Set<clsTaskDto>()
            .OrderByDescending(TK => TK.RecieveTime)
            .Where(Task => Task.RecieveTime >= StartTime &&
                           Task.RecieveTime <= EndTime &&
                           (string.IsNullOrEmpty(AGVName) || Task.DesignatedAGVName == AGVName) &&
                           (string.IsNullOrEmpty(TaskName) || Task.TaskName.Contains(TaskName)) &&
                           (TaskResult == TASK_RUN_STATUS.UNKNOWN || Task.State == TaskResult) &&
                           (ActionType == ACTION_TYPE.Unknown || Task.Action == ActionType));

                // Step 2: Load tasks into memory and apply advanced filtering
                List<clsTaskDto> _TaskList = _TaskQuery.ToList();

                tasksQueryOut = _TaskList.Where(Task =>
                {
                    // 將 Source 分割為多個關鍵字
                    var sourceKeywords = string.IsNullOrEmpty(Source) ? new string[0] : Source.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    // 模糊搜尋條件
                    bool matchesSource = sourceKeywords.Length == 0 ||
                                         sourceKeywords.Any(keyword =>
                                             _GetDisplayNameOfTagStr(Task.From_Station_Tag.ToString()).Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                             _GetDisplayNameOfTagStr(Task.To_Station_Tag.ToString()).Contains(keyword, StringComparison.OrdinalIgnoreCase));

                    // 回傳結果
                    return matchesSource &&
                           (string.IsNullOrEmpty(FailReason) || Task.FailureReason.Contains(FailReason));
                }).ToList();

                // Step 3: Calculate counts
                FailNum = tasksQueryOut.Count(tk => tk.State == TASK_RUN_STATUS.FAILURE);
                CanceledNum = tasksQueryOut.Count(tk => tk.State == TASK_RUN_STATUS.CANCEL);
                CompletedNum = tasksQueryOut.Count(tk => tk.State == TASK_RUN_STATUS.ACTION_FINISH);
                TotalNum = tasksQueryOut.Count;

                // Step 4: Apply pagination
                tasksQueryOut = tasksQueryOut.Skip((Page - 1) * conditions.DataNumberPerPage)
                                             .Take(conditions.DataNumberPerPage)
                                             .ToList();
            };
            return (TotalNum, tasksQueryOut, CompletedNum, FailNum, CanceledNum);
        }


        public (int total, List<clsTaskDto> tasksQueryOut, int CompleteNum, int FailNum, int CancelNum) TaskQueryORIGIN(TaskQueryCondition conditions)
        {
            Map _useMap = null;
            try
            {
                _useMap = MapManager.LoadMapFromFile(AGVSConfigulator.SysConfigs.PATHES_STORE[SystemConfigs.PATH_ENUMS.CURRENT_MAP_FILE_PATH], out _, false, false);
            }
            catch (Exception ex)
            {
            }
            TASK_RUN_STATUS TaskResult = conditions.TaskResult;
            string AGVName = conditions.AGVName;
            string TaskName = conditions.TaskName;
            string FailReason = conditions.Description;
            DateTime StartTime = conditions.StartTime;
            DateTime EndTime = conditions.EndTime;
            ACTION_TYPE ActionType = conditions.ActionType;
            int Page = conditions.CurrentPage < 1 ? 1 : conditions.CurrentPage;
            int TotalNum = 0;
            int FailNum = 0;

            int CanceledNum = 0;
            int CompletedNum = 0;
            string Source = conditions.Source;
            string Destine = conditions.Destine;
            string _GetDisplayNameOfTagStr(string tagStr)
            {
                if (int.TryParse(tagStr, out int tag))
                {
                    MapPoint _pt = _useMap.Points.Values.FirstOrDefault(pt => pt.TagNumber == tag);
                    if (_pt == null)
                        return tagStr;

                    return _pt.Graph.Display;
                }
                else
                {
                    return tagStr;
                }
            }
            List<clsTaskDto> tasksQueryOut = new();
            using (DbContextHelper dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                IQueryable<clsTaskDto> _Task = dbhelper._context.Set<clsTaskDto>().OrderByDescending(TK => TK.RecieveTime)
                                                                                  .Where(Task => Task.RecieveTime >= StartTime &&
                                                                                                 Task.RecieveTime <= EndTime &&
                                                                                                 (string.IsNullOrEmpty(AGVName) || (Task.DesignatedAGVName == AGVName)) &&
                                                                                                 (string.IsNullOrEmpty(TaskName) || Task.TaskName.Contains(TaskName)) &&
                                                                                                 (TaskResult == TASK_RUN_STATUS.UNKNOWN || Task.State == TaskResult) &&
                                                                                                 (ActionType == ACTION_TYPE.Unknown || Task.Action == ActionType) &&
                                                                                                 (string.IsNullOrEmpty(Source) || Task.From_Station_Display.Contains(Source)) &&
                                                                                                 (string.IsNullOrEmpty(Destine) || Task.From_Station_Display.Contains(FailReason)) &&
                                                                                                 (string.IsNullOrEmpty(FailReason) || Task.FailureReason.Contains(FailReason))
                                                                                                 );
                FailNum = _Task.Count(tk => tk.State == TASK_RUN_STATUS.FAILURE);
                CanceledNum = _Task.Count(tk => tk.State == TASK_RUN_STATUS.CANCEL);
                CompletedNum = _Task.Count(tk => tk.State == TASK_RUN_STATUS.ACTION_FINISH);

                TotalNum = _Task.Count();
                tasksQueryOut = _Task.Skip((Page - 1) * conditions.DataNumberPerPage)
                                     .Take(conditions.DataNumberPerPage)
                                     .ToList();
            };
            return (TotalNum, tasksQueryOut, CompletedNum, FailNum, CanceledNum);
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, TASK_RUN_STATUS Result = TASK_RUN_STATUS.UNKNOWN, string fileName = null)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, AGVSConfigulator.SysConfigs.clsAGVS_Print_Data.SavePath + "Task");
            var _fileName = fileName is null ? DateTime.Now.ToString("yyyy-MM-dd-HH") + ".csv" : fileName;
            TASK_RUN_STATUS state_query = Result;
            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception)
            {
                folder = Path.GetTempPath();
            }

            string FilePath = Path.Combine(folder, "TaskQuery_" + _fileName);
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                List<clsTaskDto> _Tasks = dbhelper._context.Set<clsTaskDto>().Where
                (Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime &&
                (string.IsNullOrEmpty(AGV_Name) || (Task.DesignatedAGVName == AGV_Name)) &&
                state_query == TASK_RUN_STATUS.UNKNOWN || Task.State == state_query &&
                (string.IsNullOrEmpty(TaskName) || (Task.TaskName.Contains(TaskName)))).ToList();
                _Tasks = OrderDataRebuild(_Tasks, setCanceledAsFailure: false);
                WirteTaskQueryResultToFile(FilePath, _Tasks);
            };
            return FilePath;
        }
        public static string ExportSpeficDateHistoryToDestine(DateTime date)
        {
            DateTime queryStartTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime queryEndTime = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

            string Date = date.ToString("yyyy-MM-dd");
            string folder = Path.Combine(AGVSConfigulator.SysConfigs.AutoSendDailyData.SavePath, Date);
            string _fileName = Date + ".csv";
            Directory.CreateDirectory(folder);
            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException($"無法創建資料夾({folder})");
            string FilePath = Path.Combine(folder, "Task" + _fileName);
            List<clsTaskDto> _Tasks = new List<clsTaskDto>();
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                _Tasks = dbhelper._context.Set<clsTaskDto>().Where(Task => Task.RecieveTime >= queryStartTime && Task.RecieveTime <= queryEndTime).ToList();
            };
            _Tasks = OrderDataRebuild(_Tasks, setCanceledAsFailure: true);
            WirteTaskQueryResultToFile(FilePath, _Tasks);
            return FilePath;
        }
        public static string AutoExportYesterdayHistoryToDestine()
        {
            DateTime yesterDay = DateTime.Now.AddDays(-1);
            return ExportSpeficDateHistoryToDestine(yesterDay);
        }
        private static List<clsTaskDto> OrderDataRebuild(List<clsTaskDto> _Tasks, bool setCanceledAsFailure = false)
        {
            Map _useMap = null;
            try
            {
                _useMap = MapManager.LoadMapFromFile(AGVSConfigulator.SysConfigs.PATHES_STORE[SystemConfigs.PATH_ENUMS.CURRENT_MAP_FILE_PATH], out _, false, false);
            }
            catch (Exception ex)
            {
            }
            _Tasks.ForEach(orderState =>
            {
                if (orderState.State == TASK_RUN_STATUS.CANCEL && setCanceledAsFailure)
                {
                    orderState.State = TASK_RUN_STATUS.FAILURE;
                }
                if (orderState.Carrier_ID == "-1")
                    orderState.Carrier_ID = "";

                if (orderState.From_Slot == "-1")
                    orderState.From_Slot = "0";
                if (orderState.To_Slot == "-1")
                    orderState.To_Slot = "0";

                if (orderState.DispatcherName.ToLower() == "vms_idle")
                    orderState.DispatcherName = "";

                if (orderState.StartTime == DateTime.MinValue)
                    orderState.StartTime = orderState.RecieveTime;

                if (orderState.FinishTime == DateTime.MinValue)
                    orderState.FinishTime = orderState.RecieveTime;

                if (!string.IsNullOrEmpty(orderState.FailureReason))
                    orderState.FailureReason = CheckAndRebuildFailReason(orderState.FailureReason);

                if (_useMap != null)
                {
                    if (orderState.From_Station != "0" && orderState.From_Station != "-1")
                        orderState.From_Station_Display = _GetDisplayNameOfTagStr(orderState.From_Station);
                    if (orderState.To_Station != "0" && orderState.To_Station != "-1")
                        orderState.To_Station_Display = _GetDisplayNameOfTagStr(orderState.To_Station);

                    string _GetDisplayNameOfTagStr(string tagStr)
                    {
                        if (int.TryParse(tagStr, out int tag))
                        {
                            MapPoint _pt = _useMap.Points.Values.FirstOrDefault(pt => pt.TagNumber == tag);
                            if (_pt == null)
                                return tagStr;

                            return _pt.Graph.Display;
                        }
                        else
                        {
                            return tagStr;
                        }
                    }
                }

            });
            return _Tasks;



        }
        public static string CheckAndRebuildFailReason(string failureReason)
        {
            //[1063] UNLOAD_BUT_AGV_NO_CARGO_MOUNTED(UNLOAD_BUT_AGV_NO_CARGO_MOUNTED)
            if (!failureReason.Contains("[") && !failureReason.Contains("]"))
                return failureReason;
            //[123
            string strAlcode = failureReason.Split(']')[0].Replace("[", "");
            string originalAlarmEN = failureReason.Split('(').Last().Replace(")", "");

            string originalAlarmZh = failureReason.Replace($"[{strAlcode}]", "").Replace($"({originalAlarmEN})", "");

            if (originalAlarmEN.Trim() != originalAlarmZh.Trim())
                return failureReason;
            //string originalAlarmZh = failureReason.Split(' ').Last().Split("(").First();

            if (!int.TryParse(strAlcode, out int alarmCode))
                return failureReason;

            if (!AlarmManagerCenter.AlarmCodes.TryGetValue((ALARMS)alarmCode, out var alarmCodeDto))
                return failureReason;

            if (alarmCodeDto.Description_En.ToUpper() != originalAlarmEN.Replace("_", " ").ToUpper())
                return failureReason;

            string newFailReason = $"[{alarmCode}] {alarmCodeDto.Description}";
            Console.WriteLine($"New Fail Reason build {newFailReason} (Origin: {newFailReason})");
            return newFailReason;
        }

        private static void WirteTaskQueryResultToFile(string FilePath, List<clsTaskDto> Tasks)
        {
            Tasks = Tasks.OrderByDescending(tk => tk.RecieveTime).ToList();
            Map _useMap = null;
            try
            {
                _useMap = MapManager.LoadMapFromFile(AGVSConfigulator.SysConfigs.PATHES_STORE[SystemConfigs.PATH_ENUMS.CURRENT_MAP_FILE_PATH], out _, false, false);
            }
            catch (Exception ex)
            {
            }
            List<string> list = new List<string> {
                "任務名稱," +
                "任務狀態," +
                "接收時間," +
                "起始站點," +
                "目的站點," +
                "起始Port," +
                "目的Port," +
                "任務描述," +
                "執行AGV," +
                "CSTID," +
                "開始時間," +
                "結束時間," +
                "花費時間," +
                "任務距離," +
                "取料時間," +
                "放料時間," +
                "起始位置," +
                "使用者ID," +
                "取消任務使用者ID," +
                "失敗原因,"+
                "Tag歷史," +
                "任務駐留時間(sec),"+
                "任務距離(m)"};

            list.AddRange(Tasks.Select(Task =>
            $"{Task.TaskName}," +
            $"{Task.StateName}," +
            $"{Task.RecieveTime}," +
            $"{Task.From_Station_Display.Replace(",", "_")}," +
            $"{Task.To_Station_Display.Replace(",", "_")}," +
            $"{Task.From_Slot}," +
            $"{Task.To_Slot}," +
            $"{Task.ActionName}," +
            $"{Task.DesignatedAGVName}," +
            $"{Task.Carrier_ID}," + //CSTID
            $"{Task.StartTime}," +  //開始時間
            $"{Task.FinishTime}," + //結束時間
            $"{TimeSpan.FromSeconds((Task.FinishTime - Task.StartTime).TotalSeconds).ToString()}," +//花費時間
            $"{Task.TotalMileage}," +
            $"{(Task.UnloadTime == DateTime.MinValue ? "" : Task.UnloadTime)}," + //取料時間
            $"{(Task.LoadTime == DateTime.MinValue ? "" : Task.LoadTime)}," +//放料時間
            $"{_GetStationNameByTag(Task.StartLocationTag).Replace(",", "_")}," +//起始位置
            $"{(Task.DispatcherName.ToLower() == "vms_idle" ? "" : Task.DispatcherName)}," +
            $"{(Task.State == TASK_RUN_STATUS.CANCEL ? Task.DesignatedAGVName : "")}," +
            $"{_GetFailReason(Task.State, Task.FailureReason)}," +
            $"{Task.TagsTracking}," +//Tag歷史
            $"{GetTaskQueuedDurationInSeconds(Task).ToString()}," + //任務駐留時間
            $"{Task.TotalMileage * 1000.0}"//TotalMileage_meter
            ));

            File.WriteAllLines(FilePath, list, Encoding.UTF8);

            string _GetFailReason(TASK_RUN_STATUS taskState, string failReason)
            {

                if (taskState != TASK_RUN_STATUS.ACTION_FINISH && string.IsNullOrEmpty(failReason))
                    return $"[{(int)ALARMS.SYSTEM_ERROR}] {ALARMS.SYSTEM_ERROR.ToString()}(Fail Reason Empty)";

                if (failReason == null || failReason == "")
                    return "";

                if (failReason.Contains(","))
                    return failReason.Split(",")[0];
                else if (failReason.Contains(";"))
                    return failReason.Split(";")[0];
                else
                    return failReason;
            }

            string _GetStationNameByTag(int tag)
            {
                if (_useMap == null)
                    return tag.ToString();
                MapPoint mapPt = _useMap.Points.Values.FirstOrDefault(pt => pt.TagNumber == tag);
                if (mapPt == null)
                    return tag.ToString();
                return mapPt.Graph.Display;
            }

            static double GetTaskQueuedDurationInSeconds(clsTaskDto Task)
            {
                if (Task.currentProgress == VehicleMovementStage.Not_Start_Yet)
                    return (Task.FinishTime - Task.RecieveTime).TotalSeconds;
                return (Task.StartTime - Task.RecieveTime).TotalSeconds;
            }
        }


        public async Task<TASK_RUN_STATUS> GetTaskStateByID(string taskName)
        {
            try
            {
                await Task.Delay(100);
                var taskDto = TaskSet.Where(tk => tk.TaskName == taskName).AsNoTracking().FirstOrDefault();
                if (taskDto != null)
                    return taskDto.State;
                else
                    return TASK_RUN_STATUS.WAIT;
            }
            catch (Exception ex)
            {
                return TASK_RUN_STATUS.WAIT;
            }
        }


        public async Task<ACTION_TYPE> GetTaskActionTypeByID(string taskName)
        {
            try
            {
                await Task.Delay(100);
                var taskDto = TaskSet.Where(tk => tk.TaskName == taskName).AsNoTracking().FirstOrDefault();
                if (taskDto != null)
                    return taskDto.Action;
                else
                    return ACTION_TYPE.Unknown;
            }
            catch (Exception ex)
            {
                return ACTION_TYPE.Unknown;
            }
        }
        public List<clsTaskDto> GetTasksByTimeInterval(DateTime start, DateTime end, string? taskIDFileter = "")
        {
            bool taskIDFilterRequired = !string.IsNullOrEmpty(taskIDFileter);
            string? _idFilterLowwer = taskIDFileter?.ToLower();
            return TaskSet.Where(tk => tk.RecieveTime >= start && tk.FinishTime <= end && (taskIDFilterRequired ? tk.TaskName.ToLower().Contains(_idFilterLowwer) : true)).ToList();
        }

        public void SetRunningTaskWait()
        {
            try
            {
                foreach (var task in TaskSet.Where(tsk => tsk.State == TASK_RUN_STATUS.NAVIGATING))
                {
                    task.State = TASK_RUN_STATUS.FAILURE;
                }
                dbContext.SaveChanges();

            }
            catch (SqliteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
