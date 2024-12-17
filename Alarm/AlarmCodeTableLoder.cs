using AGVSystemCommonNet6.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm
{

    internal class AlarmCodeTableLoder
    {
        public string ALARM_CODE_FILE_PATH => Path.Combine(AGVSConfigulator.ConfigsFilesFolder, "AGVS_AlarmCodes.json");

        public AlarmCodeTableLoder() { }

        public clsAlarmCode[] ReadAlarmCodeTable()
        {
            try
            {
                AlarmCodeTable jsonFile = ReadJsonFromFile();
                if (jsonFile == null)
                    throw new Exception();

                int runningVersionInt = GetVersionInt(jsonFile.FileVersion);
                int devVersionInt = GetVersionInt(AlarmCodeTable.VERSION);

                Console.WriteLine($"[Notice] Alarm Table版本資訊_ 現場版本: {jsonFile.FileVersion}/開發版本: {AlarmCodeTable.VERSION}");

                if (runningVersionInt < devVersionInt)
                    throw new Exception("FILE VERSION TOO OLD");
                else if (runningVersionInt > devVersionInt)
                    Console.WriteLine("[Notice] 現場的 Alarm Code Table版本高於開發中版本!");
                return jsonFile.Table;
            }
            catch (Exception ex) //若有例外表示檔案不存在或是 json反序列畫時失敗，需要重建新的檔案
            {
                Console.WriteLine(ex.Message);
                AlarmCodeTable newJson = CreateNewAlarmCodeTableFile();
                File.WriteAllText(ALARM_CODE_FILE_PATH, JsonConvert.SerializeObject(newJson, Formatting.Indented));
                return newJson.Table; //返回最新的
            }
        }

        private AlarmCodeTable ReadJsonFromFile()
        {
            if (!File.Exists(ALARM_CODE_FILE_PATH))
                throw new FileNotFoundException($"{ALARM_CODE_FILE_PATH} not exist");

            try
            {
                string jsonStr = File.ReadAllText(ALARM_CODE_FILE_PATH);
                return JsonConvert.DeserializeObject<AlarmCodeTable>(jsonStr);
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (JsonSerializationException ex)
            {
                throw ex;
            }
        }

        private AlarmCodeTable CreateNewAlarmCodeTableFile()
        {
            AlarmCodeTable newAlarmCodeTableObj = new AlarmCodeTable
            {
                FileVersion = AlarmCodeTable.VERSION,
            };
            return newAlarmCodeTableObj;
        }

        private int GetVersionInt(string versionString)
        {
            try
            {
                return int.Parse(versionString.Replace(".", ""));
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
