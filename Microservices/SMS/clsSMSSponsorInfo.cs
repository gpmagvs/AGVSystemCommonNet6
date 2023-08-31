using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.SMS
{
    public class clsSMSSponsorInfo
    {
        public string ZoneName { get; set; }
        public string SMSIP { get; set; }
        public List<string> Phones { get; set; } = new List<string>();
    }
}
