using Microsoft.Extensions.Hosting;
using NLog;
using System.Diagnostics;
using System.Management;

namespace AGVSystemCommonNet6.Sys
{
    public class PCPerformanceService : BackgroundService
    {
        public event EventHandler<Exception> OnExceptionHappend;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger logger = LogManager.GetLogger("PCPerformanceLog");
            try
            {
                long totalMemory = GetTotalPhysicalMemory() / 1024 / 1024;
                var process = Process.GetCurrentProcess();
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            // 記憶體用量 (工作集)
                            long memoryUsage = Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024; // 以 MB 為單位
                                                                                                       // 獲取系統總物理記憶體 (以 MB 為單位)
                            using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                            using (var processCpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName))
                            {
                                double memoeryUsageRate = Math.Round((double)memoryUsage / (double)totalMemory * 100.0, 2);

                                // 讓計數器有時間初始化
                                cpuCounter.NextValue();
                                processCpuCounter.NextValue();
                                await Task.Delay(1000);
                                // 獲取系統整體 CPU 使用率
                                float systemCpuUsage = (float)Math.Round(cpuCounter.NextValue(), 2);
                                // 獲取當前程序 CPU 使用率
                                float processCpuUsage = (float)Math.Round(processCpuCounter.NextValue() / Environment.ProcessorCount, 2);
                                logger.Info($"當前程序記憶體用量|{memoryUsage}MB/{totalMemory}MB({memoeryUsageRate}%)|當前程序CPU使用率|{processCpuUsage}%|系統整體CPU使用率${systemCpuUsage}%");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            await Task.Delay(1000);
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static long GetTotalPhysicalMemory()
        {
            long totalMemory = 0;

            // 使用 WMI 查询系统总物理内存
            using (var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem"))
            {
                foreach (var result in searcher.Get())
                {
                    totalMemory = Convert.ToInt64(result["TotalVisibleMemorySize"]);
                }
            }

            // 返回以 KB 为单位的内存总量
            return totalMemory * 1024; // 转换为字节
        }
    }
}
