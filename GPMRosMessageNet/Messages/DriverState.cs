

using RosSharp.RosBridgeClient;


namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages
{
    public class DriverState : Message
    {
        public const string RosMessageName = "gpm_msgs/DriverState";

        public string name { get; set; }
        public sbyte state { get; set; }
        public byte errorCode { get; set; }
        public float speed { get; set; }
        public float position { get; set; }
        public ushort voltage { get; set; }
        public ushort outPercentage { get; set; }
        public ushort outCurrent { get; set; }
        public double overload { get; set; }

        public DriverState()
        {
            this.name = "";
            this.state = -1;
            this.errorCode = 0;
            this.speed = 0.0f;
            this.position = 0.0f;
            this.voltage = 0;
            this.outPercentage = 0;
            this.outCurrent = 0;
            this.overload = 0.0;
        }

        public DriverState(string name, sbyte state, byte errorCode, float speed, float position, ushort voltage, ushort outPercentage, ushort outCurrent, double overload)
        {
            this.name = name;
            this.state = state;
            this.errorCode = errorCode;
            this.speed = speed;
            this.position = position;
            this.voltage = voltage;
            this.outPercentage = outPercentage;
            this.outCurrent = outCurrent;
            this.overload = overload;
        }
    }
}
