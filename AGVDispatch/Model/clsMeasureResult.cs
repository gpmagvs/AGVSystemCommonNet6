using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch.Model
{
    public class clsMeasureResult
    {
        public int TagID { get; private set; }
        public string TaskName { get; set; } = "";
        public string AGVName { get; set; } = "";
        public string BayName { get; set; } = "";
        [Key]
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public string result { get; set; } = "";
        public string location { get; set; } = "";
        public int illuminance { get; set; }
        public int decibel { get; set; }
        public double temperature { get; set; }
        public double humudity { get; set; }
        /// <summary>
        /// 異丙醇
        /// </summary>
        public int IPA { get; set; }
        /// <summary>
        /// 丙酮
        /// </summary>
        public int Acetone { get; set; }
        public double TVOC { get; set; }
        public string time { get; set; } = "";
        public int partical_03um { get; set; }
        public int partical_05um { get; set; }
        public int partical_10um { get; set; }
        public int partical_30um { get; set; }
        public int partical_50um { get; set; }
        public int partical_100um { get; set; }
        public int PID { get; set; }

        public clsMeasureResult(int TagID)
        {
            this.TagID = TagID;
        }
        public bool result_bol
        {
            get
            {
                return result == "done";
            }
        }
        public string GetCommandStr()
        {
            object[] resultObj = new object[]
            {
                     result,
                     location,
                     illuminance,
                    decibel,
                    temperature*100,
                    humudity*100,
                    IPA,
                    TVOC*10,
                    Acetone,
                    time,
                    partical_03um,
                    partical_05um,
                    partical_10um,
                    partical_30um,
                    partical_50um,
                    partical_100um,
                    PID,
            };
            return string.Join(",", resultObj.Select(obj => obj.ToString()));
        }
    }
}
