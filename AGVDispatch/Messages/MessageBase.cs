using Newtonsoft.Json;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
 
    public abstract class MessageBase
    {
        public string SID { get; set; }
        public string EQName { get; set; }

        [JsonProperty("System Bytes")]
        public int SystemBytes { get; set; }
        public string OriJsonString;

        public Dictionary<string, MessageHeader> Header { get; set; }

    }

}
