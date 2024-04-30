using AGVSystemCommonNet6.AGVDispatch.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.MAP.MapPoint;

namespace AGVSystemCommonNet6.MAP.GeoJson
{
    public class GeoFeature
    {
        public string type { get; set; } = "Feature";
        public object geometry { get; set; }
        public FeatureProperties properties { get; set; } = new FeatureProperties();
    }


    public class FeatureProperties
    {
        public int point_index { get; set; }
        public int tag_id { get; set; }
        public string name { get; set; }
        public STATION_TYPE station_type { get; set; }
        public string station_type_str
        {
            get => station_type.ToString();
        }
        public string color
        {
            get
            {
                string color = "seagreen";
                switch (station_type)
                {
                    case STATION_TYPE.Normal:
                        color = "green";
                        break;
                    case STATION_TYPE.EQ:
                        return "blue";
                    case STATION_TYPE.STK:
                        break;
                    case STATION_TYPE.Charge:
                        break;
                    case STATION_TYPE.Buffer:
                        break;
                    case STATION_TYPE.Charge_Buffer:
                        break;
                    case STATION_TYPE.Charge_STK:
                        break;
                    case STATION_TYPE.Escape:
                        break;
                    case STATION_TYPE.EQ_LD:
                        break;
                    case STATION_TYPE.STK_LD:
                        break;
                    case STATION_TYPE.EQ_ULD:
                        break;
                    case STATION_TYPE.STK_ULD:
                        break;
                    case STATION_TYPE.Fire_Door:
                        break;
                    case STATION_TYPE.Fire_EQ:
                        break;
                    case STATION_TYPE.Auto_Door:
                        break;
                    case STATION_TYPE.Elevator:
                        break;
                    case STATION_TYPE.Elevator_LD:
                        break;
                    case STATION_TYPE.Unknown:
                        break;
                    default:
                        break;
                }
                return color;
            }
        }
    }

    public class PointGeomety
    {
        public string type { get;  } = "Point";
        public double[]  coordinates { get; set; }
    }

    public class LineStringGeomety
    {
        public string type { get;  } = "LineString";
        public List<double[]> coordinates { get; set; }
    }

}
