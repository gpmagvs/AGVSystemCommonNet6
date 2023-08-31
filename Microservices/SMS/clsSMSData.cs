using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.SMS
{
    public class clsSMSData
    {
        public List<string> Phone { get; set; } = new List<string>();
        public string Message { get; set; } = "";
        public string AP { get; set; } = "";
        public List<string> Country { get; set; } = new List<string>() { "TW" };
    }
}
