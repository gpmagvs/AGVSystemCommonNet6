

using RosSharp.RosBridgeClient;


namespace AGVSystemCommonNet6.GPMRosMessageNet.Services
{
    public class Fire_Action_Response : Message
    {
        public const string RosMessageName = "gpm_msgs/FireCommand";

        public bool confirm { get; set; }

        public Fire_Action_Response()
        {
            this.confirm = false;
        }

        public Fire_Action_Response(bool confirm)
        {
            this.confirm = confirm;
        }
    }
}
