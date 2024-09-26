using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    public class clsExitRequest : MessageBase
    {
        public Dictionary<string, ExitRequest> Header { get; set; } = new Dictionary<string, ExitRequest>();

    }

    public class ExitRequest
    {

        [JsonProperty("Time Stamp")]
        public string TimeStamp { get; set; }
        [JsonProperty("Exit Point")]
        public int ExitPoint { get; set; } = 0;
    }



    public class clsExitRequestACKMessage : MessageBase
    {
        public Dictionary<string, SimpleRequestResponseWithTimeStamp> Header { get; set; }
        internal SimpleRequestResponseWithTimeStamp ExitRequestAck => this.Header[Header.Keys.First()];

    }


    public class clsExitResponse : clsExitRequest
    {
        public ExitRequest exitRequest => Header[Header.Keys.First()];
    }
}
