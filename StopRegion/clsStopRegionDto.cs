using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.StopRegion
{
    public class clsStopRegionDto
    {
        [Key]
        public DateTime StartTime { get; set; }
        public string AGVName { get; set; } = "";
        public DateTime EndTime { get; set; }
        public string RegionName { get; set; } = "";
        public MAIN_STATUS Main_Status { get; set; }
    }
}
