using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Vehicle_Control.LogAnalysis.Models
{
    public class clsIOLog : clsLogItemBase
    {
        public enum DIRECTION
        {
            INPUT, OUTPUT
        }
        public DIRECTION IO_Direction { get; set; } = DIRECTION.INPUT;
        public string IO_Name { get; set; } = "";
        public int Value { get; set; } = 0;
    }
}
