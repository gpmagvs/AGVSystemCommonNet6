using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Sys
{
    public static class ProcessTools
    {
        public static bool IsProcessRunning(string processName, out List<int> PIDList)
        {
            Console.WriteLine($"Start Check Process '{processName}' is running or not");
            PIDList = new List<int>();
            Process[] processCnt = Process.GetProcessesByName(processName);

            int _currentProcessId = Process.GetCurrentProcess().Id;
            PIDList = processCnt.Where(P => P.Id != _currentProcessId)
                                .Select(p => p.Id).ToList();
            return processCnt.Length > 1;
        }
    }
}
