

using RosSharp.RosBridgeClient;


namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages
{
    public class CSTReaderState : Message
    {
        public const string RosMessageName = "gpm_msgs/CSTReaderState";

        public string name { get; set; }
        public string data { get; set; }

        /// <summary>
        /// -1:�s�u���`
        /// 1:��ӧ���
        /// 2:��Ӥ�
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
