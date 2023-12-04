using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Vehicle_Control.LogAnalysis.Models
{
    public class clsLogItemBase
    {
        public DateTime Time { get; set; }
        public string Message { get; set; } = "";

        public string TimeStr => Time.ToString("yyyy/MM/dd HH:mm:ss.ffff");

    }
}
