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
    public class MeasurementDataMsg : Message
    {
        public const string RosMessageName = "sick_safetyscanners/MeasurementDataMsg";

        public uint number_of_beams { get; set; }
        public ScanPointMsg[] scan_points { get; set; }

        public MeasurementDataMsg()
        {
            this.number_of_beams = 0;
            this.scan_points = new ScanPointMsg[0];
        }

        public MeasurementDataMsg(uint number_of_beams, ScanPointMsg[] scan_points)
        {
            this.number_of_beams = number_of_beams;
            this.scan_points = scan_points;
        }
    }
}
