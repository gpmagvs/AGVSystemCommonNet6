using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages
{
    public class IOlistsMsg : Message
    {
        public const string RosMessageName = "gpm_msgs/IOlists";

        public IOlistMsg[] IOtable;

        public IOlistsMsg()
        {
            IOtable = new IOlistMsg[0];
        }

        public IOlistsMsg(IOlistMsg[] iOtable)
        {
            IOtable = iOtable;
        }
    }

    public class IOlistsMsg_KGS : IOlistsMsg
    {
        public new const string RosMessageName = "agvstate/IOlists";

        public new IOlistMsg_KGSBase[] IOtable;

        public IOlistsMsg_KGS()
        {
            IOtable = new IOlistMsg_KGSBase[0];
        }

        public IOlistsMsg_KGS(IOlistMsg_KGSBase[] iOtable)
        {
            IOtable = iOtable;
        }
    }
}
