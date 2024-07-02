using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.Availability
{
    /// <summary>
    /// 稼動資料
    /// </summary>
    [Index(nameof(Time))]
    public class AvailabilityDto
    {
        [Key]
        public string KeyStr { get; set; } = "";
        [Column]
        public DateTime Time { get; set; } = DateTime.MinValue;
        [Column]
        public string AGVName { get; set; } = "";
        [Column]
        public double IDLE_TIME { get; set; } = 0;
        [Column]
        public double RUN_TIME { get; set; } = 0;
        [Column]
        public double DOWN_TIME { get; set; } = 0;
        [Column]
        public double CHARGE_TIME { get; set; } = 0;
        [Column]
        public double UNKNOWN_TIME { get; set; } = 0;

        internal string GetKey()
        {
            return AGVName + Time.ToString();
        }
    }
}
