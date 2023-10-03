using Newtonsoft.Json;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    public class clsTaskResetReqMessage : MessageBase
    {
        public Dictionary<string, TaskResetDto> Header { get; set; } = new Dictionary<string, TaskResetDto>();
        internal TaskResetDto ResetData => this.Header[Header.Keys.First()];
    }


    public class TaskResetDto
    {
        [JsonProperty("Time Stamp")]
        public string Time_Stamp { get; set; }

        [JsonProperty("Reset Mode")]
        public RESET_MODE ResetMode { get; set; }

        [JsonProperty("Reset Type")]
        public RESET_TYPE ResetType { get; set; }
    }
}
