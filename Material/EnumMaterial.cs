using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Material
{
    public enum MaterialInstallStatus : int
    {
        OK = 1,
        NG,
        Recover,
        Unknown,
        None,
    }

    public enum MaterialIDStatus : int
    {
        OK = 1,
        NG,
        Unknown,
        None,
    }

    public enum MaterialCondition : int
    {
        //搬運狀態
        Wait = 1,
        Transfering,
        Done,
        //帳料修改狀態
        Add,
        Delete,
        Edit,
    }

    public enum MaterialType : int
    {
        None = 1,
        Frame,
        Tray,
    }
}
