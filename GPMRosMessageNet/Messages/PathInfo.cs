

using RosSharp.RosBridgeClient;


namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages
{
    public class PathInfo : Message
    {
        public const string RosMessageName = "gpm_msgs/PathInfo";

        /// <summary>
        /// 位置ID
        /// </summary>
        public ushort tagid { get; set; }
        /// <summary>
        /// 0 :主走道不避障, 1:主走道避障, 2:進 Bay不避障 , 3:進窄門不避障
        /// </summary>
        public ushort laserMode { get; set; }
        /// <summary>
        /// 0:可旋轉 ;1:不可旋轉
        /// </summary>
        public ushort direction { get; set; }
        /// <summary>
        /// 地圖名稱
        /// </summary>
        public string map { get; set; }
        public ushort changeMap { get; set; }
        /// <summary>
        /// 走行最大限速
        /// </summary>
        public double speed { get; set; }
        /// <summary>
        /// 超音波偵測設定距離(cm)
        /// </summary>
        public double ultrasonicDistance { get; set; }
        /// <summary>
        /// 0: 一般區域
        /// 1: 防火區
        /// </summary>
        public bool FireArea { get; set; } = false;
        /// <summary>
        /// 0: 一般區域
        /// 1: 防火設備
        /// </summary>  
        public bool FirePoint { get; set; } = false;
        public PathInfo()
        {
            this.tagid = 0;
            this.laserMode = 0;
            this.direction = 0;
            this.map = "";
            this.changeMap = 0;
            this.speed = 0.0;
            this.ultrasonicDistance = 0.0;
            this.FirePoint = false;
            this.FireArea = false;
        }

        public PathInfo(ushort tagid, ushort laserMode, ushort direction, string map, ushort changeMap, double speed, double ultrasonicDistance, bool FirePoint, bool FireArea)
        {
            this.tagid = tagid;
            this.laserMode = laserMode;
            this.direction = direction;
            this.map = map;
            this.changeMap = changeMap;
            this.speed = speed;
            this.ultrasonicDistance = ultrasonicDistance;
            this.FireArea = FireArea;
            this.FirePoint = FirePoint;
        }
    }
}
