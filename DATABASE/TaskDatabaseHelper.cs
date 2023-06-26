using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.TASK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE
{
    public class TaskDatabaseHelper : IDisposable
    {
        protected readonly string connection_str;
        protected DbContextHelper dbhelper;
        private bool disposedValue;
        public TaskDatabaseHelper()
        {
            this.connection_str = AGVSConfigulator.SysConfigs.DBConnection;
            dbhelper = new DbContextHelper(connection_str);
        }

        public List<clsTaskDto> GetALL()
        {
            var alltasks = dbhelper._context.Tasks.ToList();
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

        public List<clsTaskDto> GetALLCompletedTask()
        {
            using (var dbhelper = new DbContextHelper(connection_str))
            {
                var incompleteds = dbhelper._context.Set<clsTaskDto>().Where(tsk => tsk.State == TASK_RUN_STATUS.ACTION_FINISH | tsk.State == TASK_RUN_STATUS.FAILURE).OrderByDescending(t => t.RecieveTime).ToList();
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
                    dbhelper._context.Set<clsTaskDto>().Add(taskState);
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
                dbhelper._context.SaveChanges();
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
                clsTaskDto? taskExist = dbhelper._context.Set<clsTaskDto>().FirstOrDefault(tsk => tsk.TaskName == task_name);
                if (taskExist != null)
                {
                    dbhelper._context.Set<clsTaskDto>().Remove(taskExist);
                    dbhelper._context.SaveChanges();
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
            dbhelper._context.SaveChanges();
        }
    }
}
