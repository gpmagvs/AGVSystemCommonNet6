using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Sys
{
    public class EnvironmentVariables
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        public static void AddUserVariable(string variableName, string variableValue)
        {
            try
            {
                string value = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(value))
                    Environment.SetEnvironmentVariable(variableName, variableValue, EnvironmentVariableTarget.User);
                //// 讀取並驗證環境變數是否添加成功
                //string value = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User);
                //Console.WriteLine($"Environment variable '{variableName}' is set to: {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Set Environment variable '{variableName}' as {variableValue} fail. {ex.Message}");
            }
        }
    }
}
