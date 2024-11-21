using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Equipment
{
    public abstract class EquipmentStatusBase
    {

        public enum RUN_STATUS
        {
            IDLE,
            RUN,
            DOWM
        }


        [Key]
        [MaxLength(150)]  // 或其他適當的長度限制
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public bool Connected { get; set; } = false;

        public int Tag { get; set; } = 0;
        public RUN_STATUS Status { get; set; } = RUN_STATUS.DOWM;

    }
}
