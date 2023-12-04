using SQLite;

namespace AGVSystemCommonNet6.Vehicle_Control.Models
{
    public class clsParkingAccuracy: TaskStatusModelBase
    {
        public string ParkingLocation { get; set; } = "";
        public int ParkingTag { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public double Slam_X { get; set; }
        public double Slam_Y { get; set; }

        public bool IsGoodParkingLoaction { get; set; } = true;

        public double Slam_Theta { get; set; }

        public double DistanceToTagCenter
        {
            get
            {
                return Math.Round(Math.Sqrt(X * X + Y * Y), 2);
            }
        }
    }
}
