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
        public enum MAP_VERSION
        {
            V1 = 1,
            V2 = 2
        }

        /// <summary>
        /// 地圖版本
        /// V1:設定接設定在點上,路徑(線段)不可設定屬性; V2 : 路徑(線段)可設定屬性
        /// </summary>
        public virtual MAP_VERSION Version { get; set; } = MAP_VERSION.V1;
        public clsMapOptions Options { get; set; } = new clsMapOptions();
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

        public List<MapPath> Segments { get; set; } = new List<MapPath>();

        public List<MapRegion>Regions { get; set; } = new List<MapRegion>();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, Bay> Bays { get; set; } = new Dictionary<string, Bay>();

        public int GetPointIndexByGraphDisplayName(string name)
        {
            var pt = Points.FirstOrDefault(pt => pt.Value.Graph.Display == name);
            return pt.Value == null ? -1 : pt.Key;
        }
        public IEnumerable<int> GetStationTags()
        {
            return Points.Where(pt => pt.Value.StationType != STATION_TYPE.Normal).Select(pt => pt.Value.TagNumber).ToList();
        }
    }
}
