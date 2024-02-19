﻿using AGVSystemCommonNet6;
using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6
{
    public class clsAGVStateDto
    {
        [Key]
        public string AGV_Name { get; set; }
        public bool Enabled { get; set; }
        public string AGV_Description { get; set; } = "";
        /// <summary>
        /// 所屬車隊
        /// </summary>
        public VMS_GROUP Group { get; set; }
        public AGV_MODEL Model { get; set; }
        public MAIN_STATUS MainStatus { get; set; }
        public ONLINE_STATE OnlineStatus { get; set; }
        public string CurrentLocation { get; set; } = "";
        public string CurrentCarrierID { get; set; } = "";
        public double BatteryLevel_1 { get; set; } = 0;
        public double BatteryLevel_2 { get; set; } = 0;
        public bool Connected { get; set; }
        public bool IsCharging { get; set; }
        public string TaskName { get; set; } = "";

        public TASK_RUN_STATUS TaskRunStatus { get; set; }
        public ACTION_TYPE TaskRunAction { get; set; }
        /// <summary>
        /// 當前執行的任務動作
        /// </summary>
        public ACTION_TYPE CurrentAction { get; set; }
        public double Theta { get; set; }
        public VehicleMovementStage TransferProcess { get; set; }

        [NotMapped] // 此欄位不會寫入資料表
        public DateTime TaskETA { get; set; } = DateTime.MaxValue;
        [NotMapped]// 此欄位不會寫入資料表
        public bool IsExecutingOrder { get; set; } = false;
        [NotMapped]// 此欄位不會寫入資料表
        public double VehicleLength { get; set; } = 145.0;
        [NotMapped]// 此欄位不會寫入資料表
        public double VehicleWidth { get; set; } = 70;
        public void Update(clsAGVStateDto entity)
        {
            AGV_Description = entity.AGV_Description;
            Model = entity.Model;
            MainStatus = entity.MainStatus;
            OnlineStatus = entity.OnlineStatus;
            CurrentLocation = entity.CurrentLocation;
            CurrentCarrierID = entity.CurrentCarrierID;
            BatteryLevel_1 = entity.BatteryLevel_1;
            BatteryLevel_2 = entity.BatteryLevel_2;
            TaskName = entity.TaskName;
            TaskRunStatus = entity.TaskRunStatus;
            TaskRunAction = entity.TaskRunAction;
            Theta = entity.Theta;
            Connected = entity.Connected;
            CurrentAction = entity.CurrentAction;
            TransferProcess = entity.TransferProcess;
            IsCharging = entity.IsCharging;
        }

        public bool HasChanged(clsAGVStateDto newState)
        {
            var clone_ = this.clone();
            var clone_new = newState.clone();
            clone_.Theta = clone_new.Theta = 0;
            bool isdifferent = clone_.ToJson() != clone_new.ToJson();
            return isdifferent;
        }

        private clsAGVStateDto clone()
        {
            return JsonConvert.DeserializeObject<clsAGVStateDto>(this.ToJson());
        }
    }
}
