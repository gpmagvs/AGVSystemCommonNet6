using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class Map
    {
        public string Name { get; set; }
        public string Note { get; set; }
        public int PointIndex { get; set; }
        public Dictionary<int, MapPoint> Points { get; set; }
        public Dictionary<string, BezierCurve> BezierCurves { get; set; } = new Dictionary<string, BezierCurve>();

        [JsonIgnore]
        public MapPath[] Pathes
        {
            get
            {
                List<MapPath> GetMapPathes(MapPoint point)
                {

                    Dictionary<int, double> targets = point.Target.ToList().FindAll(kp => Points.ContainsKey(kp.Key)).ToDictionary(kp => kp.Key, kp => kp.Value);
                    return targets.Select(kp => new MapPath() { StartPoint = point, EndPoint = Points[kp.Key] }).ToList();
                }
                MapPath[] pathes = Points.ToList().FindAll(point => point.Value.Target.Count != 0).SelectMany(point => GetMapPathes(point.Value)).ToArray();
                return pathes;
            }
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, Bay> Bays { get; set; }


    }
}
