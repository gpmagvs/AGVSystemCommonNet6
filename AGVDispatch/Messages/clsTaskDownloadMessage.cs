using AGVSystemCommonNet6.GPMRosMessageNet.Actions;
using AGVSystemCommonNet6.GPMRosMessageNet.Messages;
using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Vehicle_Control.Models;
using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Actionlib;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System.Runtime.CompilerServices;
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
        public string Task_Simplex { get; set; } = "";

        [JsonProperty("Task Sequence")]
        public int Task_Sequence { get; set; }
        public clsMapPoint[] Trajectory { get; set; } = new clsMapPoint[0];

        [JsonProperty("Homing Trajectory")]
        public clsMapPoint[] Homing_Trajectory { get; set; } = new clsMapPoint[0];

        [JsonProperty("Action Type")]
        public ACTION_TYPE Action_Type { get; set; }

        public clsCST[] CST { get; set; } = new clsCST[0];
        public int Destination { get; set; }
        /// <summary>
        /// 在工作站(設備、Rack...)進行取放貨時，Z軸需上升至的層數(Zero-base, 0->第一層 ,1->第二層...)
        /// </summary>
        public int Height { get; set; } = 0;

        [JsonProperty("Escape Flag")]
        public bool Escape_Flag { get; set; }
        [JsonProperty("Station Type")]
        public STATION_TYPE Station_Type { get; set; }
        public clsMapPoint[] ExecutingTrajecory => Trajectory.Length != 0 ? Trajectory : Homing_Trajectory;
        public List<int> TagsOfTrajectory => ExecutingTrajecory.Select(pt => pt.Point_ID).ToList();
        public string OriTaskDataJson = "";
        public bool GoTOHomePoint = false;

        public clsMapPoint InpointOfEnterWorkStation { get; set; } = new clsMapPoint();
        public clsMapPoint OutPointOfLeaveWorkstation { get; set; } = new clsMapPoint();

        public bool IsLDULDAction()
        {
            return Action_Type == ACTION_TYPE.Load ||
                Action_Type == ACTION_TYPE.LoadAndPark ||
                Action_Type == ACTION_TYPE.Unload;
        }
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
                return Station_Type == STATION_TYPE.EQ || Station_Type == STATION_TYPE.STK || Station_Type == STATION_TYPE.Charge_STK;
            }
        }
        public bool HasCargo { get; set; } = false;

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

                GUIDE_TYPE mobility_mode = DetermineGuideType(taskData.Action_Type);
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
                    laserMode = (ushort)point.Control_Mode.Dodge,
                    direction = (ushort)point.Control_Mode.Spin,
                    map = point.Map_Name,
                    changeMap = 0,
                    speed = point.Speed,
                    ultrasonicDistance = point.UltrasonicDistance
                }).ToArray();

                if (taskData.GoTOHomePoint) //Loading 結束
                {

                    poses = poses.Reverse().ToArray();
                    pathInfo = pathInfo.Reverse().ToArray();
                    goal.finalGoalID = (ushort)taskData.Homing_Trajectory.First().Point_ID;
                    goal.mobilityModes = (ushort)DetermineGuideType(taskData.Action_Type);
                }

                if (taskData.Action_Type == ACTION_TYPE.ExchangeBattery)
                {
                    poses = poses.Skip(1).ToArray();
                    pathInfo = pathInfo.Skip(1).ToArray();
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
        /// 決定導航移動導引模式
        /// </summary>
        /// <param name="_Action_Type"></param>
        /// <returns></returns>
        private static GUIDE_TYPE DetermineGuideType(ACTION_TYPE _Action_Type)
        {
            if (_Action_Type == ACTION_TYPE.None || _Action_Type == ACTION_TYPE.Measure || _Action_Type == ACTION_TYPE.Escape)
                return GUIDE_TYPE.SLAM;
            else
            {
                return _Action_Type == ACTION_TYPE.ExchangeBattery ? GUIDE_TYPE.AR_TAG : GUIDE_TYPE.Color_Tap;
            }
        }
        /// <summary>
        /// 回Home點的任務 (mobility=2)
        /// </summary>
        /// <returns></returns>
        public clsTaskDownloadData CreateGoHomeTaskDownloadData()
        {
            var taskData = JsonConvert.DeserializeObject<clsTaskDownloadData>(this.ToJson());

            bool isBackToEntryPoint = Action_Type != ACTION_TYPE.Measure && Action_Type != ACTION_TYPE.ExchangeBattery ?
                                        true : taskData.OutPointOfLeaveWorkstation.Point_ID == taskData.InpointOfEnterWorkStation.Point_ID;

            taskData.IsLocalTask = IsLocalTask;
            taskData.IsEQHandshake = IsEQHandshake;
            taskData.IsActionFinishReported = IsActionFinishReported;
            if (isBackToEntryPoint)
            {
                taskData.GoTOHomePoint = true;
                taskData.Destination = Homing_Trajectory.First().Point_ID;
            }
            else
            {
                taskData.Destination = taskData.OutPointOfLeaveWorkstation.Point_ID;
                taskData.Homing_Trajectory = new clsMapPoint[2]
                {
                     taskData.Homing_Trajectory.Last(),
                     taskData.OutPointOfLeaveWorkstation,
                };
            }
            return taskData;
        }

        public clsOrderInfo OrderInfo { get; set; } = new clsOrderInfo();

        private bool _IsActionFinishReported = true;
        public bool IsActionFinishReported
        {
            get => _IsActionFinishReported;
            set
            {
                if (_IsActionFinishReported != value)
                {
                    _IsActionFinishReported = value;
                    LOG.INFO($"{Task_Name} Action Finish 上報狀態:{(value ? "已上報派車完成" : "未上報派車完成")}", false);
                }
            }
        }
        public bool IsEQHandshake = false;

        /// <summary>
        /// 是否為本地任務
        /// </summary>
        public bool IsLocalTask = false;

        public bool IsSegmentTask
        {
            get
            {
                try
                {
                    return ExecutingTrajecory != null && (ExecutingTrajecory?.Length) != 0 && Destination != ExecutingTrajecory?.Last().Point_ID;
                }
                catch (Exception ex)
                {
                    LOG.WARN($" IsSegmentTask get fail:{ex.Message}");
                    return false;
                }
            }
        }

        [NonSerialized]
        public List<clsVibrationRecord> VibrationRecords = new List<clsVibrationRecord>();


        /// <summary>
        /// 任務訂單的資訊
        /// </summary>
        public class clsOrderInfo
        {
            public string DestineName { get; set; } = "";
            public string SourceName { get; set; } = "";

            public int DestineTag { get; set; } = 0;
            public int SourceTag { get; set; } = 0;
            public bool IsTransferTask { get; set; } = false;
            public ACTION_TYPE ActionName { get; set; } = ACTION_TYPE.NoAction;

            public delegate bool GetPortExistStatusDelegate();
            public static GetPortExistStatusDelegate OnGetPortExistStatus;

            public string DisplayText
            {
                get
                {
                    if (ActionName == ACTION_TYPE.Carry)
                    {
                        bool cargoOnAGV = false;
                        if (OnGetPortExistStatus != null)
                        {
                            cargoOnAGV = OnGetPortExistStatus();
                        }
                        return cargoOnAGV ? $"前往[{DestineName}]放貨(來源-[{SourceName}])" : $"前往[{SourceName}]取貨(目的地-{DestineName})";
                    }
                    else if (ActionName == ACTION_TYPE.Load)
                    {
                        return !IsTransferTask ? $"{DestineName}放貨" : $"[{DestineName}] 放貨(來源-{SourceName})";
                    }
                    else if (ActionName == ACTION_TYPE.Unload)
                    {
                        return !IsTransferTask ? $"{DestineName}取貨" : $"[{SourceName}] 取貨(目的地-{DestineName})";
                    }
                    else if (ActionName == ACTION_TYPE.Charge)
                    {
                        return $"前往[{DestineName}] 充電";
                    }
                    else if (ActionName == ACTION_TYPE.Unpark || ActionName == ACTION_TYPE.Discharge)
                    {
                        return $"退出充電站前往[{(IsTransferTask ? SourceName : DestineName)}]";
                    }
                    else if (ActionName == ACTION_TYPE.None)
                    {
                        return $"移動至[{DestineName}]";
                    }
                    return "";
                }
            }

        }
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
        /// <summary>
        /// 車控用雷射
        /// </summary>
        public int Dodge { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Spin { get; set; }
    }

    public class clsCST
    {
        [JsonProperty("CST ID")]
        public string CST_ID { get; set; }
        [JsonProperty("CST Type")]
        public CST_TYPE CST_Type { get; set; }
    }

}
