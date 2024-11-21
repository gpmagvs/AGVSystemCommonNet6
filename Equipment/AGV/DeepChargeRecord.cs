using AGVSystemCommonNet6.AGVDispatch.Messages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AGVSystemCommonNet6.Equipment.AGV
{
    /// <summary>
    /// 儲存深充任務的記錄
    /// </summary>
    public class DeepChargeRecord
    {

        public enum DEEP_CHARGE_TRIGGER_MOMENT
        {
            MANUAL, //手動觸發, 例如人工點選
            AUTO,   //自動觸發
        }

        /// <summary>
        /// 深充任務訂單接收的時間
        /// </summary>
        [Key]
        [Required]
        public DateTime OrderRecievedTime { get; set; }
        /// <summary>
        /// 車輛ID
        /// </summary>
        public string AGVID { get; set; } = "";

        /// <summary>
        /// 深充任務的ID
        /// </summary>
        public string TaskID { get; set; } = string.Empty;
        /// <summary>
        /// 任務狀態
        /// </summary>
        public TASK_RUN_STATUS OrderStatus { get; set; } = TASK_RUN_STATUS.UNKNOWN;

        /// <summary>
        /// 深充任務訂單產生的時機
        /// </summary>
        public DEEP_CHARGE_TRIGGER_MOMENT TriggerBy { get; set; } = DEEP_CHARGE_TRIGGER_MOMENT.MANUAL;

        /// <summary>
        /// 結束深充的觸發來源
        /// </summary>
        public DEEP_CHARGE_TRIGGER_MOMENT EndBy { get; set; } = DEEP_CHARGE_TRIGGER_MOMENT.MANUAL;


        /// <summary>
        /// 深充任務的開始時間
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 深充任務的結束時間
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 起始電量
        /// </summary>
        public double BeginBatLv { get; set; } = 0;

        /// <summary>
        /// 最終電量
        /// </summary>
        public double FinalBatLv { get; set; } = 0;

        /// <summary>
        /// 起始電壓
        /// </summary>
        public double BeginVoltage { get; set; } = 0;
        /// <summary>
        /// 最終電壓
        /// </summary>
        public double FinalVoltage { get; set; } = 0;
        /// <summary>
        /// 深充任務的充電時間
        /// </summary>
        public double ChargeTime { get; set; } = 0;

        /// <summary>
        /// [JSON] 儲存深充其間的電池數據
        /// </summary>
        public string HistoryDataJson { get; set; } = string.Empty;
    }
}
