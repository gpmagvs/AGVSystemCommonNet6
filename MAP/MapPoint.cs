using AGVSystemCommonNet6.AGVDispatch.Messages;
using Newtonsoft.Json;
using System.Drawing;

namespace AGVSystemCommonNet6.MAP
{
    public class MapPoint
    {
        public enum STATION_TYPE : int
        {
            Normal = 0,
            EQ = 1,
            STK = 2,
            Charge = 3,
            Buffer = 4,
            Charge_Buffer = 5,
            Charge_STK = 6,
            Exchange = 7,
            Escape = 8,
            EQ_LD = 11,
            STK_LD = 12,
            EQ_ULD = 21,
            STK_ULD = 22,
            Fire_Door = 31,
            Fire_EQ = 32,
            Auto_Door = 33,
            Elevator = 100,
            Elevator_LD = 201,
            TrayEQ = 202,
            TrayEQ_LD = 211,
            TrayEQ_ULD = 221,
            Unknown = 9999
        }
        public MapPoint() { }
        public MapPoint(string Name, int TagNumber)
        {
            this.Name = Name;
            this.TagNumber = TagNumber;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public string? Name { get; set; } = "";
        public int TagNumber { get; set; }
        public int Direction { get; set; }
        /// <summary>
        /// 
        /// 二次定位點的停車角度
        /// </summary>
        public int Direction_Secondary_Point { get; set; }
        public bool AGV_Alarm { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 是否可停駐
        /// </summary>
        public bool IsStandbyPoint { get; set; }

        /// <summary>
        /// 是否為二次定位點
        /// </summary>
        public bool IsSegment { get; set; }
        public bool IsOverlap { get; set; }
        /// <summary>
        /// 是否可停車
        /// </summary>
        public bool IsParking { get; set; }
        /// <summary>
        /// 是否可避車
        /// </summary>
        public bool IsAvoid { get; set; }
        /// <summary>
        /// 是否為虛擬點
        /// </summary>
        public bool IsVirtualPoint { get; set; }
        /// <summary>
        /// 是否為自動門
        /// </summary>
        public bool IsAutoDoor { get; set; }
        /// <summary>
        /// 是否為消防設備
        /// </summary>
        public bool IsExtinguishing { get; set; }

        /// <summary>
        /// 是否為交通檢查點
        /// </summary>
        public bool IsTrafficCheckPoint { get; set; }

        public bool IsNarrowPath { get; set; }
        /// <summary>
        /// 註冊點(,逗點分隔)
        /// </summary>
        public string? InvolvePoint { get; set; } = "";
        /// <summary>
        /// 新版的註冊點屬性
        /// </summary>
        public int[] RegistsPointIndexs { get; set; } = new int[0];
        public STATION_TYPE StationType { get; set; }
        public int LsrMode { get; set; }
        public double Speed { get; set; }

        public string? Bay { get; set; } = "";
        public Graph Graph { get; set; } = new Graph();

        /// <summary>
        /// Key Point Index, value:權重
        /// </summary>
        public Dictionary<int, double>? Target { get; set; } = new Dictionary<int, double>();
        public int? DodgeMode { get; set; } = 0;
        public int? SpinMode { get; set; } = 0;
        public string? SubMap { get; set; } = "";
        public AutoDoor? AutoDoor { get; set; } = new AutoDoor();
        public MotionInfo? MotionInfo { get; set; } = new MotionInfo();

        public int TagOfInPoint { get; set; } = -1;
        public int TagOfOutPoint { get; set; } = -1;
        public string? Region { get; set; } = "";

        

        [JsonIgnore]
        public bool IsCharge
        {
            get
            {
                return StationType == STATION_TYPE.Charge || StationType == STATION_TYPE.Charge_STK || StationType == STATION_TYPE.Charge_Buffer;
            }
        }

        [JsonIgnore]
        public bool IsEquipment
        {
            get
            {
                return StationType == STATION_TYPE.STK ||
                    StationType == STATION_TYPE.Charge_STK ||
                    StationType == STATION_TYPE.EQ ||
                    StationType == STATION_TYPE.EQ_LD ||
                    StationType == STATION_TYPE.EQ_ULD ||
                    StationType == STATION_TYPE.STK_LD ||
                    StationType == STATION_TYPE.STK_ULD ||
                    StationType == STATION_TYPE.Buffer ||
                    StationType == STATION_TYPE.Fire_EQ ||
                    StationType == STATION_TYPE.Elevator ||
                    StationType == STATION_TYPE.Elevator_LD ||
                    StationType == STATION_TYPE.TrayEQ ||
                    StationType == STATION_TYPE.TrayEQ_LD ||
                    StationType == STATION_TYPE.TrayEQ_ULD ||
                    StationType == STATION_TYPE.Charge_STK ||
                    StationType == STATION_TYPE.Charge_Buffer;
            }
        }

        [JsonIgnore]
        public bool IsSTK
        {
            get
            {
                return StationType == STATION_TYPE.STK || StationType == STATION_TYPE.STK_LD || StationType == STATION_TYPE.STK_ULD || StationType == STATION_TYPE.Charge_STK;
            }
        }
        /// <summary>
        /// 是否為EQLINK=>表示需要走磁導引
        /// </summary>
        [JsonIgnore]
        public bool IsEQLink
        {
            get
            {
                return StationType == STATION_TYPE.EQ ||
                   StationType == STATION_TYPE.EQ_ULD ||
                   StationType == STATION_TYPE.Charge_Buffer ||
                   StationType == STATION_TYPE.EQ_LD ||
                   StationType == STATION_TYPE.Charge_STK ||
                   StationType == STATION_TYPE.Charge ||
                   StationType == STATION_TYPE.STK ||
                   StationType == STATION_TYPE.STK_LD ||
                   StationType == STATION_TYPE.STK_ULD ||
                   StationType == STATION_TYPE.Elevator ||
                   StationType == STATION_TYPE.Elevator_LD ||
                   StationType == STATION_TYPE.TrayEQ ||
                   StationType == STATION_TYPE.TrayEQ_LD ||
                   StationType == STATION_TYPE.TrayEQ_ULD
                   ;
            }
        }

        /// <summary>
        /// 是否為十字路口或T字路口
        /// </summary>
        [JsonIgnore]
        public bool IsCross
        {
            get
            {
                return Target == null ? false : Target.Count != 0;
            }
        }
        [JsonIgnore]
        public clsPointRegistInfo RegistInfo = new clsPointRegistInfo();

        public PointF ToCoordination()
        {
            return new PointF((float)X, (float)Y);
        }
    }
}
