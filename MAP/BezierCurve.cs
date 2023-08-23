using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class BezierCurve
    {
        public string ID { get; set; }
        public double[] MidPointCoordination { get; set; } = new double[2];
        public int Rank { get; set; } = 2;
    }
}
