using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class clsMapPoiintRegist
    {
        public bool IsRegisted { get; set; } = false;

        public DateTime RegistTime { get; set; } = DateTime.MinValue;
        public string RegisterAGVName { get; set; } = "";

    }
}
