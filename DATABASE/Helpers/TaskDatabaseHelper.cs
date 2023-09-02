using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.TASK;
using AGVSystemCommonNet6.Tools.Database;
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
            var _incomplete_tasks = TaskSet.Where(tsk => tsk.State == TASK_RUN_STATUS.WAIT | tsk.State == TASK_RUN_STATUS.NAVIGATING).OrderByDescending(t => t.RecieveTime);
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
        virtual public int Add(clsTaskDto taskState)
        {
            try
            {
                Console.WriteLine($"{JsonConvert.SerializeObject(taskState, Formatting.Indented)}");
                TaskSet.Add(taskState);
                return SaveChanges();
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
                        AlarmManagerCenter.AddAlarm(ALARMS.Task_Status_Cant_Save_To_Database);
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

        public void TaskQuery(out int count, int currentpage, DateTime startTime, DateTime endTime, string AGV_Name, out List<clsTaskDto> Task)
        {

            Task = new List<clsTaskDto>();
            if (AGV_Name == "ALL")
            {
                count = TaskSet.Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime).Count();
                int skipindex = (currentpage - 1) * 10;
                Task = dbhelper._context.Set<clsTaskDto>().Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime).Skip(skipindex).Take(10).ToList();

            }
            else
            {
                count = TaskSet.Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime).Count();
                int skipindex = (currentpage - 1) * 10;
                Task = TaskSet.Where(Task => Task.RecieveTime >= startTime && Task.RecieveTime <= endTime && Task.DesignatedAGVName == AGV_Name).Skip(skipindex).Take(10).ToList();
            }
        }

        public async Task< TASK_RUN_STATUS> GetTaskStateByID(string taskName)
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

        public List<clsTaskDto> GetTasksByTimeInterval(DateTime start, DateTime end)
        {
            return TaskSet.Where(tk => tk.RecieveTime >= start && tk.FinishTime <= end).ToList();
        }

        public void SetRunningTaskWait()
        {
            foreach (var task in TaskSet.Where(tsk => tsk.State == TASK_RUN_STATUS.NAVIGATING))
            {
                task.State = TASK_RUN_STATUS.WAIT;
            }
            dbContext.SaveChanges();

        }
    }
}
