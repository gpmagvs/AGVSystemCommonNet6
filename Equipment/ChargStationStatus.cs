using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Equipment
{


    public class ChargStationStatus : EquipmentStatusBase
    {

        public double VoltageIn { get; set; } = 220.0;

        public double VoltageOut { get; set; } = 0.0;

        public double Current { get; set; } = 0.0;

        public double ChargerTemperature { get; set; } = 0.0;
        public double StationTemperature { get; set; } = 0.0;
    }
}
