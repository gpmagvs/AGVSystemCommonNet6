using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Equipment.AGV
{
    public class AGVStatus : EquipmentStatusBase
    {
        #region 電池

        public double BatLevel { get; set; } = 0;
        public double BatVoltage { get; set; } = 0;
        public double BatChargeCurrent { get; set; } = 0;
        public double BatDisChargeCurrent { get; set; } = 0;
        #endregion

        #region 左輪馬達
        public double MotorRightVoltage { get; set; } = 0;
        public double MotorRightCurrent { get; set; } = 0;
        public double MotorRightTemperature { get; set; } = 0;
        public int MotorRightAlarmCode { get; set; } = 0;
        #endregion

        #region 右輪馬達
        public double MotorLeftVoltage { get; set; } = 0;
        public double MotorLeftCurrent { get; set; } = 0;
        public double MotorLeftTemperature { get; set; } = 0;
        public int MotorLeftAlarmCode { get; set; } = 0;
        #endregion

        public double CoordinateX { get; set; } = 0;
        public double CoordinateY { get; set; } = 0;

        public string CurrentPathTag { get; set; } = "";


    }
}
