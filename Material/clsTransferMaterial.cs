using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Material
{
    public class clsTransferMaterial
    {
        public clsTransferMaterial() { }
        public string MaterialID { get; set; } = string.Empty;
        public int TargetTag { get; set; } = -1;
        public int TargetSolt { get { return TargetRow; } }
        public string TargetStation { get; set; } = string.Empty;
        public int TargetRow { get; set; } = -1;
        public int TargetColumn { get; set; } = -1;
        public string SourceStation { get; set; } = string.Empty;
        public int SourceTag { get; set; } = -1;
        public int SourceSlot { get { return SourceRow; } }
        public int SourceRow { get; set; } = -1;
        public int SourceColumn { get; set; } = -1;
        public string message { get; set; } = string.Empty;
    }
}
