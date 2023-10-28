

using RosSharp.RosBridgeClient;


namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages
{
    public class CSTReaderState : Message
    {
        public const string RosMessageName = "gpm_msgs/CSTReaderState";

        public string name { get; set; }
        public string data { get; set; }

        /// <summary>
        /// -1:連線異常
        /// 1:拍照完成
        /// 2:拍照中
        /// </summary>
        public ushort state { get; set; }

        public CSTReaderState()
        {
            this.name = "";
            this.data = "";
            this.state = 0;
        }

        public CSTReaderState(string name, string data, ushort state)
        {
            this.name = name;
            this.data = data;
            this.state = state;
        }
    }
}
