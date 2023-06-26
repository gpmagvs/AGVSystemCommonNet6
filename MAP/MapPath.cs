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
        public MapPoint StartPoint { get; set; }
        public MapPoint EndPoint { get; set; }

        public bool IsEQLink
        {
            get
            {
                return EndPoint.IsEQLink | StartPoint.IsEQLink;
            }
        }
        public bool IsPathClose
        {
            get
            {
                return !StartPoint.Enable | !EndPoint.Enable;
            }
        }
    }
}
