using AGVSystemCommonNet6.DATABASE;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;

namespace AGVSystemCommonNet6.DATABASE.BackgroundServices
{
    /// <summary>
    /// DatabaseBackgroundService
    /// </summary>
    public class DatabaseBackgroundService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public DatabaseBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 設置定時器，每隔 10 秒執行一次
            _timer = new Timer(async _ => await DoWork(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(150));
            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    // 從 DI 容器中獲取 DbContext 或其他服務
                    AGVSDbContext context = scope.ServiceProvider.GetRequiredService<AGVSDbContext>();
                    // 這裡進行背景數據庫操作或其他業務邏輯
                    var customerCount = context.Tasks.Count();
                    DatabaseCaches.TaskCaches.WaitExecuteTasks = await context.Tasks.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.WAIT).AsNoTracking().ToListAsync();
                    DatabaseCaches.TaskCaches.CompleteTasks = await context.Tasks.OrderByDescending(task => task.RecieveTime)
                                                                           .Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.CANCEL || task.State == AGVDispatch.Messages.TASK_RUN_STATUS.ACTION_FINISH)
                                                                           .Take(50).AsNoTracking().ToListAsync();
                    DatabaseCaches.TaskCaches.RunningTasks = await context.Tasks.Where(task => task.State == AGVDispatch.Messages.TASK_RUN_STATUS.NAVIGATING).AsNoTracking().ToListAsync();

                    DatabaseCaches.Alarms.UnCheckedAlarms = await context.SystemAlarms.Where(alarm => !alarm.Checked).AsNoTracking().ToListAsync();

                    DatabaseCaches.Vehicle.VehicleStates = await context.AgvStates.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DatabaseBackgroundService] DoWork Exception" + ex.Message);
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
