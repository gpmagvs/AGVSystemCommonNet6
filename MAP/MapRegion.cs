using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    /// <summary>
    /// 地圖上的禁制區
    /// </summary>
    public class MapRegion
    {
        public enum MAP_REGION_TYPE
        {
            FORBID, PASSIBLE
        }
        public string Name { get; set; }
        public MAP_REGION_TYPE RegionType { get; set; } = MAP_REGION_TYPE.FORBID;
        public List<double[]> PolygonCoordinations { get; set; } = new List<double[]>();
    }
}
