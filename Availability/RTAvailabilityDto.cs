using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.Availability
{
    public class RTAvailabilityDto
    {
        [Key]
        public DateTime StartTime { get; set; }
        public string AGVName { get; set; } = "";
        public DateTime EndTime { get; set; }
        public MAIN_STATUS Main_Status { get; set; }
        internal double Idle_duraction
        {
            get
            {
                if (Main_Status != MAIN_STATUS.IDLE)
                    return 0;
                return (EndTime - StartTime).TotalSeconds;
            }
        }
    }
}
