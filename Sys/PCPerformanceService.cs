﻿using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace AGVSystemCommonNet6.Sys
{
    public class PCPerformanceService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var process = Process.GetCurrentProcess();

            LogManager.Setup().LoadConfigurationFromFile().GetCurrentClassLogger();
            var logger = LogManager.GetLogger("PCPerformanceLog");
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    // 記憶體用量 (工作集)
                    var memoryUsage = Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024; // 以 MB 為單位
                                                                                              // 初始化 PerformanceCounter 用於計算 CPU 使用率
                    using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                    using (var processCpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName))
                    {
                        // 讓計數器有時間初始化
                        cpuCounter.NextValue();
                        processCpuCounter.NextValue();
                        await Task.Delay(1000);
                        // 獲取系統整體 CPU 使用率
                        var systemCpuUsage = cpuCounter.NextValue();

                        // 獲取當前程序 CPU 使用率
                        var processCpuUsage = processCpuCounter.NextValue() / Environment.ProcessorCount;

                        logger.Info($"當前程序記憶體用量${memoryUsage}MB$當前程序CPU使用率${processCpuUsage}%$系統整體CPU使用率${systemCpuUsage}%");

                    }

                }
            });
        }
    }
}
