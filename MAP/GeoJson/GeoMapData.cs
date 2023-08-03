using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP.GeoJson
{
    public class GeoMapData
    {
        public GeoJson PointsGeoJsonData { get; set; } = new GeoJson();
        public GeoJson LinesGeoJsonData { get; set; } = new GeoJson();

    }

}
