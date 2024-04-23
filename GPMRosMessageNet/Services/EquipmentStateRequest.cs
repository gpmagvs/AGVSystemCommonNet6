using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.GPMRosMessageNet.Services
{
    public class EquipmentStateRequest : Message
    {
        public const string RosMessageName = "gpm_msgs/EquipmentState";
        public string equipment { get; set; }
        public string state { get; set; }
        public EquipmentStateRequest()
        {
            equipment = "";
            state = "on";
        }
        public EquipmentStateRequest(EQUIPMENTS equipment, bool state)
        {
            this.equipment = equipment.ToString();
            this.state = state ? "on" : "off";
        }

        public enum EQUIPMENTS
        {
            instrument,
            illuminance,
            decibel,
            temperature,
            humidity,
            PID
        }
    }

    public class EquipmentStateResponse : Message
    {
        public const string RosMessageName = "gpm_msgs/EquipmentState";
        public bool confirm { get;set; }
        public EquipmentStateResponse()
        {
            confirm=false;
        }
        public EquipmentStateResponse(bool confirm)
        {
            this.confirm = confirm;
        }
    }
}
