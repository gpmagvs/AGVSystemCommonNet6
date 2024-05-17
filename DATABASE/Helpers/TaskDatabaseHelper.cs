using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
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

        public void TaskQuery(out int count, int currentpage, DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, string Result, string actionType, out List<clsTaskDto> Task)
        {
            TASK_RUN_STATUS state_query = 0;
            if (Result == "完成")
                state_query = TASK_RUN_STATUS.ACTION_FINISH;
            if (Result == "失敗")
                state_query = TASK_RUN_STATUS.FAILURE;
            if (Result == "取消")
                state_query = TASK_RUN_STATUS.CANCEL;


            ACTION_TYPE action_type_query = ACTION_TYPE.None;
            if (actionType == "移動")
                action_type_query = ACTION_TYPE.None;
            if (actionType == "放貨")
                action_type_query = ACTION_TYPE.Load;
            if (actionType == "取貨")
                action_type_query = ACTION_TYPE.Unload;
            if (actionType == "充電")
                action_type_query = ACTION_TYPE.Charge;
            if (actionType == "搬運")
                action_type_query = ACTION_TYPE.Carry;
            if (actionType == "量測")
                action_type_query = ACTION_TYPE.Measure;
            if (actionType == "交換電池")
                action_type_query = ACTION_TYPE.ExchangeBattery;

            count = 0;
            Task = new List<clsTaskDto>();
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _Task = dbhelper._context.Set<clsTaskDto>().OrderByDescending(TK => TK.FinishTime).Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime
                                    && (AGV_Name == "ALL" ? (true) : (Task.DesignatedAGVName == AGV_Name)) && (TaskName == null ? (true) : (Task.TaskName.Contains(TaskName)))
                                    && (Result == "ALL" ? true : Task.State == state_query)
                                    && (actionType == "ALL" ? true : Task.Action == action_type_query)
                );
                count = _Task.Count();
                Task = _Task.Skip((currentpage - 1) * 19).Take(19).ToList();
            };
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string AGV_Name, string TaskName)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, @"SaveLog\\Task");
            Directory.CreateDirectory(folder);
            string FilePath = Path.Combine(folder, "TaskQuery_" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".csv");
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _Task = dbhelper._context.Set<clsTaskDto>().Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime
                                    && (AGV_Name == "ALL" ? (true) : (Task.DesignatedAGVName == AGV_Name)) && (TaskName == null ? (true) : (Task.TaskName.Contains(TaskName)))
                );

                List<string> list = _Task.Select(Task => $"{Task.RecieveTime},{Task.StartTime},{Task.FinishTime},{(Task.FinishTime - Task.StartTime).TotalSeconds},{Task.TaskName},{Task.StateName},{Task.DesignatedAGVName},{Task.ActionName},{Task.Carrier_ID},{Task.From_Station},{Task.From_Station_Display},{Task.To_Station},{Task.To_Station_Display},{Task.FailureReason}").ToList();
                File.WriteAllLines(FilePath, list, Encoding.UTF8);
            };
            return FilePath;
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
                    task.State = TASK_RUN_STATUS.WAIT;
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
