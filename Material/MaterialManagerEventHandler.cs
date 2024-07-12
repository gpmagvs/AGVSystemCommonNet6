using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Material
{
    public class MaterialManagerEventHandler
    {
        public static event EventHandler<clsMaterialInfo> OnMaterialTransferStatusChange;
        public static event EventHandler<clsMaterialInfo> OnMaterialAdd;
        public static event EventHandler<clsMaterialInfo> OnMaterialDelete;
    }
}
