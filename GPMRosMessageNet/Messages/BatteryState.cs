

using RosSharp.RosBridgeClient;


namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages
{
    public class BatteryState : Message
    {
        public const string RosMessageName = "gpm_msgs/BatteryState";

        public ushort batteryID { get; set; } = 1;
        public sbyte state { get; set; }
        public byte errorCode { get; set; }
        public ushort voltage { get; set; }
        public byte batteryLevel { get; set; }
        public ushort chargeCurrent { get; set; }
        public ushort dischargeCurrent { get; set; }
        public byte maxCellTemperature { get; set; }
        public byte minCellTemperature { get; set; }
        public ushort cycle { get; set; }
        public ushort chargeTime { get; set; }
        public ushort useTime { get; set; }

        public BatteryState()
        {
            this.batteryID = 0;
            this.state = 0;
            this.errorCode = 0;
            this.voltage = 0;
            this.batteryLevel = 0;
            this.chargeCurrent = 0;
            this.dischargeCurrent = 0;
            this.maxCellTemperature = 0;
            this.minCellTemperature = 0;
            this.cycle = 0;
            this.chargeTime = 0;
            this.useTime = 0;
        }

        public BatteryState(ushort batteryID, sbyte state, byte errorCode, ushort Voltage, byte batteryLevel, ushort chargeCurrent, ushort dischargeCurrent, byte maxCellTemperature, byte minCellTemperature, ushort cycle, ushort chargeTime, ushort useTime)
        {
            this.batteryID = batteryID;
            this.state = state;
            this.errorCode = errorCode;
            this.voltage = Voltage;
            this.batteryLevel = batteryLevel;
            this.chargeCurrent = chargeCurrent;
            this.dischargeCurrent = dischargeCurrent;
            this.maxCellTemperature = maxCellTemperature;
            this.minCellTemperature = minCellTemperature;
            this.cycle = cycle;
            this.chargeTime = chargeTime;
            this.useTime = useTime;
        }
    }
}
