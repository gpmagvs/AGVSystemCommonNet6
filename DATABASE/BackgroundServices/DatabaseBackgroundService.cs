using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using KGSWebAGVSystemAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WebSocketSharp;
using Task = System.Threading.Tasks.Task;
using AGVSystemCommonNet6;

namespace AGVSystemCommonNet6.DATABASE.BackgroundServices
{
    /// <summary>
    /// DatabaseBackgroundService
    /// </summary>
    public class DatabaseBackgroundService : BackgroundService, IDisposable
    {
        private readonly ILogger<DatabaseBackgroundService> _logger;
        private readonly IServiceProvider _services;
        public int cacheTimerInterval { get; set; } = 200;
        public DatabaseBackgroundService(IServiceProvider services, ILogger<DatabaseBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(DatabaseDataSyncWork);
            Task.Run(fetchFinishTasksCallback);
        }

        private async Task fetchFinishTasksCallback()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    await Task.Delay(5000);

                    _stopwatch.Restart();
                    using (var scope = _services.CreateScope())
                    {

                        if (AGVSConfigulator.SysConfigs.BaseOnKGSWebAGVSystem)
                        {
                            WebAGVSystemContext kgDBContext = scope.ServiceProvider.GetRequiredService<WebAGVSystemContext>();
                            List<KGSWebAGVSystemAPI.Models.Task> tasks = kgDBContext.Tasks.AsNoTracking().OrderByDescending(task => task.ReceiveTime).Take(40).ToList();

                            DatabaseCaches.TaskCaches.CompleteTasks = tasks.ToGPMTaskCollection();

                            continue;
                        }

                        var dbContext = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();

                        DatabaseCaches.TaskCaches.CompleteTasks = (await GetTasksInSpecficTimeRange()).Where(task => IsTaskInFinishedState(task))
                                                                                                  .OrderByDescending(task => task.RecieveTime)
                                                                                                  .Take(40).ToList();
                        _stopwatch.Stop();

                        if (_stopwatch.Elapsed.Seconds > 8)
                        {
                            _logger.LogWarning($"{DateTime.Now} DatabaseBackgroundService [fetchFinishTasksCallback] Time Spend Long...: " + _stopwatch.Elapsed.TotalSeconds);
                        }

                        async Task<List<clsTaskDto>> GetTasksInSpecficTimeRange()
                        {
                            DateTime recieveTimeLowerLimit = DateTime.Now.AddDays(-1);
                            return await dbContext.Tasks.AsNoTracking().Where(t => t.RecieveTime >= recieveTimeLowerLimit).ToListAsync();
                        }

                        bool IsTaskInFinishedState(clsTaskDto _task)
                        {
                            return _task.State == AGVDispatch.Messages.TASK_RUN_STATUS.CANCEL ||
                                   _task.State == AGVDispatch.Messages.TASK_RUN_STATUS.FAILURE ||
                                   _task.State == AGVDispatch.Messages.TASK_RUN_STATUS.ACTION_FINISH;
                        }
                    }

                }
                catch (Exception ex)
                {
                }
            }
        }


        private async Task DatabaseDataSyncWork()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            Stopwatch _stopwatch_tasks_query = Stopwatch.StartNew();
            Stopwatch _stopwatch_alarms_query = Stopwatch.StartNew();

            while (true)
            {
                await Task.Delay(cacheTimerInterval);
                _stopwatch.Restart();
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        if (AGVSConfigulator.SysConfigs.BaseOnKGSWebAGVSystem)
                        {
                            WebAGVSystemContext kgDBContext = scope.ServiceProvider.GetRequiredService<WebAGVSystemContext>();
                            var RunningAndWaitingTasks = kgDBContext.ExecutingTasks.AsNoTracking().ToList().ToGPMTaskCollection();
                            DatabaseCaches.TaskCaches.WaitExecuteTasks = RunningAndWaitingTasks.Where(tk => tk.State == AGVDispatch.Messages.TASK_RUN_STATUS.WAIT).ToList();
                            DatabaseCaches.TaskCaches.RunningTasks = RunningAndWaitingTasks.Where(tk => tk.State == AGVDispatch.Messages.TASK_RUN_STATUS.NAVIGATING).ToList();
                            continue;
                        }



                        var dbContext = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();
                        async Task<List<clsTaskDto>> GetTasksInSpecficTimeRange()
                        {
                            _stopwatch_tasks_query.Restart();
                            try
                            {

                                DateTime recieveTimeLowerLimit = DateTime.Now.AddDays(-1);
                                return await dbContext.Tasks.AsNoTracking().Where(t => t.RecieveTime >= recieveTimeLowerLimit).OrderByDescending(t => t.RecieveTime).Take(30).ToListAsync();
                            }
                            finally
                            {
                                _stopwatch_tasks_query.Stop();
                                if (_stopwatch_tasks_query.Elapsed.Seconds > 1)
                                {
                                    _logger.LogWarning($"{DateTime.Now} DatabaseBackgroundService [GetTasksInSpecficTimeRange] Time Spend Long...: " + _stopwatch_tasks_query.Elapsed.TotalSeconds);
                                }
                            }
                        }
                        async Task<List<clsAlarmDto>> GetAlarmsInSpeficTimeRange()
                        {
                            _stopwatch_alarms_query.Restart();
                            try
                            {
                                DateTime recieveTimeLowerLimit = DateTime.Now.AddDays(-1);
                                return await dbContext.SystemAlarms.AsNoTracking().Where(al => al.Time >= recieveTimeLowerLimit).ToListAsync();
                            }
                            finally
                            {
                                _stopwatch_alarms_query.Stop();
                                if (_stopwatch_alarms_query.Elapsed.Seconds > 1)
                                {
                                    _logger.LogWarning($"{DateTime.Now} DatabaseBackgroundService [GetAlarmsInSpeficTimeRange] Time Spend Long...: " + _stopwatch_alarms_query.Elapsed.TotalSeconds);
                                }
                            }

                        }

                        List<clsTaskDto> _TasksForQuery = await GetTasksInSpecficTimeRange();

                        DatabaseCaches.TaskCaches.WaitExecuteTasks = _TasksForQuery.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.WAIT).ToList();

                        DatabaseCaches.TaskCaches.RunningTasks = _TasksForQuery.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.NAVIGATING).ToList();

                        DatabaseCaches.Alarms.UnCheckedAlarms = (await GetAlarmsInSpeficTimeRange()).Where(alarm => !alarm.Checked).ToList();

                        DatabaseCaches.Vehicle.VehicleStates = dbContext.AgvStates.AsNoTracking().ToList();

                        _stopwatch.Stop();

                        if (_stopwatch.Elapsed.Seconds > 1)
                        {
                            _logger.LogWarning($"{DateTime.Now} DatabaseBackgroundService Work Time Long...: " + _stopwatch.Elapsed.TotalSeconds);
                        }

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }
            }

        }

    }

}
