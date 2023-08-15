using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.TASK;
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
    public class AGVStatusDBHelper : TaskDatabaseHelper
    {
        public new List<clsAGVStateDto> GetALL(bool enabled_agv_only = true)
        {
            using (var dbhelper = new DbContextHelper(connection_str))
            {
                if (enabled_agv_only)
                    return dbhelper._context.AgvStates.Where(agv => agv.Enabled).ToList();
                else
                    return dbhelper._context.AgvStates.ToList();
            }
        }

        public clsAGVStateDto GetAGVStateByName(string agv_name)
        {

            using (var dbhelper = new DbContextHelper(connection_str))
            {
                return dbhelper._context.AgvStates.Where(agv => agv.AGV_Name == agv_name).First();
            }
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
                using (var dbhelper = new DbContextHelper(connection_str))
                {
                    Console.WriteLine($"{JsonConvert.SerializeObject(AGVStateDto, Formatting.Indented)}");
                    dbhelper._context.Set<clsAGVStateDto>().Add(AGVStateDto);
                    int ret = await dbhelper._context.SaveChangesAsync();
                    return ret;
                }
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

        public async Task<(bool confirm, string errorMesg)> UpdateBatteryLevel(string agv_name, double batteryLevel)
        {
            string errorMesg = string.Empty;
            try
            {
                using (var dbhelper = new DbContextHelper(connection_str))
                {
                    clsAGVStateDto? agvState = dbhelper._context.Set<clsAGVStateDto>().FirstOrDefault(dto => dto.AGV_Name == agv_name);
                    if (agvState != null)
                    {
                        agvState.BatteryLevel = batteryLevel;
                        int ret = await dbhelper._context.SaveChangesAsync();

                        return (true, "");
                    }
                    else
                        return (false, "Can't Get AGV Data");
                }
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
                using (var dbhelper = new DbContextHelper(connection_str))
                {
                    clsAGVStateDto? agvState = dbhelper._context.Set<clsAGVStateDto>().FirstOrDefault(dto => dto.AGV_Name == AGVStateDto.AGV_Name);
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
                        agvState.BatteryLevel = AGVStateDto.BatteryLevel;
                        agvState.TaskName = AGVStateDto.TaskName;
                        agvState.TaskRunStatus = AGVStateDto.TaskRunStatus;
                        agvState.TaskRunAction = AGVStateDto.TaskRunAction;
                        agvState.Theta = AGVStateDto.Theta;
                        agvState.Connected = AGVStateDto.Connected;
                    }
                    else
                    {
                        AGVStateDto.Enabled = true;
                        Add(AGVStateDto);
                    }
                    int ret = await dbhelper._context.SaveChangesAsync();
                }
                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public bool IsExist(string AGVName)
        {
            using (var dbhelper = new DbContextHelper(connection_str))
            {
                clsAGVStateDto? agvState = dbhelper._context.Set<clsAGVStateDto>().FirstOrDefault(dto => dto.AGV_Name == AGVName);
                return agvState != null;
            }
        }

        public void UpdateConnected(string name, bool value)
        {
            using (var dbhelper = new DbContextHelper(connection_str))
            {
                clsAGVStateDto? agvState = dbhelper._context.Set<clsAGVStateDto>().FirstOrDefault(dto => dto.AGV_Name == name);
                if (agvState != null)
                {

                    agvState.Connected = value;
                    dbhelper._context.SaveChangesAsync();
                }
            }
        }

        internal void ChangeAllOffline()
        {
            foreach (var agv_status in dbContext.AgvStates)
            {
                agv_status.OnlineStatus = clsEnums.ONLINE_STATE.OFFLINE;
                agv_status.Connected = false;
            }
            dbContext.SaveChangesAsync();
        }
    }
}
