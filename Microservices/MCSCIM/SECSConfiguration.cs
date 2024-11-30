using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.MCSCIM
{
    public class SECSConfiguration
    {
        public string DeviceID { get; set; } = "2F_AGVC02";
        public string CarrierLOCPrefixName { get; set; } = "ABFRACK005";
        public string SystemID { get; set; } = "022";

        public int UnknowTrayIDFlowNumberUsed { get; set; } = 0;
        public int UnknowRackIDFlowNumberUsed { get; set; } = 0;

    }
}
