using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Configuration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public enum TransferStage
    {
        /// <summary>
        /// 到轉運站放貨
        /// </summary>
        MoveToTransferStationLoad,
        /// <summary>
        /// 到轉運站取貨
        /// </summary>
        MoveToTransferStationUnload,
        NO_Transfer
    }
    public enum VehicleMovementStage
    {
        Not_Start_Yet = 0,
        Traveling = 1,
        Traveling_To_Source = 2,
        Traveling_To_Destine = 3,
        WorkingAtSource = 4,
        WorkingAtDestination = 5,
        WorkingAtChargeStation = 6,
        LeaveFrom_WorkStation = 7,
        LeaveFrom_ChargeStation = 8,
        ParkAtWorkStation = 9,
        MeasureInBay = 10,
        LoadingAtTransferStation,
        UnloadingAtTransferStation,
        AvoidPath,
        AvoidPath_Park,
        Traveling_To_Region_Wait_Point,
        Completed = 500,
    }

    [Index(nameof(RecieveTime))]
    [Index(nameof(StartTime))]
    [Index(nameof(FinishTime))]
    public class clsTaskDto
    {
        [Required]
        public ACTION_TYPE Action { get; set; }
        public DateTime RecieveTime { get; set; }

        [NotMapped]
        public string RecieveTime_Formated
        {
            get
            {
                return RecieveTime.ToString("HH:mm:ss");
            }
        }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 取料時間
        /// </summary>
        public DateTime UnloadTime { get; set; }
        /// <summary>
        /// 放料時間
        /// </summary>
        public DateTime LoadTime { get; set; }

        /// <summary>
        /// 任務開始到結束車子行走的總里程
        /// </summary>
        public double TotalMileage { get; set; } = 0;

        [NotMapped]
        public string FinishTime_Formated
        {
            get
            {
                return FinishTime.ToString("HH:mm:ss");
            }
        }
        [Key]
        [MaxLength(50)]
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

        /// <summary>
        /// 負責第二段搬運至終點(需轉運的搬運任務執行時)的AGV名稱
        /// </summary>

        [AllowNull]
        public string TransferToDestineAGVName { get; set; } = string.Empty;

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


        [NotMapped]
        public bool IsFromAGV => From_Station.Contains("AGV");

        [NotMapped]
        public string From_Station_Display { get; set; } = string.Empty;

        public int From_Station_Tag => int.TryParse(From_Station, out int tag) ? tag : -1;

        [Required]
        public AGV_TYPE From_Station_AGV_Type { get; set; } = AGV_TYPE.Any;
        [Required]
        public string From_Slot { get; set; } = "-1";

        [Required]
        public string To_Station { get; set; } = "-1";
        [NotMapped]
        public string To_Station_Display { get; set; } = string.Empty;

        [Required]
        public AGV_TYPE To_Station_AGV_Type { get; set; } = AGV_TYPE.Any;
        public int To_Station_Tag => int.TryParse(To_Station, out int tag) ? tag : -1;

        /// <summary>
        /// 起始位置
        /// </summary>
        public int StartLocationTag { get; set; } = 0;

        [NotMapped]
        public string StartLocationDisplay { get; set; } = string.Empty;
        public int TransferToTag { get; set; } = -1;
        public int TransferFromTag { get; set; } = -1;

        [Required]
        public string To_Slot { get; set; } = "-1";

        public int CST_TYPE { get; set; } = 200;

        public string Carrier_ID { get; set; } = "";

        /// <summary>
        /// 車載真實上報ID
        /// </summary>
        public string Actual_Carrier_ID { get; set; } = "";

        /// <summary>
        /// 任務需要換車時的中繼站點Tag
        /// </summary>
        [MaybeNull]
        public int ChangeAGVMiddleStationTag { get; set; } = 0;

        [Required]
        public int Height { get; set; } = 0;
        /// <summary>
        /// 優先度
        /// </summary>
        /// 
        [Required]
        public int Priority { get; set; } = 50;

        /// <summary>
        /// 訂單是否具有世界高的權限
        /// </summary>
        [Required]
        public bool IsHighestPriorityTask { get; set; } = false;

        public bool IsTrafficControlTask { get; set; } = false;
        /// <summary>
        /// by pass 功能:
        /// 1.EQ IO狀態
        /// 2.搬運任務起終點可接受貨物類型
        /// </summary>
        public bool bypass_eq_status_check { get; set; } = false;


        [AllowNull]
        public bool need_change_agv { get; set; } = false;
        /// <summary>
        /// 一般任務:0, 
        /// ex:A->轉->B
        /// A->轉:1 , 轉->B:2
        /// </summary>
        public int transfer_task_stage { get; set; } = 0;


        public string soucePortID { get; set; } = "";
        public string sourceZoneID { get; set; } = "";

        /// <summary>
        /// 目的地Port ID
        /// </summary>
        /// <value></value>
        public string destinePortID { get; set; } = "";

        /// <summary>
        /// 目的區域ID
        /// </summary>
        /// <value></value>
        public string destineZoneID { get; set; } = "";

        /// <summary>
        /// 是否從MCS派車
        /// </summary>
        /// <value></value>
        public bool isFromMCS { get; set; } = false;
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

            if (dto.DesignatedAGVName != null)
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

            CST_TYPE = dto.CST_TYPE;
            need_change_agv = dto.need_change_agv;
            transfer_task_stage = dto.transfer_task_stage;
            UnloadTime = dto.UnloadTime;
            LoadTime = dto.LoadTime;
            TotalMileage = dto.TotalMileage;
            StartLocationTag = dto.StartLocationTag;
            Actual_Carrier_ID = dto.Actual_Carrier_ID;
        }

    }
}
