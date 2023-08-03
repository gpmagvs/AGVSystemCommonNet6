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
    public class OutputPathsMsg : Message
    {
        public const string RosMessageName = "sick_safetyscanners/OutputPathsMsg";

        public bool[] status { get; set; }
        public bool[] is_safe { get; set; }
        public bool[] is_valid { get; set; }
        public int active_monitoring_case { get; set; }

        public OutputPathsMsg()
        {
            this.status = new bool[0];
            this.is_safe = new bool[0];
            this.is_valid = new bool[0];
            this.active_monitoring_case = 0;
        }

        public OutputPathsMsg(bool[] status, bool[] is_safe, bool[] is_valid, int active_monitoring_case)
        {
            this.status = status;
            this.is_safe = is_safe;
            this.is_valid = is_valid;
            this.active_monitoring_case = active_monitoring_case;
        }
    }
}
