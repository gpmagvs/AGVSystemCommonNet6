﻿using AGVSystemCommonNet6.AGVDispatch.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AGVSystemCommonNet6.AGVDispatch
{

    public class clsTaskDto
    {
        public DateTime RecieveTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        [Key]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        public TASK_RUN_STATUS State { get; set; } = TASK_RUN_STATUS.WAIT;
        public string StateName
        {
            get
            {
                switch (State)
                {
                    case TASK_RUN_STATUS.WAIT:
                        return "等待";
                    case TASK_RUN_STATUS.NAVIGATING:
                        return "執行中";
                    case TASK_RUN_STATUS.ACTION_FINISH:
                        return "完成";
                    case TASK_RUN_STATUS.FAILURE:
                        return "失敗";

                    case TASK_RUN_STATUS.CANCEL:
                        return "取消";
                    default:
                        return "等待";
                }
            }

        }

        /// <summary>
        /// 派工人員名稱
        /// </summary>
        /// 
        public string DispatcherName { get; set; } = string.Empty;

        /// <summary>
        /// 失敗原因
        /// </summary>
        /// 
        public string FailureReason { get; set; } = string.Empty;

        public string DesignatedAGVName { get; set; } = "";


        [Required]
        public ACTION_TYPE Action { get; set; }
        public string ActionName
        {
            get
            {
                switch (Action)
                {
                    case ACTION_TYPE.None:
                        return "移動";
                    case ACTION_TYPE.Load:
                        return "放貨";
                    case ACTION_TYPE.Unload:
                        return "取貨";
                    case ACTION_TYPE.Charge:
                        return "充電";
                    case ACTION_TYPE.Carry:
                        return "搬運";
                    case ACTION_TYPE.Measure:
                        return "量測";
                    case ACTION_TYPE.ExchangeBattery:
                        return "交換電池";
                    default:
                        return Action.ToString();
                }
            }
        }

        [Required]
        public string From_Station { get; set; } = "-1";

        [Required]
        public string From_Slot { get; set; } = "-1";

        [Required]
        public string To_Station { get; set; } = "-1";

        [Required]
        public string To_Slot { get; set; } = "-1";

        public string Carrier_ID { get; set; } = "";
        /// <summary>
        /// 優先度
        /// </summary>
        /// 
        [Required]
        public int Priority { get; set; } = 50;

        public bool IsTrafficControlTask { get; set; } = false;
        [NotMapped]
        public bool bypass_eq_status_check { get; set; } = false;

        public void Update(clsTaskDto dto)
        {
            if (dto.RecieveTime != default)
                RecieveTime = dto.RecieveTime;

            if (dto.StartTime != default)
                StartTime = dto.StartTime;

            if (dto.FinishTime != default)
                FinishTime = dto.FinishTime;

            if (!string.IsNullOrWhiteSpace(dto.TaskName))
                TaskName = dto.TaskName;
            // State是一个枚举，你可能总是想更新它，即使它是默认值。
            State = dto.State;

            if (!string.IsNullOrWhiteSpace(dto.DispatcherName))
                DispatcherName = dto.DispatcherName;

            if (!string.IsNullOrWhiteSpace(dto.FailureReason))
                FailureReason = dto.FailureReason;

            if (!string.IsNullOrWhiteSpace(dto.DesignatedAGVName))
                DesignatedAGVName = dto.DesignatedAGVName;

            Action = dto.Action;

            if (!string.IsNullOrWhiteSpace(dto.From_Station))
                From_Station = dto.From_Station;

            if (!string.IsNullOrWhiteSpace(dto.From_Slot))
                From_Slot = dto.From_Slot;

            if (!string.IsNullOrWhiteSpace(dto.To_Station))
                To_Station = dto.To_Station;

            if (!string.IsNullOrWhiteSpace(dto.To_Slot))
                To_Slot = dto.To_Slot;

            if (!string.IsNullOrWhiteSpace(dto.Carrier_ID))
                Carrier_ID = dto.Carrier_ID;
            Priority = dto.Priority;
            IsTrafficControlTask = dto.IsTrafficControlTask;
            bypass_eq_status_check = dto.bypass_eq_status_check;
        }

    }
}