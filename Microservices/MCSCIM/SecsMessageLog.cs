using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.MCSCIM
{
    public class SecsMessageLog
    {
        [Key]
        public DateTime LogTime { get; set; } = DateTime.MinValue;
        public int S { get; set; } = 0;
        public int F { get; set; } = 0;
        public int CEID { get; set; } = 0;
        public string CommandID { get; set; } = string.Empty;

        public string MessageDetail { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


    }
}
