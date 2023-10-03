using Newtonsoft.Json;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    /// <summary>
    /// 0323 Carrier Virtual ID Query 。
    /// 當 AGVC VCR Read Fail 時，須以此訊息向 AGVS 詢問 Virtual ID。
    /// </summary>
    public class clsCarrierVirtualIDQueryMessage : MessageBase
    {
        public Dictionary<string, clsVirtualIDQu> Header { get; set; } = new Dictionary<string, clsVirtualIDQu>();
        internal clsVirtualIDQu ResetData => this.Header[Header.Keys.First()];
    }


    public class clsVirtualIDQu
    {
        [JsonProperty("Time Stamp")]
        public string Time_Stamp { get; set; }
    }
}
