/* 
 * This message is auto generated by ROS#. Please DO NOT modify.
 * Note:
 * - Comments from the original code will be written in their own line 
 * - Variable sized arrays will be initialized to array of size 0 
 * Please report any issues at 
 * <https://github.com/siemens/ros-sharp> 
 */



using RosSharp.RosBridgeClient.MessageTypes.Nav;
using AGVSystemCommonNet6.GPMRosMessageNet.Actions;
using RosSharp.RosBridgeClient;
using AGVSystemCommonNet6.GPMRosMessageNet.Messages;

namespace AGVSystemCommonNet6.GPMRosMessageNet.Actions
{
    public class TaskCommandGoal : Message
    {
        public const string RosMessageName = "gpm_msgs/TaskCommandGoal";
        public enum GUIDE_TYPE : ushort
        {
            SLAM = 0,
            Color_Tap = 1,
            AR_TAG = 2,
        }
        public RosSharp.RosBridgeClient.MessageTypes.Nav.Path planPath { get; set; }
        //  
        public ushort guideType { get; set; }
        // 0-moving forward ; 1-fixed orientation
        public ushort mobilityModes { get; set; }
        public ushort finalGoalID { get; set; }
        public PathInfo[] pathInfo { get; set; }
        public string taskID { get; set; }

        public TaskCommandGoal()
        {
            this.planPath = new RosSharp.RosBridgeClient.MessageTypes.Nav.Path();
            this.guideType = 0;
            this.mobilityModes = 0;
            this.finalGoalID = 0;
            this.pathInfo = new PathInfo[0];
            this.taskID = "";
        }

        public TaskCommandGoal(RosSharp.RosBridgeClient.MessageTypes.Nav.Path planPath, ushort guideType, ushort mobilityModes, ushort finalGoalID, PathInfo[] pathInfo, string taskID)
        {
            this.planPath = planPath;
            this.guideType = guideType;
            this.mobilityModes = mobilityModes;
            this.finalGoalID = finalGoalID;
            this.pathInfo = pathInfo;
            this.taskID = taskID;
        }
    }
}
