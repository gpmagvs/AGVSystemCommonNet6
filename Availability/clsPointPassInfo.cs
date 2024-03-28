using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Availability
{
    public class clsPointPassInfo
    {
        [Key]
        public DateTime Time { get; set; }
        public int Tag { get; set; }
        public double Duration { get; set; }
        public string AGVName { get; set; } = "";
    }
}
