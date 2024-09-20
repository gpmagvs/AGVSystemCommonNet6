using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.MAP;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public void TaskQuery(out int count, int currentpage, DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, string Result, string actionType, string failurereason, out List<clsTaskDto> Task)
        {
            TASK_RUN_STATUS state_query = 0;
            if (Result == "完成" || Result == "Completed")
                state_query = TASK_RUN_STATUS.ACTION_FINISH;
            if (Result == "失敗" || Result == "Fail")
                state_query = TASK_RUN_STATUS.FAILURE;
            if (Result == "取消" || Result == "Cancel")
                state_query = TASK_RUN_STATUS.CANCEL;


            ACTION_TYPE action_type_query = ACTION_TYPE.None;
            if (actionType == "移動" || actionType == "Move")
                action_type_query = ACTION_TYPE.None;
            if (actionType == "放貨" || actionType == "UnLoad")
                action_type_query = ACTION_TYPE.Load;
            if (actionType == "取貨" || actionType == "Load")
                action_type_query = ACTION_TYPE.Unload;
            if (actionType == "充電" || actionType == "Charge")
                action_type_query = ACTION_TYPE.Charge;
            if (actionType == "搬運" || actionType == "Transfer")
                action_type_query = ACTION_TYPE.Carry;
            if (actionType == "量測" || actionType == "Measure")
                action_type_query = ACTION_TYPE.Measure;
            if (actionType == "交換電池" || actionType == "Exchange Battrey")
                action_type_query = ACTION_TYPE.ExchangeBattery;

            count = 0;
            Task = new List<clsTaskDto>();
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _Task = dbhelper._context.Set<clsTaskDto>().OrderByDescending(TK => TK.RecieveTime).Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime
                                    && (AGV_Name == "ALL" ? (true) : (Task.DesignatedAGVName == AGV_Name)) && (TaskName == null ? (true) : (Task.TaskName.Contains(TaskName)))
                                    && (Result == "ALL" ? true : Task.State == state_query)
                                    && (actionType == "ALL" ? true : Task.Action == action_type_query)
                                    && (failurereason == null ? (true) : (Task.FailureReason.Contains(failurereason)))
                /*(failurereason == "ALL" ? (true) :(Task.FailureReason.Contains(failurereason)))*/
                /*(failurereason == "ALL" ? true :Task.FailureReason == failurereason)*/
                );
                count = _Task.Count();
                Task = _Task.Skip((currentpage - 1) * 19).Take(19).ToList();
            };
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, string fileName = null)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, AGVSConfigulator.SysConfigs.clsAGVS_Print_Data.SavePath + "Task");
            var _fileName = fileName is null ? DateTime.Now.ToString("yyyy-MM-dd-HH") + ".csv" : fileName;
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
                List<clsTaskDto> _Tasks = dbhelper._context.Set<clsTaskDto>().Where(Task => Task.RecieveTime >= startTime &&
                                                                                            Task.RecieveTime <= endTime &&
                                                                                            (AGV_Name == "ALL" ? (true) : (Task.DesignatedAGVName == AGV_Name)) &&
                                                                                            (TaskName == null ? (true) : (Task.TaskName.Contains(TaskName))))
                                                                             .ToList();
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
                if (orderState.State == TASK_RUN_STATUS.CANCEL)
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
        private static void WirteTaskQueryResultToFile(string FilePath, List<clsTaskDto> Tasks)
        {
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
                "失敗原因" };

            list.AddRange(Tasks.Select(Task =>
            $"{Task.TaskName}," +
            $"{Task.StateName}," +
            $"{Task.RecieveTime}," +
            $"{Task.From_Station_Display}," +
            $"{Task.To_Station_Display}," +
            $"{Task.From_Slot}," +
            $"{Task.To_Slot}," +
            $"{Task.ActionName}," +
            $"{Task.DesignatedAGVName}," +
            $"{Task.Carrier_ID}," +
            $"{Task.StartTime}," +
            $"{Task.FinishTime}," +
            $"{TimeSpan.FromSeconds((Task.FinishTime - Task.StartTime).TotalSeconds).ToString()}," +
            $"{Task.TotalMileage}," +
            $"{(Task.UnloadTime == DateTime.MinValue ? "" : Task.UnloadTime)}," +
            $"{(Task.LoadTime == DateTime.MinValue ? "" : Task.LoadTime)}," +
            $"{_GetStationNameByTag(Task.StartLocationTag)}," +
            $"{(Task.DispatcherName.ToLower() == "vms_idle" ? "" : Task.DispatcherName)}," +
            $"{(Task.State == TASK_RUN_STATUS.CANCEL ? Task.DesignatedAGVName : "")}," +
            $"{_GetFailReason(Task.FailureReason)}"));
            File.WriteAllLines(FilePath, list, Encoding.UTF8);

            string _GetFailReason(string failReason)
            {
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
        public List<clsTaskDto> GetTasksByTimeInterval(DateTime start, DateTime end)
        {
            return TaskSet.Where(tk => tk.RecieveTime >= start && tk.FinishTime <= end).ToList();
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
