using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages
{
    public class IOlistMsg : Message
    {
        public const string RosMessageName = "gpm_msgs/IOlist";

        public string Key;
        public int Coil;
        public int addr;

        public IOlistMsg()
        {
            Key = "";
            Coil = 0;
            addr = 0;
        }

        public IOlistMsg(string key, int coil, int address)
        {
            Key = key;
            Coil = coil;
            addr = address;
        }
    }

    public class IOlistMsg_KGSBase : IOlistMsg
    {
        public new const string RosMessageName = "agvstate/IOlist";
    }
}
