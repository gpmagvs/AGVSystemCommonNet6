using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch.RunMode
{    /// <summary>
     /// 系統運轉狀態
     /// </summary>
    public enum RUN_MODE
    {
        MAINTAIN,
        RUN,
        SWITCH_TO_MAITAIN_ING,
        SWITCH_TO_RUN_ING
    }

    /// <summary>
    /// HOST連線狀態
    /// </summary>
    public enum HOST_CONN_MODE
    {
        OFFLINE,
        ONLINE
    }

    /// <summary>
    /// HOST 模式
    /// </summary>
    public enum HOST_OPER_MODE
    {
        LOCAL,
        REMOTE
    }
}
