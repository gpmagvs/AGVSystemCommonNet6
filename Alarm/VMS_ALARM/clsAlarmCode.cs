using SQLite;
using System.Security.Claims;

namespace AGVSystemCommonNet6.Alarm.VMS_ALARM
{
    public class clsAlarmCode
    {
        public enum LEVEL
        {
            Warning, Alarm
        }

        [PrimaryKey]
        public DateTime Time { get; set; }
        public int Code { get; set; }
        public string Description { get; set; } = "";
        public string CN { get; set; } = "";
        public LEVEL ELevel { get; set; }

        public string Level => ELevel.ToString();
        public AlarmCodes EAlarmCode
        {
            get
            {
                try
                {
                    return Enum.GetValues(typeof(AlarmCodes)).Cast<AlarmCodes>().First(ac => (int)ac == Code);
                }
                catch (Exception ex)
                {
                    return AlarmCodes.Unknown;
                }
            }
        }

        public bool IsRecoverable { get; internal set; } = true;

        public clsAlarmCode Clone()
        {
            return new clsAlarmCode
            {
                CN = CN,
                Code = Code,
                Description = Description,
                ELevel = ELevel,
                IsRecoverable = IsRecoverable,
                Time = Time,

            };
        }
    }

}
