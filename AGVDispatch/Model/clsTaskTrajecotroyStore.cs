using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch.Model
{
    /// <summary>
    /// 任務軌跡
    /// </summary>
    public class clsTaskTrajecotroyStore
    {
        [Key]
        public string TaskName { get; set; } = string.Empty;
        public DateTime RecieveTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string DesignatedAGVName { get; set; } = "";

        public double X { get; set; }
        public double Y { get; set; }
        public double Theta { get; set; }

    }
}
