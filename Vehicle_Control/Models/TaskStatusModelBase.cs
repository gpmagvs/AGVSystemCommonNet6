using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Vehicle_Control.Models
{
    public class TaskStatusModelBase
    {

        [PrimaryKey]
        public DateTime Time { get; set; }
        public string TaskName { get; set; } = "";
        public int DestineTag { get; set; } = -1;
        public TaskStatusModelBase() { }
    }
}
