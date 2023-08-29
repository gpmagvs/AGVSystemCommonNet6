using AGVSystemCommonNet6.AGVDispatch.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class MapPoint
    {
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
        /// 註冊點(,逗點分隔)
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? InvolvePoint { get; set; } = "";
        /// <summary>
        /// 新版的註冊點屬性
        /// </summary>
        public int[] RegistsPointIndexs { get; set; } = new int[0];
        public STATION_TYPE StationType { get; set; }
        public int LsrMode { get; set; }
        public double Speed { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Bay { get; set; } = "";
        public Graph Graph { get; set; } = new Graph();
        /// <summary>
        /// Key Point Index, value:權重
        /// </summary>
        public Dictionary<int, double>? Target { get; set; } = new Dictionary<int, double>();
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? DodgeMode { get; set; } = "";
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? SpinMode { get; set; } = "";
        public string? SubMap { get; set; } = "";
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public AutoDoor? AutoDoor { get; set; } = new AutoDoor();
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MotionInfo? MotionInfo { get; set; } = new MotionInfo();
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]

        public string? Region { get; set; } = "";



        [JsonIgnore]
        public bool IsCharge
        {
            get
            {
                return StationType == STATION_TYPE.Charge | StationType == STATION_TYPE.Charge_STK | StationType == STATION_TYPE.Charge_Buffer;
            }
        }

        [JsonIgnore]
        public bool IsEquipment
        {
            get
            {
                return StationType == STATION_TYPE.STK |
                    StationType == STATION_TYPE.EQ |
                    StationType == STATION_TYPE.EQ_LD |
                    StationType == STATION_TYPE.EQ_ULD |
                    StationType == STATION_TYPE.STK_LD |
                    StationType == STATION_TYPE.STK_ULD |
                    StationType == STATION_TYPE.Fire_EQ |
                    StationType == STATION_TYPE.Elevator |
                    StationType == STATION_TYPE.Elevator_LD |
                    StationType == STATION_TYPE.TrayEQ |
                    StationType == STATION_TYPE.TrayEQ_LD |
                    StationType == STATION_TYPE.TrayEQ_ULD;
            }
        }

        [JsonIgnore]
        public bool IsSTK
        {
            get
            {
                return StationType == STATION_TYPE.STK | StationType == STATION_TYPE.STK_LD | StationType == STATION_TYPE.STK_ULD | StationType == STATION_TYPE.Charge_STK;
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
                return StationType == STATION_TYPE.EQ |
                   StationType == STATION_TYPE.EQ_ULD |
                   StationType == STATION_TYPE.Charge_Buffer |
                   StationType == STATION_TYPE.EQ_LD |
                   StationType == STATION_TYPE.Charge_STK |
                   StationType == STATION_TYPE.Charge |
                   StationType == STATION_TYPE.STK |
                   StationType == STATION_TYPE.STK_LD |
                   StationType == STATION_TYPE.STK_ULD |
                   StationType == STATION_TYPE.Elevator |
                   StationType == STATION_TYPE.Elevator_LD |
                   StationType == STATION_TYPE.TrayEQ |
                   StationType == STATION_TYPE.TrayEQ_LD |
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
        public clsMapPoiintRegist? RegistInfo { get; set; } = new clsMapPoiintRegist();

        /// <summary>
        /// 註冊這個站點
        /// </summary>
        public bool TryRegistPoint(string AGVName, out clsMapPoiintRegist registInfo)
        {
            registInfo = null;
            if (RegistInfo == null)
                RegistInfo = new clsMapPoiintRegist();

            if (RegistInfo.IsRegisted && AGVName != "System")
            {
                return false;
            }
            RegistInfo = new clsMapPoiintRegist()
            {
                RegistTime = DateTime.Now,
                RegisterAGVName = AGVName
            };
            registInfo = RegistInfo;
            RegistInfo.IsRegisted = true;
            Console.WriteLine($"{AGVName} Regist Tag_{TagNumber}");
            return true;
        }

        public bool TryUnRegistPoint(string name, out string errMsg)
        {
            errMsg = "";

            if (RegistInfo == null)
                RegistInfo = new clsMapPoiintRegist();
            if (!RegistInfo.IsRegisted)
                return true;
            if (name == "System" | RegistInfo.RegisterAGVName == name)
            {
                Console.WriteLine($"{name} UnRegist Tag_{TagNumber}");
                RegistInfo.RegisterAGVName = "";
                RegistInfo.IsRegisted = false;
                return true;
            }
            else
            {
                errMsg = "非註冊該點位之AGV無法解除註冊";
                return false;
            }
        }
    }
}
