﻿using AGVSystemCommonNet6.Alarm.SECS_Alarm_Code.Enums;

namespace AGVSystemCommonNet6.Alarm.SECS_Alarm_Code
{
    public class AlarmCodeMapperBaseOnGPMSpec : AlarmCodeMapperBaseOnKGSSpec
    {

        public override MapResult GetHCACKReturnCode(ALARMS alarmCode)
        {

            if (MappingTable.ContainsKey(alarmCode))
            {
                var code = MappingTable[alarmCode];
                HCACK_RETURN_CODE_GPM codeEnum = Enum.GetValues(typeof(HCACK_RETURN_CODE_GPM)).Cast<HCACK_RETURN_CODE_GPM>().FirstOrDefault(x => (byte)x == code);
                return new MapResult((byte)codeEnum, codeEnum.ToString());
            }

            return new MapResult((byte)HCACK_RETURN_CODE_GPM.System_Error, HCACK_RETURN_CODE_GPM.System_Error.ToString());
        }
    }
}
