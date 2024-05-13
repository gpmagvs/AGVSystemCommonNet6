using AGVSystemCommonNet6.AGVDispatch;
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
        private Timer _timer;
        private Timer _timer_query_latest_finished_task;
        public int cacheTimerInterval { get; set; } = 200;
        public DatabaseBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(callback, null, 0, cacheTimerInterval);
            _timer_query_latest_finished_task = new Timer(fetchFinishTasksCallback, null, 0, 5000);
            //_ = Task.Run(DoWork);
        }

        private void fetchFinishTasksCallback(object? state)
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();

            using (var scope = _scopeFactory.CreateAsyncScope())
            {
                try
                {

                    using AGVSDbContext context = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();

                    List<clsTaskDto> GetTasksInSpecficTimeRange()
                    {
                        DateTime recieveTimeLowerLimit = DateTime.Now.AddDays(-1);
                        return context.Tasks.AsNoTracking().Where(t => t.RecieveTime >= recieveTimeLowerLimit).ToList();
                    }


                    DatabaseCaches.TaskCaches.CompleteTasks = GetTasksInSpecficTimeRange()
                                                                            .Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.CANCEL || task.State == AGVDispatch.Messages.TASK_RUN_STATUS.ACTION_FINISH)
                                                                            .OrderByDescending(task => task.RecieveTime)
                                                                            .Take(40).ToList();
                    _stopwatch.Stop();

                    if (_stopwatch.Elapsed.Seconds > 1)
                    {
                        Console.WriteLine("DatabaseBackgroundService [fetchFinishTasksCallback] Time Spend Long...: " + _stopwatch.Elapsed.TotalSeconds);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[DatabaseBackgroundService] fetchFinishTasksCallback: Exception" + ex.Message);

                }

            }
        }
      
        private void callback(object? state)
        {
            Stopwatch _stopwatch = Stopwatch.StartNew();
            try
            {
                using (var scope = _scopeFactory.CreateAsyncScope())
                {
                    using AGVSDbContext context = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();
                    // 這裡進行背景數據庫操作或其他業務邏輯
                    //var taskLatest = context.Tasks.AsNoTracking().OrderByDescending(task => task.RecieveTime).Take(40).ToList();

                    List<clsTaskDto> GetTasksInSpecficTimeRange()
                    {
                        DateTime recieveTimeLowerLimit = DateTime.Now.AddDays(-2);
                        return context.Tasks.AsNoTracking().Where(t => t.RecieveTime >= recieveTimeLowerLimit).ToList();
                    }

                    List<clsTaskDto> _TasksForQuery = GetTasksInSpecficTimeRange();

                    DatabaseCaches.TaskCaches.WaitExecuteTasks = _TasksForQuery.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.WAIT).ToList();


                    //DatabaseCaches.TaskCaches.CompleteTasks = taskLatest.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.CANCEL || task.State == AGVDispatch.Messages.TASK_RUN_STATUS.ACTION_FINISH)
                    //                                                    .ToList();

                    DatabaseCaches.TaskCaches.RunningTasks = _TasksForQuery.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.NAVIGATING).ToList();
                    //_stopwatch.Stop();
                    // Console.WriteLine("TaskCaches: " + _stopwatch.Elapsed.ToString());
                    //_stopwatch.Restart();
                    DatabaseCaches.Alarms.UnCheckedAlarms = context.SystemAlarms.AsNoTracking().Where(alarm => !alarm.Checked).ToList();
                    // Console.WriteLine("UnCheckedAlarms: " + _stopwatch.Elapsed.ToString());
                    // _stopwatch.Restart();
                    DatabaseCaches.Vehicle.VehicleStates = context.AgvStates.AsNoTracking().ToList();
                    _stopwatch.Stop();

                    if (_stopwatch.Elapsed.Seconds > 1)
                    {
                        Console.WriteLine("DatabaseBackgroundService Work Time Long...: " + _stopwatch.Elapsed.TotalSeconds);
                    }
                }


                // Console.WriteLine("VehicleStates: " + _stopwatch.Elapsed.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DatabaseBackgroundService] DoWork Exception" + ex.Message);
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
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
