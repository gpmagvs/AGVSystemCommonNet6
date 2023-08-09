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
    public class ApplicationDataMsg : Message
    {
        public const string RosMessageName = "sick_safetyscanners/ApplicationDataMsg";

        public ApplicationInputsMsg inputs { get; set; }
        public ApplicationOutputsMsg outputs { get; set; }

        public ApplicationDataMsg()
        {
            this.inputs = new ApplicationInputsMsg();
            this.outputs = new ApplicationOutputsMsg();
        }

        public ApplicationDataMsg(ApplicationInputsMsg inputs, ApplicationOutputsMsg outputs)
        {
            this.inputs = inputs;
            this.outputs = outputs;
        }
    }
}