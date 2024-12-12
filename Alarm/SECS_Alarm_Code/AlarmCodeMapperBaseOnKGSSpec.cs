using AGVSystemCommonNet6.Alarm.SECS_Alarm_Code.Enums;

namespace AGVSystemCommonNet6.Alarm.SECS_Alarm_Code
{

    public class AlarmCodeMapperBaseOnKGSSpec : SECSHCACKAlarmCodeMapper
    {

        public override MapResult GetHCACKReturnCode(ALARMS alarmCode)
        {

            if (MappingTable.ContainsKey(alarmCode))
            {
                var code = MappingTable[alarmCode];
                HCACK_RETURN_CODE_YELLOW codeEnum = Enum.GetValues(typeof(HCACK_RETURN_CODE_YELLOW)).Cast<HCACK_RETURN_CODE_YELLOW>().FirstOrDefault(x => (byte)x == code);
                return new MapResult((byte)codeEnum, codeEnum.ToString());
            }

            return new MapResult((byte)HCACK_RETURN_CODE_YELLOW.Unknown_Conditions, HCACK_RETURN_CODE_YELLOW.Unknown_Conditions.ToString());
        }
    }
}
