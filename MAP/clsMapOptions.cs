using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class clsMapOptions
    {
        public string pathColor { get; set; } = "rgb(45,42,46)";
        public int fontSizeOfDisplayName { get; set; } = 12;
        public int fontSizeOfAsCandicates { get; set; } = 16;

        public List<string> EQIcons { get; set; }= new List<string>();  

    }
}
