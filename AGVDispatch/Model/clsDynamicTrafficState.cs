using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.MAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.AGVDispatch.Model
{
    /// <summary>
    /// 交通動態資訊
    /// </summary>
    public class clsDynamicTrafficState
    {
        public enum TRAFFIC_ACTION
        {
            PASS,
            WAIT,
            WAIT_TRAFFIC_STATE_NOT_UPDATE_YET,
        }
        public Dictionary<string, clsAGVTrafficState> AGVTrafficStates { get; set; } = new Dictionary<string, clsAGVTrafficState>();
        public List<MapPoint> RegistedPoints { get; set; } = new List<MapPoint>();
        public TRAFFIC_ACTION GetTrafficStatusByTag(string agv_name, int tagNumber)
        {
            if (!AGVTrafficStates.TryGetValue(agv_name, out clsAGVTrafficState agv_state))
                return TRAFFIC_ACTION.WAIT;

            MapPoint? toPoint = RegistedPoints.FirstOrDefault(pt => pt.TagNumber == tagNumber);
            if (toPoint == null)
            {
                return TRAFFIC_ACTION.PASS;
            }

            if (toPoint.RegistInfo.IsRegisted)
            {
                if (toPoint.RegistInfo.RegisterAGVName != agv_name)
                {
                    return TRAFFIC_ACTION.WAIT;
                }
                else
                    return TRAFFIC_ACTION.PASS;
            }
            else
                return TRAFFIC_ACTION.PASS;
        }
    }

    /// <summary>
    /// 車輛的動態
    /// </summary>
    public class clsAGVTrafficState
    {
        public string AGVName { get; set; } = "";

        /// <summary>
        /// AGV當前車速
        /// </summary>
        public double AGVSpeed { get; set; } = 0;

        /// <summary>
        /// 任務優先等級
        /// </summary>
        public int TaskLevel { get; set; } = 1;

        /// <summary>
        /// 接收任務的時間
        /// </summary>
        public DateTime TaskRecieveTime { get; set; } = DateTime.MinValue;
        /// <summary>
        /// AGV 當前狀態
        /// </summary>
        public MAIN_STATUS AGVStatus { get; set; } = MAIN_STATUS.DOWN;

        public MapPoint CurrentPosition { get; set; } = new MapPoint();

        public bool IsOnline { get; set; } = false;

        /// <summary>
        /// 剩餘的導航路徑
        /// </summary>
        public List<MapPoint> RemainNavTrajectory
        {
            get
            {
                if (PlanningNavTrajectory.Count == 0)
                    return new List<MapPoint>();
                var currentPositionIndex = PlanningNavTrajectory.IndexOf(CurrentPosition);
                return PlanningNavTrajectory.Skip(currentPositionIndex).Take(PlanningNavTrajectory.Count - currentPositionIndex).ToList();
            }
        }
        /// <summary>
        /// 規劃的導航路徑
        /// </summary>
        public List<MapPoint> PlanningNavTrajectory { get; set; } = new List<MapPoint> { };

    }
}
