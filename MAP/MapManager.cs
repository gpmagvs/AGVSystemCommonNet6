using AGVSystemCommonNet6.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AGVSystemCommonNet6.MAP
{
    public class MapManager
    {
        public static Map LoadMapFromFile(string mapfile)
        {
            if (!File.Exists(mapfile))
                return new Map()
                {
                     Points =new Dictionary<int, MapPoint>()
                };
            var json = System.IO.File.ReadAllText(mapfile);
            if (json == null)
                return null;
            try
            {

                var data_obj = JsonConvert.DeserializeObject<Dictionary<string, Map>>(json);
                return data_obj["Map"];
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadMapFromFile Error  : " + ex.Message);
                return new Map();
            }
        }
        public static Map LoadMapFromFile()
        {
            if (!File.Exists(AGVSConfigulator.SysConfigs.MapConfigs.MapFileFullName))
                return new Map()
                {
                    Points = new Dictionary<int, MapPoint>(),
                    Bays = new Dictionary<string, Bay>(),
                    Name = "empty",
                    Note = "empty"
                };
            var json = System.IO.File.ReadAllText(AGVSConfigulator.SysConfigs.MapConfigs.MapFileFullName);
            if (json == null)
                return null;
            try
            {

                var data_obj = JsonConvert.DeserializeObject<Dictionary<string, Map>>(json);
                return data_obj["Map"];
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadMapFromFile Error  : " + ex.Message);
                return new Map();
            }
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
