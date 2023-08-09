using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch.Model
{
    /// <summary>
    /// 任務軌跡
    /// </summary>
    public class clsTaskTrajecotroyStore
    {
        [Key]
        public string TaskName { get; set; } = string.Empty;

        public string AGVName { get; set; } = "";

        public string CoordinationsJson { get; set; } = "{}";

        internal List<clsTrajCoordination> Coordinations
        {
            get
            {
                return JsonConvert.DeserializeObject<List<clsTrajCoordination>>(CoordinationsJson);
            }
        }
    }
    public class clsTrajCoordination
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Theta { get; set; }
    }
}
