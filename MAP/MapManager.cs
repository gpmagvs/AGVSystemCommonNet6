using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Configuration;
using Newtonsoft.Json;

namespace AGVSystemCommonNet6.MAP
{
    public class MapManager
    {
        public static Map LoadMapFromFile(string mapfile)
        {
            if (!File.Exists(mapfile))
                return new Map()
                {
                    Points = new Dictionary<int, MapPoint>(),
                    Bays = new Dictionary<string, Bay>(),
                    Name = "empty",
                    Note = "empty"
                };
            var json = System.IO.File.ReadAllText(mapfile);
            if (json == null)
                return null;
            try
            {
                var data_obj = JsonConvert.DeserializeObject<Dictionary<string, Map>>(json);
                var map = data_obj["Map"];
                if (map.Points.Count != 0 && map.Segments.Count == 0)
                {
                    List<MapPath_V2> GetMapPathes(Map map, MapPoint point)
                    {
                        var Points = map.Points;
                        int index = Points.FirstOrDefault(pt => pt.Value == point).Key;
                        bool isBezierendpoint = point.Graph.IsBezierCurvePoint;
                        Dictionary<int, double> targets = point.Target.ToList().FindAll(kp => Points.ContainsKey(kp.Key)).ToDictionary(kp => kp.Key, kp => kp.Value);
                        return targets.Select(kp => new MapPath_V2()
                        {
                            IsEQLink = point.StationType != STATION_TYPE.Normal | Points[kp.Key].StationType != STATION_TYPE.Normal,
                            StartPtIndex = index,
                            EndPtIndex = kp.Key,
                            StartCoordination = new double[2] { point.X, point.Y },
                            EndCoordination = new double[2] { Points[kp.Key].X, Points[kp.Key].Y },
                        }
                        ).ToList();
                    }
                    List<MapPath_V2> pathes = map.Points.ToList().FindAll(point => point.Value.Target.Count != 0).SelectMany(point => GetMapPathes(map, point.Value)).ToList();
                    map.Segments = pathes;
                    SaveMapToFile(map, mapfile);
                }
                return map;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadMapFromFile Error  : " + ex.Message);
                return new Map();
            }
        }
        public static Map LoadMapFromFile()
        {
            return LoadMapFromFile(AGVSConfigulator.SysConfigs.MapConfigs.MapFileFullName);
     
        }

        public static bool SaveMapToFile(Map map_modified, string local_map_file_path)
        {
            try
            {
                Dictionary<string, Map> dataToSave = new Dictionary<string, Map>()
                {
                    {"Map",map_modified }
                };
                string json = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);
                System.IO.File.WriteAllText(local_map_file_path, json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static List<int> GetTags()
        {
            var map = LoadMapFromFile();
            List<int> tags = map.Points.Values.Select(pt => pt.TagNumber).ToList().Distinct().ToList();
            tags.Sort();
            return tags;
        }

    }
}
