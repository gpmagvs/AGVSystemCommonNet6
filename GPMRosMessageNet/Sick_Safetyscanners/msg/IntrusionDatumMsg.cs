/* 
 * This message is auto generated by ROS#. Please DO NOT modify.
 * Note:
 * - Comments from the original code will be written in their own line 
 * - Variable sized arrays will be initialized to array of size 0 
 * Please report any issues at 
 * <https://github.com/siemens/ros-sharp> 
 */


using RosSharp.RosBridgeClient;

namespace AGVSystemCommonNet6.GPMRosMessageNet.SickSafetyscanners
{
    public class IntrusionDatumMsg : Message
    {
        public const string RosMessageName = "sick_safetyscanners/IntrusionDatumMsg";

        public uint size { get; set; }
        public bool[] flags { get; set; }

        public IntrusionDatumMsg()
        {
            this.size = 0;
            this.flags = new bool[0];
        }

        public IntrusionDatumMsg(uint size, bool[] flags)
        {
            this.size = size;
            this.flags = flags;
        }
    }
}
