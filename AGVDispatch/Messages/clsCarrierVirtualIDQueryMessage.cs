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
        public enum VIRTUAL_ID_QUERY_TYPE
        {
            READ_FAIL = 2,
            NOT_MATCH = 3
        }
    
        [JsonProperty("Time Stamp")]
        public string Time_Stamp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("Query Type")]
        public VIRTUAL_ID_QUERY_TYPE QueryType { get; set; } = VIRTUAL_ID_QUERY_TYPE.READ_FAIL;

        [JsonProperty("CST Type")]
        public CST_TYPE CSTType { get; set; } = CST_TYPE.Tray;
    }

    /// <summary>
    /// [0324]=>0323的回覆。
    /// AGVS 收到 AGVC 端上報 Carrier Virtual ID Query 後，會回覆 Virtual ID 給 AGVC。
    /// </summary>
    public class clsCarrierVirtualIDResponseMessage : MessageBase
    {
        public Dictionary<string, clsCarrierVirtualIDResponse> Header { get; set; } = new Dictionary<string, clsCarrierVirtualIDResponse>();
        internal clsCarrierVirtualIDResponse CarrierVirtualIDResponse => this.Header[Header.Keys.First()];
    }
    public class clsCarrierVirtualIDResponse
    {
        [JsonProperty("Time Stamp")]
        public string TimeStamp { get; set; }
        [JsonProperty("Virtual ID")]
        public string VirtualID { get; set; } = "";
    }
    public class clsCarrierVirtualIDResponseWebAPI: clsCarrierVirtualIDResponse
    {
        public new DateTime TimeStamp { get; set; }
        public new string VirtualID { get; set; } = "";
    }
}
