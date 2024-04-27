using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Configuration;
using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
using System.IO.Compression;
using static AGVSystemCommonNet6.MAP.MapPoint;

namespace AGVSystemCommonNet6.MAP
{
    public class MapManager
    {
        public static Map LoadMapFromFile(string mapfile, out string errorMsg, bool auto_create_segment = true, bool auto_check_path_error = true)
        {
            errorMsg = "";
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
                var data_obj = new Dictionary<string, Map>();

                try
                {
                    data_obj = JsonConvert.DeserializeObject<Dictionary<string, Map>>(json, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                catch (Exception ex)
                {
                    Map _map = JsonConvert.DeserializeObject<Map>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    data_obj.Add("Map", _map);
                }
                var map = data_obj["Map"];
                if (auto_create_segment && map.Points.Count != 0 && map.Segments.Count == 0)
                {
                    map.Segments = CreateSegments(map);
                    SaveMapToFile(map, mapfile);
                }

                if (auto_check_path_error)
                {
                    CheckMapSettingAndFix(map, mapfile);
                }
                if (map.Options.EQIcons.Count == 0)
                {
                    map.Options.EQIcons = new List<string>() {
                        "/images/eq-icon.png",
                        "/images/rack.png",
                    };

                }
                return map;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message + ex.StackTrace;
                Console.WriteLine("LoadMapFromFile Error  : " + ex.Message);
                return new Map();
            }
        }
        public static Map LoadMapFromFile(bool auto_create_segment = true, bool auto_check_path_error = true)
        {
            return LoadMapFromFile(AGVSConfigulator.SysConfigs.MapConfigs.MapFileFullName, out string msg, auto_create_segment, auto_check_path_error);

        }
        public static List<MapPath> CreateSegments(Map map)
        {
            List<MapPath> GetMapPathes(Map map, MapPoint point)
            {
                var Points = map.Points;
                int index = Points.FirstOrDefault(pt => pt.Value == point).Key;
                bool isBezierendpoint = point.Graph.IsBezierCurvePoint;
                Dictionary<int, double> targets = point.Target.ToList().FindAll(kp => Points.ContainsKey(kp.Key)).ToDictionary(kp => kp.Key, kp => kp.Value);
                return targets.Select(kp => new MapPath()
                {
                    IsEQLink = point.StationType != STATION_TYPE.Normal || Points[kp.Key].StationType != STATION_TYPE.Normal,
                    StartPtIndex = index,
                    EndPtIndex = kp.Key,
                    StartCoordination = new double[2] { point.X, point.Y },
                    EndCoordination = new double[2] { Points[kp.Key].X, Points[kp.Key].Y },
                }
                ).ToList();
            }
            List<MapPath> pathes = map.Points.ToList().FindAll(point => point.Value.Target.Count != 0).SelectMany(point => GetMapPathes(map, point.Value)).ToList();
            return pathes;
        }
        public static bool SaveMapToFile(Map map_modified, string local_map_file_path)
        {
            try
            {
                map_modified = CheckMapSettingAndFix(map_modified);
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
        private static void CheckMapSettingAndFix(Map map, string save_path)
        {
            var fixmap = CheckMapSettingAndFix(map);
            SaveMapToFile(fixmap, save_path);
        }
        private static Map CheckMapSettingAndFix(Map map)
        {
            if (map.Segments.Any(path => !IsPointExistAtMap(path.StartPtIndex) || !IsPointExistAtMap(path.EndPtIndex)))
            {
                var error_path = map.Segments.FindAll(path => !IsPointExistAtMap(path.StartPtIndex) || !IsPointExistAtMap(path.EndPtIndex));
                foreach (var p in error_path)
                {
                    map.Segments.Remove(p);
                }
            }

            var errorPath = map.Segments.FindAll(path => IsEqLinkButPropertyError(path));
            foreach (var path in errorPath)
            {
                path.IsEQLink = true;
            }

            bool IsEqLinkButPropertyError(MapPath path)
            {
                if (path.IsEQLink)
                    return false;
                bool isStartPtEQ = map.Points[path.StartPtIndex].IsEquipment || map.Points[path.StartPtIndex].IsCharge;
                bool isEndPtEQ = map.Points[path.EndPtIndex].IsEquipment || map.Points[path.EndPtIndex].IsCharge;
                return isStartPtEQ || isEndPtEQ;
            }
            return map;
            bool IsPointExistAtMap(int pointIndex)
            {
                return map.Points.TryGetValue(pointIndex, out var point);
            }
        }
    }
}
