using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.TASK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE.Helpers
{
    public class TaskDatabaseHelper : DBHelperAbstract, IDisposable
    {
        private bool disposedValue;

        public List<clsTaskDto> GetALL()
        {
            var alltasks = dbContext.Tasks.ToList();
            return alltasks;
        }


        public List<clsTaskDto> GetALLInCompletedTask()
        {
            using (var dbhelper = new DbContextHelper(connection_str))
            {
                var incompleteds = dbhelper._context.Set<clsTaskDto>().Where(tsk => tsk.State == TASK_RUN_STATUS.WAIT | tsk.State == TASK_RUN_STATUS.NAVIGATING).OrderByDescending(t => t.RecieveTime).ToList();
                if (incompleteds.Count > 0)
                {
                }
                return incompleteds;
            }
        }

        public List<clsTaskDto> GetALLCompletedTask(int num = 20)
        {
            using (var dbhelper = new DbContextHelper(connection_str))
            {
                TASK_RUN_STATUS[] endTaskSTatus = new TASK_RUN_STATUS[] { TASK_RUN_STATUS.FAILURE, TASK_RUN_STATUS.CANCEL, TASK_RUN_STATUS.ACTION_FINISH, TASK_RUN_STATUS.NO_MISSION };
                var incompleteds = dbhelper._context.Set<clsTaskDto>().Where(tsk => endTaskSTatus.Contains(tsk.State)).OrderByDescending(t => t.RecieveTime).Take(num).ToList();
                return incompleteds;
            }
        }

        /// <summary>
        /// 新增一筆任務資料
        /// </summary>
        /// <param name="taskState"></param>
        /// <returns></returns>
        virtual public int Add(clsTaskDto taskState)
        {
            try
            {
                using (var dbhelper = new DbContextHelper(connection_str))
                {
                    Console.WriteLine($"{JsonConvert.SerializeObject(taskState, Formatting.Indented)}");
                    dbhelper._context.Tasks.Add(taskState);
                    int ret = dbhelper._context.SaveChanges();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool Update(clsTaskDto taskState)
        {
            try
            {
                var task = GetALL().FirstOrDefault(task => task.TaskName == taskState.TaskName);
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
               dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool DeleteTask(string task_name)
        {
            try
            {
                clsTaskDto? taskExist = dbContext.Tasks.FirstOrDefault(tsk => tsk.TaskName == task_name);
                if (taskExist != null)
                {
                    taskExist.State = TASK_RUN_STATUS.CANCEL;
                    taskExist.RecieveTime = DateTime.Now;
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        // ~TaskDatabaseHelper()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }
        public static void TaskQuery(out int count, int currentpage, DateTime startTime, DateTime endTime, string AGV_Name, string TaskName, out List<clsTaskDto> Task)
        {
            count = 0;
            Task = new List<clsTaskDto>();
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _Task = dbhelper._context.Set<clsTaskDto>().Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime
                                    && (AGV_Name == "ALL" ? (true) : (Task.DesignatedAGVName == AGV_Name)) && (TaskName == null ? (true) : (Task.TaskName.Contains(TaskName)))
                );
                count = _Task.Count();
                Task = _Task.Skip((currentpage - 1) * 15).Take(15).ToList();
            };
        }
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string AGV_Name, string TaskName)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, @"d:\\SaveLog");
            string FilePath = Path.Combine(folder, "TaskQuery" + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".csv");
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _Task = dbhelper._context.Set<clsTaskDto>().Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime
                                    && (AGV_Name == "ALL" ? (true) : (Task.DesignatedAGVName == AGV_Name)) && (TaskName == null ? (true) : (Task.TaskName.Contains(TaskName)))
                );

                List<string> list = _Task.Select(Task => $"{Task.RecieveTime},{Task.FinishTime},{Task.TaskName},{Task.StateName},{Task.DesignatedAGVName},{Task.ActionName},{Task.Carrier_ID},{Task.From_Station},{Task.To_Station},{Task.FailureReason}").ToList();
                File.WriteAllLines(FilePath, list, Encoding.UTF8);
            };
            return FilePath;
        }

        public TASK_RUN_STATUS GetTaskStateByID(string taskName)
        {
            try
            {
                using (var dbhelper = new DbContextHelper(connection_str))
                {
                    var taskDto = dbhelper._context.Set<clsTaskDto>().FirstOrDefault(tk => tk.TaskName == taskName);
                    if (taskDto != null)
                        return taskDto.State;
                    else
                        return TASK_RUN_STATUS.WAIT;
                }
            }
            catch (Exception ex)
            {
                return TASK_RUN_STATUS.WAIT;
            }
        }

        public List<clsTaskDto> GetTasksByTimeInterval(DateTime start, DateTime end)
        {
            return dbContext.Tasks.Where(tk=>tk.RecieveTime>=start && tk.FinishTime<=end).ToList();
        }

        public void SetRunningTaskWait()
        {
            foreach (var task in dbContext.Tasks.Where(tsk=>tsk.State== TASK_RUN_STATUS.NAVIGATING))
            {
                task.State = TASK_RUN_STATUS.WAIT;
            }
            dbContext.SaveChanges();

        }
    }
}
