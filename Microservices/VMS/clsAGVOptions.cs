using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.VMS
{
    public class clsAGVOptions
    {
        public enum PROTOCOL
        {
            TCP = 0,
            RESTFulAPI = 1,
        }

        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5036;
        public PROTOCOL Protocol { get; set; }

        public bool Simulation { get; set; }

        public int InitTag { get; set; } = 1;

        public bool Enabled { get; set; } = true;
        /// <summary>
        /// 車身長度
        /// <remark>
        /// 單位:cm
        /// </remark>
        /// </summary>
        public double VehicleLength { get; set; } = 145.0;
        public double VehicleWidth { get; set; } = 70;

        public List<int> List_ChargeStation { get; set; } = new List<int>();

        public clsBatteryOptions BatteryOptions { get; set; } = new clsBatteryOptions();

        public string AGV_ID { get; set; } = "";
    }

    public class clsBatteryOptions
    {
        public double HightLevel { get; set; } = 99;
        public double MiddleLevel { get; set; } = 50;

        public double LowLevel { get; set; } = 20;
    }
}
