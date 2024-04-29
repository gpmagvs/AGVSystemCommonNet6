using AGVSystemCommonNet6.AGVDispatch.Messages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE.Helpers
{
    public class AGVStatusDBHelper : DBHelperAbstract
    {
        private DbSet<clsAGVStateDto> AGVStatusSet => dbContext.AgvStates;
        public List<clsAGVStateDto> GetALL(bool enabled_agv_only = true)
        {
            if (enabled_agv_only)
                return AGVStatusSet.Where(agv => agv.Enabled).AsNoTracking().ToList();
            else
                return AGVStatusSet.ToList();
        }

        public clsAGVStateDto GetAGVStateByAGVName(string agv_name)
        {
            return AGVStatusSet.Where(agv => agv.AGV_Name == agv_name).AsNoTracking().FirstOrDefault();
        }

        /// <summary>
        /// 新增一筆AGV狀態資料
        /// </summary>
        /// <param name="AGVStateDto"></param>
        /// <returns></returns>
        public async Task<int> Add(clsAGVStateDto AGVStateDto)
        {
            try
            {
                AGVStatusSet.Add(AGVStateDto);
                return await SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return 0;
            }
            catch (Exception ex)
            {
                string exception_type = ex.GetType().ToString();
                throw ex;
            }
        }

        public async Task<(bool confirm, string errorMesg)> UpdateBatteryLevel(string agv_name, double[] batteryLevel)
        {
            string errorMesg = string.Empty;
            try
            {
                clsAGVStateDto? agvState = AGVStatusSet.FirstOrDefault(dto => dto.AGV_Name == agv_name);
                if (agvState != null)
                {
                    agvState.BatteryLevel_1 = batteryLevel[0];
                    agvState.BatteryLevel_2 = batteryLevel.Length >= 2 ? batteryLevel[1] : 0;
                    int ret = await SaveChanges();

                    return (true, "");
                }
                else
                    return (false, "Can't Get AGV Data");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool confirm, string errorMesg)> Update(clsAGVStateDto AGVStateDto)
        {
            try
            {
                clsAGVStateDto? agvState = AGVStatusSet.FirstOrDefault(dto => dto.AGV_Name == AGVStateDto.AGV_Name);
                if (agvState != null)
                {
                    if (JsonConvert.SerializeObject(agvState) == JsonConvert.SerializeObject(AGVStateDto))
                        return (true, "");

                    agvState.AGV_Description = AGVStateDto.AGV_Description;
                    agvState.Model = AGVStateDto.Model;
                    agvState.MainStatus = AGVStateDto.MainStatus;
                    agvState.OnlineStatus = AGVStateDto.OnlineStatus;
                    agvState.CurrentLocation = AGVStateDto.CurrentLocation;
                    agvState.CurrentCarrierID = AGVStateDto.CurrentCarrierID;
                    agvState.BatteryLevel_1 = AGVStateDto.BatteryLevel_1;
                    agvState.BatteryLevel_2 = AGVStateDto.BatteryLevel_2;
                    agvState.TaskName = AGVStateDto.TaskName;
                    agvState.TaskRunStatus = AGVStateDto.TaskRunStatus;
                    agvState.TaskRunAction = AGVStateDto.TaskRunAction;
                    agvState.Theta = AGVStateDto.Theta;
                    agvState.Connected = AGVStateDto.Connected;
                }
                else
                {
                    AGVStateDto.Enabled = true;
                    await Add(AGVStateDto);
                }
                int ret = await SaveChanges();

                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

    }
}
