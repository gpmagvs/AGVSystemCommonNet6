using AGVSystemCommonNet6.AGVDispatch.RunMode;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Sys
{
    public class AGVSSystemStatus
    {
        [Key]
        [MaxLength(150)]  // 或其他適當的長度限制
        public string FieldName { get; set; } = "";
        public string Version { get; set; } = "1.0.0";
        public RUN_MODE RunMode { get; set; } = RUN_MODE.MAINTAIN;
        public HOST_CONN_MODE HostConnMode { get; set; } = HOST_CONN_MODE.OFFLINE;
        public HOST_OPER_MODE HostOperMode { get; set; } = HOST_OPER_MODE.LOCAL;
    }
}
