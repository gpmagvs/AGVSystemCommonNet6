using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP.YunTech
{
    public class clsYuntechSubMap
    {
        public string MapName { get; set; }
        public string png { get; set; }
        public string Maptxt { get; set; }
        public double MapScale { get; set; }
        public clsMapsub[] Mapsub { get; set; } = new clsMapsub[0];
        public clsPath[] Path { get; set; } = new clsPath[0];
        public clsMaparea[] Maparea { get; set; } = new clsMaparea[0];
    }
}
