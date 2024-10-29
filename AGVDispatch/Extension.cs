using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public static class Extension
    {
        public static clsRunningStatus ToWebAPIRunningStatusObject(this RunningStatus tcp_running_status_model)
        {
            return new clsRunningStatus()
            {
                Time_Stamp = tcp_running_status_model.Time_Stamp,
                AGV_Reset_Flag = tcp_running_status_model.AGV_Reset_Flag,
                AGV_Status = tcp_running_status_model.AGV_Status,
                Alarm_Code = tcp_running_status_model.Alarm_Code.ToWebAPIAlarmCodes(),
                CargoType = tcp_running_status_model.CargoType,
                Cargo_Status = tcp_running_status_model.Cargo_Status,
                Coordination = tcp_running_status_model.Coordination,
                CPU_Usage_Percent = tcp_running_status_model.CPU_Usage_Percent,
                CSTID = tcp_running_status_model.CSTID,
                Electric_Volume = tcp_running_status_model.Electric_Volume,
                Escape_Flag = tcp_running_status_model.Escape_Flag,
                Fork_Height = tcp_running_status_model.Fork_Height,
                Last_Visited_Node = tcp_running_status_model.Last_Visited_Node,
                Odometry = tcp_running_status_model.Odometry,
                Sensor_Status = tcp_running_status_model.Sensor_Status,
                Signal_Strength = tcp_running_status_model.Signal_Strength,
                RAM_Usage_Percent = tcp_running_status_model.RAM_Usage_Percent,
                IsCharging = tcp_running_status_model.IsCharging
            };
        }

        public static Model.clsAlarmCode[] ToWebAPIAlarmCodes(this Messages.clsAlarmCode[] tcp_alarm_codes)
        {
            return tcp_alarm_codes.Select(al => new Model.clsAlarmCode
            {
                Alarm_Category = al.Alarm_Category,
                Alarm_Description = al.Alarm_Description,
                Alarm_Description_EN = al.Alarm_Description_EN,
                Alarm_ID = al.Alarm_ID,
                Alarm_Level = al.Alarm_Level,
            }).ToArray();
        }

        public static int GetToSlotInt(this clsTaskDto taskDto)
        {
            int.TryParse(taskDto.To_Slot, out int slotInt);
            return slotInt;
        }


        public static int GetFromSlotInt(this clsTaskDto taskDto)
        {
            int.TryParse(taskDto.From_Slot, out int slotInt);
            return slotInt;
        }
    }
}
