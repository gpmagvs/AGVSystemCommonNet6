using AGVSystemCommonNet6.AGVDispatch.Messages;
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
        /// <summary>
        /// 圖片的像素大小
        /// </summary>
        public int[] Map_Image_Size { get; set; } = new int[2] { 400, 400 };
        /// <summary>
        /// 地圖邊界對應實際長度 [x1,y1,x2,y2]
        /// </summary>
        public double[] Map_Image_Boundary { get; set; } = new double[4] { -20, -20, 20, 20 };
        public Dictionary<int, MapPoint> Points { get; set; }
        public Dictionary<string, BezierCurve> BezierCurves { get; set; } = new Dictionary<string, BezierCurve>();

        [JsonIgnore]
        public MapPath[] Pathes
        {
            get
            {
                List<MapPath> GetMapPathes(MapPoint point)
                {
                    int index = Points.FirstOrDefault(pt => pt.Value == point).Key;

                    bool isBezierendpoint = point.Graph.IsBezierCurvePoint;
                    BezierCurve bezier = new BezierCurve();
                    if (isBezierendpoint)
                    {
                        BezierCurves.TryGetValue(point.Graph.BezierCurveID, out bezier);
                    }
                    Dictionary<int, double> targets = point.Target.ToList().FindAll(kp => Points.ContainsKey(kp.Key)).ToDictionary(kp => kp.Key, kp => kp.Value);
                    return targets.Select(kp => new MapPath()
                    {
                        IsEQLink = point.StationType != STATION_TYPE.Normal | Points[kp.Key].StationType != STATION_TYPE.Normal,
                        StartPtIndex = index,
                        EndPtIndex = kp.Key,
                        StartCoordination = new double[2] { point.X, point.Y },
                        EndCoordination = new double[2] { Points[kp.Key].X, Points[kp.Key].Y },
                        IsBezier = isBezierendpoint,
                        BezierMiddleCoordination = isBezierendpoint ? bezier.MidPointCoordination : new double[2] { 0, 0 }
                    }
                    ).ToList();
                }
                MapPath[] pathes = Points.ToList().FindAll(point => point.Value.Target.Count != 0).SelectMany(point => GetMapPathes(point.Value)).ToArray();
                return pathes;
            }
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, Bay> Bays { get; set; } = new Dictionary<string, Bay>();


    }
}
