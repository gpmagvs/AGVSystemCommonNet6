using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Maintainance
{

    public enum MAINTAIN_ITEM
    {
        /// <summary>
        /// 走行馬達
        /// </summary>
        HORIZON_MOTOR = 100,
        /// <summary>
        /// 牙叉
        /// </summary>
        FORKLIFT_BELT
    }

    public class VehicleMaintain
    {
        public VehicleMaintain()
        {

        }

        public VehicleMaintain(string AGVName, MAINTAIN_ITEM maintainItem)
        {
            this.AGV_Name = AGVName;
            this.MaintainItem = maintainItem;
            VehicleMaintainId = AGVName + "-" + MaintainItem;
        }

        [Key]
        public string VehicleMaintainId { get; set; }

        [ForeignKey("clsAGVStateDto")]
        public string AGV_Name { get; set; }

        public MAINTAIN_ITEM MaintainItem { get; set; } = MAINTAIN_ITEM.HORIZON_MOTOR;

        [NotMapped]
        public string MaintainItemName
        {
            get
            {
                switch (MaintainItem)
                {
                    case MAINTAIN_ITEM.HORIZON_MOTOR:
                        return "走行馬達保養";
                    case MAINTAIN_ITEM.FORKLIFT_BELT:
                        return "牙叉皮帶保養";
                    default:
                        return "NONE";
                }
            }
        }
        /// <summary>
        /// 當前數值
        /// </summary>
        public double currentValue { get; set; }

        /// <summary>
        /// 需要維修的數值
        /// </summary>

        public double maintainValue { get; set; }

        [NotMapped]
        public bool IsReachMaintainValue => maintainValue == 0 ? false : currentValue >= maintainValue;

        public clsAGVStateDto VehicleState { get; set; }

    }
}
