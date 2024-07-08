using AGVSystemCommonNet6.Availability;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.StopRegion
{
    public class StopRegionHelper
    {
        public clsStopRegionDto stopRegionDto_IdleEvent = new clsStopRegionDto();
        private MAIN_STATUS PreviousMainStatus_IdleEvent =  MAIN_STATUS.Unknown;
        
        public StopRegionHelper(string AGVName)
        {
            stopRegionDto_IdleEvent = new clsStopRegionDto();
            stopRegionDto_IdleEvent.AGVName = AGVName;
        }

        public void UpdateStopRegionData(MAIN_STATUS CurrentStatus,string Position)
        {
            try
            {
                if (PreviousMainStatus_IdleEvent != CurrentStatus)
                {
                    if (CurrentStatus == MAIN_STATUS.IDLE) //從其他狀態變成Idle
                    {
                        stopRegionDto_IdleEvent.StartTime = DateTime.Now;
                        stopRegionDto_IdleEvent.RegionName = Position;
                        stopRegionDto_IdleEvent.Main_Status = MAIN_STATUS.IDLE;
                        stopRegionDto_IdleEvent.EndTime = DateTime.Now;
                        UpdateStopRegionDataToDataBase(stopRegionDto_IdleEvent);
                    }
                    else if (PreviousMainStatus_IdleEvent ==  MAIN_STATUS.IDLE) //從Idle變成其他狀態
                    {
                        stopRegionDto_IdleEvent.EndTime = DateTime.Now;
                        UpdateStopRegionDataToDataBase(stopRegionDto_IdleEvent);
                    }
                    PreviousMainStatus_IdleEvent = CurrentStatus;
                }
                else
                {
                    if (CurrentStatus == MAIN_STATUS.IDLE)
                    {
                        stopRegionDto_IdleEvent.EndTime = DateTime.Now;
                        UpdateStopRegionDataToDataBase(stopRegionDto_IdleEvent);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void UpdateStopRegionDataToDataBase(clsStopRegionDto clsStopRegion)
        {
            try
            {
                using (DbContextHelper aGVSDbContext = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    var currentData = aGVSDbContext._context.StopRegionData.FirstOrDefault(data => data.AGVName == clsStopRegion.AGVName && data.StartTime == clsStopRegion.StartTime);
                    if (currentData == null)
                    {
                        aGVSDbContext._context.StopRegionData.Add(clsStopRegion);
                    }
                    else
                    {
                        currentData.EndTime = clsStopRegion.EndTime;
                    }
                    var ret = aGVSDbContext._context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}
