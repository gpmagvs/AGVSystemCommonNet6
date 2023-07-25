using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP.GeoJson
{
    public class GeoJson
    {
        public string type { get; set; } = "FeatureCollection";
        public List<GeoFeature> features { get; set; } = new List<GeoFeature>();

    }
}
