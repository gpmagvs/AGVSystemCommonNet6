using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.GPMRosMessageNet.Services
{
    public class SensorStateStateRequest : Message
    {
        public const string RosMessageName = "gpm_msgs/SensorState";
        public string equipment { get; set; }
        public string state { get; set; }
        public SensorStateStateRequest()
        {
            equipment = "";
            state = "on";
        }
        public SensorStateStateRequest(SENSORS equipment, bool state)
        {
            this.equipment = equipment.ToString();
            this.state = state ? "on" : "off";
        }

        public enum SENSORS
        {
            Ultrasonic,
            GroundHoleCamera
        }
    }

    public class SensorStateStateResponse : Message
    {
        public const string RosMessageName = "gpm_msgs/SensorState";
        public bool confirm { get; set; }
        public SensorStateStateResponse()
        {
            confirm = false;
        }
        public SensorStateStateResponse(bool confirm)
        {
            this.confirm = confirm;
        }
    }
}
