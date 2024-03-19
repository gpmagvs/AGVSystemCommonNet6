

using RosSharp.RosBridgeClient;


namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages
{
    public class PathInfo : Message
    {
        public const string RosMessageName = "gpm_msgs/PathInfo";

        /// <summary>
        /// ��mID
        /// </summary>
        public ushort tagid { get; set; }
        /// <summary>
        /// 0 :�D���D���׻�, 1:�D���D�׻�, 2:�i Bay���׻� , 3:�i�������׻�
        /// </summary>
        public ushort laserMode { get; set; }
        /// <summary>
        /// 0:�i���� ;1:���i����
        /// </summary>
        public ushort direction { get; set; }
        /// <summary>
        /// �a�ϦW��
        /// </summary>
        public string map { get; set; }
        public ushort changeMap { get; set; }
        /// <summary>
        /// ����̤j���t
        /// </summary>
        public double speed { get; set; }
        /// <summary>
        /// �W���i�����]�w�Z��(cm)
        /// </summary>
        public double ultrasonicDistance { get; set; }
        /// <summary>
        /// 0: �@��ϰ�
        /// 1: ������
        /// </summary>
        public bool FireArea { get; set; } = false;
        /// <summary>
        /// 0: �@��ϰ�
        /// 1: �����]��
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
