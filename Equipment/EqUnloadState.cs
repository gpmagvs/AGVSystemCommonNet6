using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Equipment
{
    [Index(nameof(StartWaitUnloadTime))]
    [Index(nameof(EndWaitUnloadTime))]
    [Index(nameof(EQTag))]
    [Index(nameof(EQName))]
    public class EqUnloadState
    {
        [Key]
        public DateTime StartWaitUnloadTime { get; set; } = DateTime.MinValue;

        public DateTime EndWaitUnloadTime { get; set; } = DateTime.MinValue;

        public int EQTag { get; set; } = -1;

        public string EQName { get; set; } = "";

        [NotMapped]
        public double WaitUnloadTime => (EndWaitUnloadTime - StartWaitUnloadTime).TotalSeconds;
    }
}
