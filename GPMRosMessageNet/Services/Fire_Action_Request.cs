

using RosSharp.RosBridgeClient;


namespace AGVSystemCommonNet6.GPMRosMessageNet.Services
{
    public class Fire_Action_Request : Message
    {
        public const string RosMessageName = "gpm_msgs/FireCommand";

        public string command { get; set; }

        public Fire_Action_Request()
        {
            this.command = "";
        }

        public Fire_Action_Request(string command)
        {
            this.command = command;
        }
    }
}
