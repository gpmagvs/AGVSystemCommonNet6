using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Tools
{
    public class clsParkingAccuracy
    {
        [PrimaryKey]
        public DateTime Time { get; set; }
        public string TaskName { get; set; } = "";
        public string ParkingLocation { get; set; } = "";
        public int ParkingTag { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public double DistanceToTagCenter
        {
            get
            {
                return Math.Round(Math.Sqrt(X * X + Y * Y), 2);
            }
        }
    }
}
