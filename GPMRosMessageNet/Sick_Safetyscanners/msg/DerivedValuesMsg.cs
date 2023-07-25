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
    public class DerivedValuesMsg : Message
    {
        public const string RosMessageName = "sick_safetyscanners/DerivedValuesMsg";

        public ushort multiplication_factor { get; set; }
        public ushort number_of_beams { get; set; }
        public ushort scan_time { get; set; }
        public float start_angle { get; set; }
        public float angular_beam_resolution { get; set; }
        public uint interbeam_period { get; set; }

        public DerivedValuesMsg()
        {
            this.multiplication_factor = 0;
            this.number_of_beams = 0;
            this.scan_time = 0;
            this.start_angle = 0.0f;
            this.angular_beam_resolution = 0.0f;
            this.interbeam_period = 0;
        }

        public DerivedValuesMsg(ushort multiplication_factor, ushort number_of_beams, ushort scan_time, float start_angle, float angular_beam_resolution, uint interbeam_period)
        {
            this.multiplication_factor = multiplication_factor;
            this.number_of_beams = number_of_beams;
            this.scan_time = scan_time;
            this.start_angle = start_angle;
            this.angular_beam_resolution = angular_beam_resolution;
            this.interbeam_period = interbeam_period;
        }
    }
}
