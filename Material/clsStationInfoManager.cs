using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Vehicle_Control.VCSDatabase;
using EquipmentManagment.Manager;
using EquipmentManagment.WIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Material
{
    public class clsStationInfoManager
    {
        private static AGVSDatabase database;

        private static bool Initialized = false;
        public clsStationInfoManager() { }

        public static void Initialize()
        {
            database = new AGVSDatabase();

            //var FailPorts = CompareStationInforWithExistSignal();
            //if (FailPorts.Count > 0)
            //{
            //    string strFailPorts = "";
            //    foreach (var port in FailPorts)
            //        strFailPorts += $"{port},";

            //    AlarmManagerCenter.AddAlarmAsync(new clsAlarmDto()
            //    {
            //        AlarmCode = (int)ALARMS.PutCSTButCSTSensorNotMatch,
            //        Level = ALARM_LEVEL.ALARM
            //    });
            //}

            Initialized = true;
        }

        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 更新WIP/撈爪的料況
        /// </summary>
        /// <param name="stationStatus"></param>
        /// <returns></returns>
        public static async Task UpdateStationInfo(clsStationStatus stationStatus)
        {
            try
            {
                if (StaEQPManagager.RacksList.FirstOrDefault(rack => rack.PortsStatus.Any(port => port.Properties.ID == stationStatus.StationName)) == null)
                    return;

                await semaphoreSlim.WaitAsync();
                if (!Initialized)
                    Initialize();

                if (CheckStationInfoExist(stationStatus.StationName) == false)
                    database.tables.StationStatus.Add(stationStatus);
                else
                    database.tables.StationStatus.Update(stationStatus);

                await database.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { semaphoreSlim.Release(); }
        }

        public static bool CheckStationInfoExist(string stationName)
        {
            List<clsStationStatus> stationInfos = new List<clsStationStatus>();

            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _stationInfos = dbhelper._context.Set<clsStationStatus>().OrderByDescending(stationinfo => stationinfo.StationName).Where(stationinfo => stationinfo.StationName == stationName);
                stationInfos = _stationInfos.ToList();
            };

            if (stationInfos.Count <= 0)
                return false;
            else
                return true;
        }

        public static List<string> CompareStationInforWithExistSignal()
        {
            List<string> FailPort = new List<string>();
            List<clsStationStatus> stationStatus = database.tables.StationStatus.ToList();

            foreach (var rack in StaEQPManagager.RacksList)
            {
                foreach (var WIPPort in rack.PortsStatus)
                {
                    var StationInfoInDB = stationStatus.FirstOrDefault(stationInfo => stationInfo.StationName == $"{rack.EQName}_{WIPPort.Properties.ID}");

                    if (StationInfoInDB == null)
                        continue;

                    // 在席On、DB有ID  -> 把DB的ID指派給該Port
                    if (WIPPort.ExistSensorStates.Any(portStatus => portStatus.Value != clsPortOfRack.SENSOR_STATUS.OFF) && string.IsNullOrEmpty(StationInfoInDB.MaterialID) == false)
                        WIPPort.CarrierID = StationInfoInDB.MaterialID;
                    // 在席Off、DB有ID -> 把DB的ID給清除
                    else if (WIPPort.ExistSensorStates.Any(portStatus => portStatus.Value != clsPortOfRack.SENSOR_STATUS.OFF) == false)
                    {
                        StationInfoInDB.MaterialID = "";
                        database.tables.StationStatus.Update(StationInfoInDB);
                    }
                    // 在席On、DB沒有ID -> 記下來統一Alarm
                    else if (WIPPort.ExistSensorStates.Any(portStatus => portStatus.Value != clsPortOfRack.SENSOR_STATUS.OFF) && string.IsNullOrEmpty(StationInfoInDB.MaterialID) == true)
                        FailPort.Add(StationInfoInDB.StationName);
                }
            }
            database.tables.SaveChangesAsync();
            return FailPort;
            //Parallel.ForEach(StaEQPManagager.RacksList, (rack) =>
            //{
            //    foreach (var port in rack.PortsStatus)
            //    {
            //        if (port.ExistSensorStates.Any(sensorStatus => sensorStatus.Value != clsPortOfRack.SENSOR_STATUS.OFF) == false)
            //        {

            //        }
            //    }
            //});
        }

        public static async void ScanWIP()
        {
            if (!Initialized)
                Initialize();

            List<clsStationStatus> stationStatus = database.tables.StationStatus.ToList();

            foreach (var rack in StaEQPManagager.RacksList)
            {
                foreach (var port in rack.PortsStatus)
                {
                    string _stationName = $"{rack.EQName}_{port.Properties.ID}";
                    if (stationStatus.Any(status => status.StationName == _stationName) == false)
                    {
                        try
                        {
                            database.tables.StationStatus.Add(new clsStationStatus()
                            {
                                StationName = _stationName,
                                StationCol = port.Properties.Column,
                                StationRow = port.Properties.Row,
                                StationTag = port.TagNumbers[0].ToString(),
                                IsEnable = port.Properties.PortEnable == 0,
                            });
                            await database.SaveChanges();

                        }
                        catch (Exception exp)
                        {
                        }
                    }
                }
            }
        }
    }
}
