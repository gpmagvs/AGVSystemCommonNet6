﻿using AGVSystemCommonNet6.MAP;
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
        public Dictionary<int, clsPointRegistInfo> RegistedPoints { get; set; } = new Dictionary<int, clsPointRegistInfo>();

        /// <summary>
        /// 被系統交通管制中的路徑
        /// </summary>
        public Dictionary<string, MapPath> ControledPathesByTraffic { get; set; } = new Dictionary<string, MapPath>();
      
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
