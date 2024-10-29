using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Microservices.AGVS;
using AGVSystemCommonNet6.Vehicle_Control.VCSDatabase;
using EquipmentManagment.Manager;
using EquipmentManagment.WIP;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.EntityFrameworkCore;
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

        public static void Initialize()
        {
            database = new AGVSDatabase();
            Initialized = true;
        }

        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 更新WIP/撈爪的料況
        /// </summary>
        /// <param name="stationStatus"></param>
        /// <returns></returns>
        private static async Task UpdateStationInfo(clsStationStatus stationStatus)
        {
            try
            {
                await semaphoreSlim.WaitAsync();

                if (!Initialized)
                    Initialize();

                using (DbContextHelper aGVSDbContext = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    if (CheckStationInfoExist(stationStatus, out clsStationStatus StationStatusInputDB) == true)
                    {
                        aGVSDbContext._context.StationStatus.Update(StationStatusInputDB);
                        await AGVSSerivces.UpdateStationInfo(StationStatusInputDB);
                    }
                    else
                    {
                        aGVSDbContext._context.StationStatus.Add(stationStatus);
                        await AGVSSerivces.UpdateStationInfo(stationStatus);
                    }

                    var ret = aGVSDbContext._context.SaveChanges();
                }

            }
            catch (DbUpdateConcurrencyException dbex)
            {
                foreach (var entry in dbex.Entries)
                {
                    if (entry.Entity is clsStationStatus)
                    {
                        // 取得資料庫中的目前數據
                        var databaseValues = entry.GetDatabaseValues();

                        if (databaseValues == null)
                        {
                            // 數據已被刪除
                            Console.WriteLine("Data has been deleted by another user.");
                        }
                        else
                        {
                            // 數據已被修改，你可以選擇用資料庫中的值覆蓋當前值
                            entry.OriginalValues.SetValues(databaseValues);

                            // 或者提示用戶選擇保留哪些更改
                            Console.WriteLine("Data has been modified by another user. Choose to overwrite or keep the current changes.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { semaphoreSlim.Release(); }
        }

        public static async Task UpdateStationInfo(clsTaskDto taskDto, MaterialType materialType, string CarrierID, bool IsNGport = false)
        {
            clsStationStatus _stationStatus = new clsStationStatus()
            {
                UpdateTime = DateTime.Now,
                StationRow = Convert.ToInt32(taskDto.To_Slot),
                StationTag = taskDto.To_Station_Tag.ToString(),
                MaterialID = CarrierID,
                Type = materialType,
                IsNGPort = IsNGport,
                IsEnable = true
            };

            await UpdateStationInfo(_stationStatus);
        }

        public static void TransferTask2StationInfo(clsTaskDto taskDto, MaterialType materialType)
        {
            // 找出符合Tag Colunm Row的Port
            int[] TagToFind = new int[] { taskDto.To_Station_Tag };
            var TargetRack = StaEQPManagager.RacksList.FirstOrDefault(rack => rack.RackOption.ColumnTagMap.ContainsValue(TagToFind));

            if (TargetRack == null)
                return;

            int columnNum = -1;
            foreach (var tags in TargetRack.RackOption.ColumnTagMap)
            {
                if (tags.Value.SequenceEqual(TagToFind))
                {
                    columnNum = tags.Key;
                    break;
                }
            }

            if (columnNum == -1)
                return;

            string WIPPort = $"{columnNum}-{taskDto.To_Slot}";
            string TargetStation = $"{TargetRack.EQName}_{WIPPort}";

            UpdateStationInfo(new clsStationStatus()
            {
                UpdateTime = DateTime.Now,
                StationName = TargetStation,
                StationTag = taskDto.To_Station_Tag.ToString(),
                StationCol = columnNum,
                StationRow = Convert.ToInt32(taskDto.To_Slot),
                MaterialID = taskDto.Carrier_ID,
                Type = materialType
            });
        }

        public static bool CheckStationInfoExist(clsStationStatus stationStatus, out clsStationStatus CombineStationStatus)
        {
            List<clsStationStatus> stationInfos = new List<clsStationStatus>();

            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _stationInfos = dbhelper._context.Set<clsStationStatus>().OrderByDescending(stationinfo => stationinfo.StationTag).Where(
                    stationinfo => stationinfo.StationTag == stationStatus.StationTag && stationinfo.StationRow == stationStatus.StationRow);
                stationInfos = _stationInfos.ToList();
            };

            if (stationInfos.Count <= 0)
            {
                CombineStationStatus = null;
                return false;
            }
            else
            {
                CombineStationStatus = stationStatus;
                CombineStationStatus.StationName = stationInfos[0].StationName;
                CombineStationStatus.StationCol = stationInfos[0].StationCol;

                return true;
            }
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
                    if (WIPPort.MaterialExistSensorStates.Any(portStatus => portStatus.Value != clsPortOfRack.SENSOR_STATUS.OFF) && string.IsNullOrEmpty(StationInfoInDB.MaterialID) == false)
                        WIPPort.CarrierID = StationInfoInDB.MaterialID;
                    // 在席Off、DB有ID -> 把DB的ID給清除
                    else if (WIPPort.MaterialExistSensorStates.Any(portStatus => portStatus.Value != clsPortOfRack.SENSOR_STATUS.OFF) == false)
                    {
                        StationInfoInDB.MaterialID = "";
                        database.tables.StationStatus.Update(StationInfoInDB);
                    }
                    // 在席On、DB沒有ID -> 記下來統一Alarm
                    else if (WIPPort.MaterialExistSensorStates.Any(portStatus => portStatus.Value != clsPortOfRack.SENSOR_STATUS.OFF) && string.IsNullOrEmpty(StationInfoInDB.MaterialID) == true)
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

        public static async Task<(bool, string)> AssignStationInfo(clsStationStatus stationStatus)
        {
            try
            {
                foreach (var rack in StaEQPManagager.RacksList)
                {
                    if (rack.RackOption.ColumnTagMap.ContainsValue(new int[] { Convert.ToInt32(stationStatus.StationTag) }) == true)
                    {
                        foreach (var port in rack.PortsStatus)
                        {
                            if (port.Properties.Column == stationStatus.StationCol && port.Properties.Row == stationStatus.StationRow)
                            {
                                port.CarrierID = stationStatus.MaterialID;
                                port.Properties.ProductionQualityStore = stationStatus.IsNGPort ? clsPortOfRack.PRUDUCTION_QUALITY.NG : clsPortOfRack.PRUDUCTION_QUALITY.OK;
                                port.CarrierExist = true;
                                port.InstallTime = stationStatus.UpdateTime;
                                return (true, $"");
                            }
                            else
                                continue;
                        }
                    }
                    else
                        continue;
                }

                foreach (var EQ in StaEQPManagager.MainEQList)
                {
                    if (EQ.EndPointOptions.TagID == Convert.ToInt32(stationStatus.StationTag))
                    {
                        EQ.PortStatus.CarrierID = stationStatus.MaterialID;
                        EQ.PortStatus.CarrierExist = true;
                        EQ.PortStatus.InstallTime = stationStatus.UpdateTime;
                        return (true, $"");
                    }
                    else
                        continue;
                }

                return (true, $"No EQ/Rack is updated !!");
            }
            catch (Exception exp)
            {
                return (false, exp.Message);
            }

        }

        public static async Task ScanWIP_EQ()
        {
            await ScanWIP();
            await ScanEQ();
        }

        private static async Task ScanWIP()
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

        private static async Task ScanEQ()
        {
            if (!Initialized)
                Initialize();

            List<clsStationStatus> stationStatus = database.tables.StationStatus.ToList();

            foreach (var EQ in StaEQPManagager.MainEQList)
            {
                string _stationName = $"{EQ.EQName}";
                if (stationStatus.Any(status => status.StationName == _stationName) == false)
                {
                    try
                    {
                        database.tables.StationStatus.Add(new clsStationStatus()
                        {
                            StationName = _stationName,
                            StationCol = 0,
                            StationRow = 0,
                            StationTag = EQ.EndPointOptions.TagID.ToString(),
                            IsEnable = EQ.EndPointOptions.Enable,
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
