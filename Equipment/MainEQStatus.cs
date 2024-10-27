using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Equipment
{

    /// <summary>
    /// 主設備狀態
    /// </summary>
    public class MainEQStatus : EquipmentStatusBase
    {
        public bool UnloadRequest { get; set; } = false;
        public bool LoadRequest { get; set; } = false;
        public bool CargoExist { get; set; } = false;
        public bool Maintaining { get; set; } = false;
        public bool PartsReplacing { get; set; } = false;

    }
}
