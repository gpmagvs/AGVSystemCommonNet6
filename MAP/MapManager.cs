using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AGVSystemCommonNet6.MAP
{
    public class MapManager
    {
        public static Map LoadMapFromFile(string local_map_file_path = @"D:\param\Map_UMTC_5F_SMK_OVEN.json")
        {
            var json = System.IO.File.ReadAllText(local_map_file_path);
            if (json == null)
                return null;
            var data_obj = JsonConvert.DeserializeObject<Dictionary<string, Map>>(json);
            return data_obj["Map"];
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
