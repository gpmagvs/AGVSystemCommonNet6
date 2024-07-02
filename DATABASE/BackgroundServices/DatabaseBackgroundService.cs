using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AGVSystemCommonNet6.DATABASE.BackgroundServices
{
    /// <summary>
    /// DatabaseBackgroundService
    /// </summary>
    public class DatabaseBackgroundService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public int cacheTimerInterval { get; set; } = 200;
        public DatabaseBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(DatabaseDataSyncWork);
            Task.Run(fetchFinishTasksCallback);
        }

        private async void fetchFinishTasksCallback()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            var scope = _scopeFactory.CreateAsyncScope();
            AGVSDbContext context = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();
            while (true)
            {
                try
                {
                    await Task.Delay(5000);
                    DatabaseCaches.TaskCaches.CompleteTasks = (await GetTasksInSpecficTimeRange()).Where(task => IsTaskInFinishedState(task))
                                                                                                  .OrderByDescending(task => task.RecieveTime)
                                                                                                  .Take(40).ToList();
                    _stopwatch.Stop();

                    if (_stopwatch.Elapsed.Seconds > 8)
                    {
                        Console.WriteLine($"{DateTime.Now} DatabaseBackgroundService [fetchFinishTasksCallback] Time Spend Long...: " + _stopwatch.Elapsed.TotalSeconds);
                    }

                    async Task<List<clsTaskDto>> GetTasksInSpecficTimeRange()
                    {
                        DateTime recieveTimeLowerLimit = DateTime.Now.AddDays(-2);
                        return await context.Tasks.AsNoTracking().Where(t => t.RecieveTime >= recieveTimeLowerLimit).ToListAsync();
                    }

                    bool IsTaskInFinishedState(clsTaskDto _task)
                    {
                        return _task.State == AGVDispatch.Messages.TASK_RUN_STATUS.CANCEL ||
                               _task.State == AGVDispatch.Messages.TASK_RUN_STATUS.FAILURE ||
                               _task.State == AGVDispatch.Messages.TASK_RUN_STATUS.ACTION_FINISH;
                    }
                }
                catch (Exception ex)
                {
                    scope = _scopeFactory.CreateAsyncScope();
                    context = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();
                }
            }
        }

        private async void DatabaseDataSyncWork()
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            var scope = _scopeFactory.CreateAsyncScope();
            AGVSDbContext context = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();

            while (true)
            {
                try
                {
                    await Task.Delay(cacheTimerInterval);
                    async Task<List<clsTaskDto>> GetTasksInSpecficTimeRange()
                    {
                        DateTime recieveTimeLowerLimit = DateTime.Now.AddDays(-2);
                        return await context.Tasks.AsNoTracking().Where(t => t.RecieveTime >= recieveTimeLowerLimit).ToListAsync();
                    }
                    async Task<List<clsAlarmDto>> GetAlarmsInSpeficTimeRange()
                    {
                        DateTime recieveTimeLowerLimit = DateTime.Now.AddDays(-1);
                        return await context.SystemAlarms.AsNoTracking().Where(al => al.Time >= recieveTimeLowerLimit).ToListAsync();
                    }

                    List<clsTaskDto> _TasksForQuery = await GetTasksInSpecficTimeRange();

                    DatabaseCaches.TaskCaches.WaitExecuteTasks = _TasksForQuery.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.WAIT).ToList();

                    DatabaseCaches.TaskCaches.RunningTasks = _TasksForQuery.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.NAVIGATING).ToList();

                    DatabaseCaches.Alarms.UnCheckedAlarms = (await GetAlarmsInSpeficTimeRange()).Where(alarm => !alarm.Checked).ToList();

                    DatabaseCaches.Vehicle.VehicleStates = context.AgvStates.AsNoTracking().ToList();

                    _stopwatch.Stop();

                    if (_stopwatch.Elapsed.Seconds > 1)
                    {
                        Console.WriteLine($"{DateTime.Now} DatabaseBackgroundService Work Time Long...: " + _stopwatch.Elapsed.TotalSeconds);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                    scope = _scopeFactory.CreateAsyncScope();
                    context = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();
                }
            }

        }

        private async Task DoWork()
        {
            while (true)
            {
                await Task.Delay(200);

            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

}
