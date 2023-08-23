using AGVSystemCommonNet6.GPMRosMessageNet.Actions;
using AGVSystemCommonNet6.GPMRosMessageNet.Messages;
using AGVSystemCommonNet6.Log;
using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Actionlib;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using static AGVSystemCommonNet6.GPMRosMessageNet.Actions.TaskCommandGoal;
using static AGVSystemCommonNet6.MAP.PathFinder;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    public class clsTaskDownloadMessage : MessageBase
    {
        public Dictionary<string, clsTaskDownloadData> Header { get; set; } = new Dictionary<string, clsTaskDownloadData>();

        public clsTaskDownloadData TaskDownload => this.Header[Header.Keys.First()];

    }


    public class clsTaskDownloadData
    {
        [JsonProperty("Task Name")]
        public string Task_Name { get; set; } = "";

        [JsonProperty("Task Simplex")]
        public string Task_Simplex
        {
            get
            {
                return $"{Task_Name}-{Task_Sequence}";
            }
        }

        [JsonProperty("Task Sequence")]
        public int Task_Sequence { get; set; }
        public clsMapPoint[] Trajectory { get; set; } = new clsMapPoint[0];

        [JsonProperty("Homing Trajectory")]
        public clsMapPoint[] Homing_Trajectory { get; set; } = new clsMapPoint[0];

        [JsonProperty("Action Type")]
        public ACTION_TYPE Action_Type { get; set; }

        public clsCST[] CST { get; set; } = new clsCST[0];
        public int Destination { get; set; }
        public int Height { get; set; }

        [JsonProperty("Escape Flag")]
        public bool Escape_Flag { get; set; }
        [JsonProperty("Station Type")]
        public STATION_TYPE Station_Type { get; set; }

        public clsMapPoint[] ExecutingTrajecory => Trajectory.Length != 0 ? Trajectory : Homing_Trajectory;
        public List<int> TagsOfTrajectory => ExecutingTrajecory.Select(pt => pt.Point_ID).ToList();
        public string OriTaskDataJson = "";
        public bool IsAfterLoadingAction = false;

        /// <summary>
        /// 送給車控CommandActionClient的 TaskCommandGoal
        /// </summary>
        public TaskCommandGoal RosTaskCommandGoal
        {
            get
            {
                return TaskDataToRosCommandGoal(this);
            }
        }

        public bool IsNeedHandshake
        {
            get
            {
                return Station_Type == STATION_TYPE.EQ | Station_Type == STATION_TYPE.STK | Station_Type == STATION_TYPE.Charge_STK;
            }
        }

        /// <summary>
        /// 把派車任務DTO轉成送給車控CommandActionClient的 TaskCommandGoal
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public static TaskCommandGoal TaskDataToRosCommandGoal(clsTaskDownloadData taskData)
        {
            try
            {
                clsMapPoint[] _ExecutingTrajecory = new clsMapPoint[0];
                _ExecutingTrajecory = taskData.ExecutingTrajecory;
                if (taskData.ExecutingTrajecory.Length == 0)
                {
                    throw new Exception("一般移動任務但是路徑長度為0");
                }
                int finalTag = taskData.Destination; //需要預先下發目標點(注意!並不是Trajection的最後一點,是整段導航任務的最後一點==>Trajection的最後一點如果跟Destination不同,表示AGVS在AGV行進途中會下發新的路徑過來)

                GUIDE_TYPE mobility_mode = taskData.Action_Type == ACTION_TYPE.None ? GUIDE_TYPE.SLAM : taskData.Action_Type == ACTION_TYPE.Discharge ? GUIDE_TYPE.Color_Tap_Backward : GUIDE_TYPE.Color_Tap_Forward;
                TaskCommandGoal goal = new TaskCommandGoal();
                goal.taskID = taskData.Task_Name;
                goal.finalGoalID = (ushort)finalTag;
                goal.mobilityModes = (ushort)mobility_mode;
                goal.planPath = new RosSharp.RosBridgeClient.MessageTypes.Nav.Path();

                var poses = _ExecutingTrajecory.Select(point => new PoseStamped()
                {
                    header = new RosSharp.RosBridgeClient.MessageTypes.Std.Header
                    {
                        seq = (uint)point.Point_ID,
                        frame_id = "map",
                        stamp = DateTime.Now.ToStdTime(),
                    },
                    pose = new Pose()
                    {
                        position = new Point(point.X, point.Y, 0),
                        orientation = point.Theta.ToQuaternion()
                    }
                }).ToArray();
                var pathInfo = _ExecutingTrajecory.Select(point => new PathInfo()
                {
                    tagid = (ushort)point.Point_ID,
                    laserMode = (ushort)point.Laser,
                    direction = (ushort)point.Control_Mode.Spin,
                    map = point.Map_Name,
                    changeMap = 0,
                    speed = point.Speed,
                    ultrasonicDistance = point.UltrasonicDistance
                }).ToArray();


                if (taskData.IsAfterLoadingAction) //Loading 結束
                {
                    poses = poses.Reverse().ToArray();
                    pathInfo = pathInfo.Reverse().ToArray();
                    goal.finalGoalID = (ushort)taskData.Homing_Trajectory.First().Point_ID;
                    goal.mobilityModes = (ushort)GUIDE_TYPE.Color_Tap_Backward;

                }

                goal.planPath.poses = poses;
                goal.pathInfo = pathInfo;
                return goal;
            }
            catch (Exception ec)
            {
                LOG.ERROR("RosTaskCommandGoal_取得ROS任務Goal物件時發生錯誤", ec);
                return null;
            }

        }
        /// <summary>
        /// 是否為切片任務(軌跡最後一點TAG不為目標點TAG)
        /// </summary>
        public bool IsTaskSegmented => ExecutingTrajecory.Length == 0 ? false : ExecutingTrajecory.Last().Point_ID != Destination;

        public clsTaskDownloadData TurnToBackTaskData()
        {
            var taskData = JsonConvert.DeserializeObject<clsTaskDownloadData>(this.ToJson());
            taskData.IsAfterLoadingAction = true;
            taskData.Destination = Homing_Trajectory.First().Point_ID;
            // taskData.Homing_Trajectory = taskData.Homing_Trajectory.Reverse().ToArray();
            return taskData;
        }
        [JsonIgnore]
        public clsPathInfo TrafficInfo { get; set; } = new clsPathInfo();
    }


    public class clsTaskDownloadAckMessage : MessageBase
    {
        public Dictionary<string, SimpleRequestResponseWithTimeStamp> Header = new Dictionary<string, SimpleRequestResponseWithTimeStamp>();
    }
    public class clsTaskDownloadAckData
    {

        [JsonProperty("Return Code")]
        public int ReturnCode { get; set; }
    }

    public class clsMapPoint
    {
        public clsMapPoint() { }
        public clsMapPoint(int index)
        {
            this.index = index;
        }
        [NonSerialized]
        public int index;
        [JsonProperty("Point ID")]
        public int Point_ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Theta { get; set; }
        public int Laser { get; set; }
        public double Speed { get; set; }

        [JsonProperty("Map Name")]
        public string Map_Name { get; set; } = "";
        [JsonProperty("Auto Door")]
        public clsAutoDoor Auto_Door { get; set; } = new clsAutoDoor();
        [JsonProperty("Control Mode")]
        public clsControlMode Control_Mode { get; set; } = new clsControlMode();
        public double UltrasonicDistance { get; set; } = 0;
    }
    public class clsAutoDoor
    {
        [JsonProperty("Key Name")]
        public string Key_Name { get; set; }
        [JsonProperty("Key Password")]
        public string Key_Password { get; set; }
    }
    public class clsControlMode
    {
        public int Dodge { get; set; }
        public int Spin { get; set; }
    }

    public class clsCST
    {
        [JsonProperty("CST ID")]
        public string CST_ID { get; set; }
        [JsonProperty("CST Type")]
        public int CST_Type { get; set; }
    }

}
