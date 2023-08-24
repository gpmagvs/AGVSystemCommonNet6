using AGVSystemCommonNet6.AGVDispatch.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class MapPath
    {
        public MapPath() { }
        public int StartPtIndex { get; set; }
        public int EndPtIndex { get; set; }
        public double[] StartCoordination { get; set; }
        public double[] EndCoordination { get; set; }
        public bool IsEQLink { get; set; }
        public string PathID => $"{StartPtIndex}_{EndPtIndex}";
        public bool IsBezier { get; set; } = false ;
        public double[] BezierMiddleCoordination{ get; set; }

    }
}
