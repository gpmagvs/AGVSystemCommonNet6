using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.MAP.YunTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP.Converter
{
    public class MapConverter
    {

        public Dictionary<string, Dictionary<string, Map>> YuntechMapToGPMMapFromFile(string fileMap, out Dictionary<string, Dictionary<string, clsYuntechSubMap>> yunTecMap)
        {
            yunTecMap = YunTechMapManager.LoadMapFromFile(fileMap);

            Dictionary<string, Dictionary<string, Map>> Maps = new Dictionary<string, Dictionary<string, Map>>();
            foreach (var _MAP in yunTecMap)
            {
                Dictionary<string, Map> BMap = _MAP.Value.ToDictionary(map => map.Key, map => YunTechSubMapToMap(map.Value));
                Maps.Add(_MAP.Key, BMap);
            }

            return Maps;
        }

        private Map YunTechSubMapToMap(clsYuntechSubMap subMap)
        {

            return new Map()
            {
                Name = subMap.MapName,
                Bays = new Dictionary<string, Bay>(),
                Note = $"{DateTime.Now.ToString("yyyyMMdd")}.87",
                Points = subMap.Mapsub.ToDictionary(sub => subMap.Mapsub.ToList().IndexOf(sub), sub => new MapPoint()
                {
                    Enable = true,
                    Name = sub.Name,
                    SubMap = subMap.MapName,
                    StationType = STATION_TYPE.Normal,                    
                    //TagNumber = subMap.Mapsub.ToList().IndexOf(sub) + 1,
                    TagNumber = Convert.ToInt32(sub.Name),
                    X = sub.x,
                    Y = sub.y,
                    Direction = (int)sub.theta,
                    Target = FindIndexOfTargetPoints(subMap, sub).ToDictionary(index => index, index => 1.0),
                    Graph = new Graph() { X = (int)sub.x, Y = (int)sub.y, Display = sub.Name },
                    AutoDoor = new AutoDoor()
                    {
                        KeyName = "",
                        KeyPassword = ""
                    },
                    Bay = "",
                    DodgeMode = 0,
                    SpinMode = 0,
                    LsrMode = 1,
                    InvolvePoint = "",
                    MotionInfo = new MotionInfo()
                    {
                        ControlBypass = new ControlBypass
                        {
                            GroundHoleCCD = "",
                            GroundHoleSensor = "",
                            UltrasonicSensor = ""
                        }
                    }


                })
            };
        }
        //public Map YuntechMapToGPMMapFromObject(object  json)
        //{

        //}

        private int[] FindIndexOfTargetPoints(clsYuntechSubMap mapRef, clsMapsub point)
        {
            if (mapRef.Path.Length == 0)
                return new int[0];
            List<clsPath> pathes = new List<clsPath>();
            foreach (var _path in mapRef.Path)
            {
                pathes.Add(_path);
                if (!_path.IsOneWay)
                {
                    pathes.Add(new clsPath
                    {
                        IsOneWay = true,
                        p0x = _path.p1x,
                        p0y = _path.p1y,
                        p1x = _path.p0x,
                        p1y = _path.p0y,
                    });
                }
            }
            List<clsPath> paths = pathes.ToList().FindAll(path => path.p0x == point.x && path.p0y == point.y);
            if (paths.Count == 0)
                return new int[0];

            int[] indexes = paths.Select(p => mapRef.Mapsub.ToList().IndexOf(mapRef.Mapsub.First(pt => pt.x == p.p1x && pt.y == p.p1y))).ToArray();
            return indexes;
        }



    }
}
