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
        public bool IsBezierCurvePoint { get; set; } = false;
        public string BezierCurveID { get; set; } = "";

    }
}
