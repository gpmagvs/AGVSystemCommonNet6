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

namespace AGVSystemCommonNet6.DATABASE
{
    public class AGVStatusDBHelper : TaskDatabaseHelper
    {
        public new List<clsAGVStateDto> GetALL()
        {
            using (var dbhelper = new DbContextHelper(connection_str))
            {
                return dbhelper._context.Set<clsAGVStateDto>().ToList();
            }
        }

        /// <summary>
        /// 新增一筆AGV狀態資料
        /// </summary>
        /// <param name="AGVStateDto"></param>
        /// <returns></returns>
        public int Add(clsAGVStateDto AGVStateDto)
        {
            try
            {
                using (var dbhelper = new DbContextHelper(connection_str))
                {
                    Console.WriteLine($"{JsonConvert.SerializeObject(AGVStateDto, Formatting.Indented)}");
                    dbhelper._context.Set<clsAGVStateDto>().Add(AGVStateDto);
                    int ret = dbhelper._context.SaveChanges();
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

        public bool UpdateBatteryLevel(string agv_name, double batteryLevel, out string errorMesg)
        {
            errorMesg = string.Empty;
            try
            {
                using (var dbhelper = new DbContextHelper(connection_str))
                {
                    clsAGVStateDto? agvState = dbhelper._context.Set<clsAGVStateDto>().FirstOrDefault(dto => dto.AGV_Name == agv_name);
                    if (agvState != null)
                    {
                        agvState.BatteryLevel = batteryLevel;
                        int ret = dbhelper._context.SaveChanges();

                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                errorMesg = ex.Message;
                return false;
            }
        }

        public bool Update(clsAGVStateDto AGVStateDto, out string errorMesg)
        {
            errorMesg = string.Empty;
            try
            {
                using (var dbhelper = new DbContextHelper(connection_str))
                {
                    clsAGVStateDto? agvState = dbhelper._context.Set<clsAGVStateDto>().FirstOrDefault(dto => dto.AGV_Name == AGVStateDto.AGV_Name);
                    if (agvState != null)
                    {
                        if (JsonConvert.SerializeObject(agvState) == JsonConvert.SerializeObject(AGVStateDto))
                            return true;

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
                        Add(AGVStateDto);
                    }
                    int ret = dbhelper._context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMesg = ex.Message;
                return false;
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
                agvState.Connected = value;
                dbhelper._context.SaveChanges();
            }
        }

        internal void ChangeAllOffline()
        {
            using (var dbhepler = new DbContextHelper(connection_str))
            {
                foreach (var agv_status in dbhelper._context.AgvStates)
                {
                    agv_status.OnlineStatus = clsEnums.ONLINE_STATE.OFFLINE;
                    agv_status.Connected = false;
                }
                dbhelper._context.SaveChanges();
            }
        }
    }
}
