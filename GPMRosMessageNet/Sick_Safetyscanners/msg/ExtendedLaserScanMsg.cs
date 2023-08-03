/* 
 * This message is auto generated by ROS#. Please DO NOT modify.
 * Note:
 * - Comments from the original code will be written in their own line 
 * - Variable sized arrays will be initialized to array of size 0 
 * Please report any issues at 
 * <https://github.com/siemens/ros-sharp> 
 */

using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;

namespace AGVSystemCommonNet6.GPMRosMessageNet.SickSafetyscanners
{
    public class ExtendedLaserScanMsg : Message
    {
        public const string RosMessageName = "sick_safetyscanners/ExtendedLaserScanMsg";

        public LaserScan laser_scan { get; set; }
        public bool[] reflektor_status { get; set; }
        public bool[] reflektor_median { get; set; }
        public bool[] intrusion { get; set; }

        public ExtendedLaserScanMsg()
        {
            this.laser_scan = new LaserScan();
            this.reflektor_status = new bool[0];
            this.reflektor_median = new bool[0];
            this.intrusion = new bool[0];
        }

        public ExtendedLaserScanMsg(LaserScan laser_scan, bool[] reflektor_status, bool[] reflektor_median, bool[] intrusion)
        {
            this.laser_scan = laser_scan;
            this.reflektor_status = reflektor_status;
            this.reflektor_median = reflektor_median;
            this.intrusion = intrusion;
        }
    }
}
