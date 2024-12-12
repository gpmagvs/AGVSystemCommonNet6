using AGVSystemCommonNet6.Alarm.SECS_Alarm_Code.Enums;

namespace AGVSystemCommonNet6.Alarm.SECS_Alarm_Code
{
    public abstract class SECSHCACKAlarmCodeMapper
    {

        public class MapResult
        {
            public readonly byte Code = 0x00;

            public readonly string Description = string.Empty;

            public MapResult(byte code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        public virtual Dictionary<ALARMS, byte> MappingTable { get; set; } = new Dictionary<ALARMS, byte>()
        {
            {ALARMS.SYSTEM_ERROR,(byte) HCACK_RETURN_CODE_YELLOW.System_Exception},
            {ALARMS.EQ_TAG_NOT_EXIST_IN_CURRENT_MAP,(byte) HCACK_RETURN_CODE_YELLOW.Cannot_Find_Source_Port},
            {ALARMS.CANNOT_DISPATCH_CARRY_TASK_WHEN_AGV_HAS_CARGO,(byte) HCACK_RETURN_CODE_YELLOW.AGV_Cargo_Status_Mismatch_With_Command},
            {ALARMS.AGV_NO_Carge_Cannot_Transfer_Cargo_From_AGV_To_Desinte,(byte) HCACK_RETURN_CODE_YELLOW.AGV_Cargo_Status_Mismatch_With_Command},
            {ALARMS.EQ_LOAD_REQUEST_ON_BUT_HAS_CARGO,(byte) HCACK_RETURN_CODE_YELLOW.Detect_Carrier_Exist_For_Destination_Port},
            {ALARMS.EQ_LOAD_REQUEST_IS_NOT_ON,(byte) HCACK_RETURN_CODE_YELLOW.Signal_Is_Not_Load_Request_For_Destination_Port},
            {ALARMS.EQ_UNLOAD_REQUEST_IS_NOT_ON,(byte) HCACK_RETURN_CODE_YELLOW.Signal_Is_Not_Unload_Request_For_Source_Port},
            {ALARMS.EQ_UNLOAD_REQUEST_ON_BUT_NO_CARGO,(byte) HCACK_RETURN_CODE_YELLOW.Cannot_Detect_Carrier_Exist_For_Source_Port},
            {ALARMS.Destine_Eq_Status_Down,(byte) HCACK_RETURN_CODE_YELLOW.Equipment_Status_Is_Down},
            {ALARMS.Source_Eq_Status_Down,(byte) HCACK_RETURN_CODE_YELLOW.Equipment_Status_Is_Down},
            {ALARMS.Destine_Eq_Already_Has_Task_To_Excute,(byte) HCACK_RETURN_CODE_YELLOW.Source_Port_Assign_AGV_But_Already_Exist_Same_Command},
        };

        public abstract MapResult GetHCACKReturnCode(ALARMS alarmCode);
    }
}
