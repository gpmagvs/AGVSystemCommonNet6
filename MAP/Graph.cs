using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class Graph
    {
        public string Display { get; set; } = "";
        public double X { get; set; }
        public double Y { get; set; }
        public string ImageName { get; set; } = "";

        public double ImageScale { get; set; } = 0.45;

        public int[] ImageSize { get; set; } = new int[2] { 64, 64 };

        public bool IsBezierCurvePoint { get; set; } = false;
        public string BezierCurveID { get; set; } = "";

        public double textOffsetX { get; set; } = 0;
        public double textOffsetY { get; set; } = -22;

    }
}
