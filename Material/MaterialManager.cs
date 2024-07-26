using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Microservices.AGVS;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using EquipmentManagment.MainEquipment;
using EquipmentManagment.Manager;
using EquipmentManagment.WIP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using static AGVSystemCommonNet6.DATABASE.DatabaseCaches;

namespace AGVSystemCommonNet6.Material
{
    public class MaterialManager
    {
        private static AGVSDatabase database;

        private static bool Initialized = false;
        public MaterialManager() { }

        public static void Initialize()
        {
            database = new AGVSDatabase();
            Initialized = true;
        }

        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        /// <summary>
        /// 新增一筆Material Log進DB
        /// </summary>
        /// <param name="materialInfo"></param>
        /// <returns></returns>
        private static async Task AddMaterialInfo(clsMaterialInfo materialInfo)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                if (!Initialized)
                    Initialize();
                database.tables.MaterialInfo.Add(materialInfo);
                await database.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { semaphoreSlim.Release(); }
        }

        /// <summary>
        /// 創建一筆新的Material Log
        /// </summary>
        /// <param name="MaterialID"></param>
        /// <param name="ActualID"></param>
        /// <param name="SourceStation"></param>
        /// <param name="TargetStation"></param>
        /// <param name="TaskSource"></param>
        /// <param name="TaskTarget"></param>
        /// <param name="installStatus"></param>
        /// <param name="IDStatus"></param>
        /// <param name="materialType"></param>
        /// <param name="materialCondition"></param>
        /// <returns></returns>
        public static async Task<clsMaterialInfo> CreateMaterialInfo(string MaterialID, string ActualID = "", string SourceStation = "", string TargetStation = "", string TaskSource = "", string TaskTarget = "", MaterialInstallStatus installStatus = MaterialInstallStatus.None, MaterialIDStatus IDStatus = MaterialIDStatus.None, MaterialType materialType = MaterialType.None, MaterialCondition materialCondition = MaterialCondition.Add)
        {
            try
            {
                clsMaterialInfo materialDto = new clsMaterialInfo()
                {
                    MaterialID = MaterialID,
                    ActualID = ActualID,
                    SourceStation = SourceStation,
                    TargetStation = TargetStation,
                    TaskSourceStation = TaskSource,
                    TaskTargetStation = TaskTarget,
                    InstallStatus = installStatus,
                    IDStatus = IDStatus,
                    Type = materialType,
                    Condition = materialCondition
                };
                materialDto.RecordTime = DateTime.Now;
                await AddMaterialInfo(materialDto);
                LOG.INFO($"Material Status Update : {materialDto.ToJson(Formatting.None)}");
                return materialDto;
            }
            catch (Exception ex)
            {
                LOG.ERROR("AddMaterialAsync", ex);
                return null;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 增建一筆Material Log，用於手動建帳
        /// </summary>
        /// <param name="MaterialID"></param>
        /// <param name="TargetStation"></param>
        /// <param name="installStatus"></param>
        /// <param name="IDStatus"></param>
        /// <param name="materialType"></param>
        /// <param name="materialCondition"></param>
        /// <returns></returns>
        public static async Task<clsMaterialInfo> AddMaterialInfo(string MaterialID, string TargetStation, MaterialInstallStatus installStatus, MaterialIDStatus IDStatus, MaterialType materialType, MaterialCondition materialCondition)
        {
            try
            {
                clsMaterialInfo materialDto = new clsMaterialInfo()
                {
                    MaterialID = MaterialID,
                    TargetStation = TargetStation,
                    InstallStatus = installStatus,
                    IDStatus = IDStatus,
                    Type = materialType,
                    Condition = materialCondition
                };
                materialDto.RecordTime = DateTime.Now;
                await AddMaterialInfo(materialDto);
                LOG.INFO($"Material Status Update : {materialDto.ToJson(Formatting.None)}");
                return materialDto;
            }
            catch (Exception ex)
            {
                LOG.ERROR("AddMaterialAsync", ex);
                return null;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 刪除帳籍
        /// </summary>
        /// <param name="MaterialID"></param>
        /// <param name="SourceStation"></param>
        /// <param name="installStatus"></param>
        /// <param name="IDStatus"></param>
        /// <param name="materialType"></param>
        /// <returns></returns>
        public static async Task<clsMaterialInfo> DeleteMaterialInfo(string MaterialID, string SourceStation, MaterialInstallStatus installStatus, MaterialIDStatus IDStatus, MaterialType materialType)
        {
            try
            {
                clsMaterialInfo materialDto = new clsMaterialInfo()
                {
                    MaterialID = MaterialID,
                    SourceStation = SourceStation,
                    InstallStatus = installStatus,
                    IDStatus = IDStatus,
                    Type = materialType,
                    Condition = MaterialCondition.Delete
                };
                materialDto.RecordTime = DateTime.Now;
                await AddMaterialInfo(materialDto);
                LOG.INFO($"Material Status Update : {materialDto.ToJson(Formatting.None)}");
                return materialDto;
            }
            catch (Exception ex)
            {
                LOG.ERROR("AddMaterialAsync", ex);
                return null;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 修改物料ID
        /// </summary>
        /// <param name="OriginMaterialID"></param>
        /// <param name="NewMaterialID"></param>
        /// <param name="SourceStation"></param>
        /// <param name="installStatus"></param>
        /// <param name="materialIDStatus"></param>
        /// <param name="materialType"></param>
        /// <returns></returns>
        public static async Task<clsMaterialInfo> EditMaterialInfo(string OriginMaterialID, string NewMaterialID, string SourceStation, MaterialInstallStatus installStatus, MaterialIDStatus materialIDStatus, MaterialType materialType)
        {
            try
            {
                clsMaterialInfo materialDto = new clsMaterialInfo()
                {
                    MaterialID = OriginMaterialID,
                    ActualID = NewMaterialID,
                    SourceStation = SourceStation,
                    InstallStatus = installStatus,
                    IDStatus = materialIDStatus,
                    Type = materialType,
                    Condition = MaterialCondition.Edit
                };
                materialDto.RecordTime = DateTime.Now;
                await AddMaterialInfo(materialDto);
                LOG.INFO($"Material Status Update : {materialDto.ToJson(Formatting.None)}");
                return materialDto;
            }
            catch (Exception ex)
            {
                LOG.ERROR("AddMaterialAsync", ex);
                return null;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 查找時間範圍內的Material Log
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="materialCondition"></param>
        /// <returns></returns>
        public static List<clsMaterialInfo> MaterialInfoQuery(DateTime startTime, DateTime endTime, string materialCondition = "ALL")
        {
            List<clsMaterialInfo> materialInfos = new List<clsMaterialInfo>();

            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _materialInfos = dbhelper._context.Set<clsMaterialInfo>().OrderByDescending(materialinfo => materialinfo.RecordTime).Where(materialinfo => materialinfo.RecordTime >= startTime
                                    && materialinfo.RecordTime <= endTime
                                    && (materialinfo.Condition.ToString() == "ALL" ? true : (materialinfo.Condition.ToString().ToUpper() == materialCondition.ToUpper()))
                );
                materialInfos = _materialInfos.ToList();
            };

            return materialInfos;
        }

        public static clsTransferMaterial? CheckEqMaterialOverLevel()
        {
            if (AGVSConfigulator.SysConfigs.MaterialBufferLevelMonitor.MonitorSwitch == false)
                return null;
            if (AGVSConfigulator.SysConfigs.MaterialBufferLevelMonitor.LevelThreshold <= 0)
                return null;

            // 篩選出有設定監控水位的設備們，然後判斷是否大於水位閥值
            var NeedMonitorStorageEq = StaEQPManagager.MainEQList.FindAll(eq => eq.EndPointOptions.IsNeedStorageMonitor == true && !eq.Eqp_Status_Down);
            var EqHasMateiral = NeedMonitorStorageEq.FindAll(eq => eq.PortStatus.CarrierExist == true);

            if (EqHasMateiral.Count <= AGVSConfigulator.SysConfigs.MaterialBufferLevelMonitor.LevelThreshold)
                return null;

            int MaxPriorityOfEq = EqHasMateiral.Max(eq => eq.EndPointOptions.StorageMonitorPriority);
            var FirstMoveEq = EqHasMateiral.FirstOrDefault(eq => eq.EndPointOptions.StorageMonitorPriority == MaxPriorityOfEq);

            // 篩選出可以放的WIP儲位
            var FirstMovePort = TargetPortRequest();

            // 回傳起終點站資訊
            if (FirstMoveEq == null || FirstMovePort == null)
                return null;
            else
            {
                clsTransferMaterial transferMaterial = new clsTransferMaterial()
                {
                    MaterialID = FirstMoveEq.EndPointOptions.InstalledCargoID,
                    SourceStation = FirstMoveEq.EQName,
                    SourceTag = FirstMoveEq.EndPointOptions.TagID,
                    TargetStation = $"{FirstMovePort.GetParentRack().EQName}-{FirstMovePort.Properties.ID}",
                    TargetTag = FirstMovePort.TagNumbers[FirstMovePort.Properties.Column - 1],
                    TargetColumn = FirstMovePort.Properties.Column,
                    TargetRow = FirstMovePort.Properties.Row,
                };
                return transferMaterial;
            }
        }

        public static clsTransferMaterial? NgMaterialTransferTarget()
        {
            clsTransferMaterial NgMaterialTargetPort = null;
            var TargetPort = TargetPortRequest();

            if (TargetPort == null)
                return null;
            try
            {
                NgMaterialTargetPort = new clsTransferMaterial();
                string TargetStation = $"WIP:{TargetPort.GetParentRack().EQName};Port:{TargetPort.Properties.ID}";
                int TargetTag = TargetPort.GetParentRack().RackOption.ColumnTagMap[TargetPort.Properties.Column].FirstOrDefault();
                int TargetColumn = TargetPort.Properties.Column;
                int TargetRow = TargetPort.Properties.Row;
                NgMaterialTargetPort.TargetStation = TargetStation;
                NgMaterialTargetPort.TargetTag = TargetTag;
                NgMaterialTargetPort.TargetColumn = TargetColumn;
                NgMaterialTargetPort.TargetRow = TargetRow;
                NgMaterialTargetPort.message = "GET NG port OK";
            }
            catch (Exception ex)
            {
                NgMaterialTargetPort.message= ex.Message;
            }

            return NgMaterialTargetPort;
        }

        private static clsPortOfRack? TargetPortRequest()
        {
            var CanUseRack = StaEQPManagager.RacksList.FindAll(rack => rack.RackOption.Enable).OrderBy(rack => rack.RackOption.StorageMonitorPriority);
            if (CanUseRack.Count() <= 0)
                return null;

            var ports = CanUseRack.SelectMany(rk => rk.PortsStatus.Where(p => p.CargoExist == false && p.Properties.PortEnable == clsPortOfRack.Port_Enable.Enable).Select(p=>p));
            
            return ports.OrderByDescending(x => x.Properties.StoragePriority).FirstOrDefault();

            //var CanUseRack = StaEQPManagager.RacksList.FindAll(portOfRack => portOfRack.PortsStatus.Any(port => port.CargoExist == false && port.CarrierExist == false && port.Properties.PortEnable == EquipmentManagment.WIP.clsPortOfRack.Port_Enable.Enable));
            //if (CanUseRack.Count <= 0)
            //    return null;

            //int MaxPriorityOfRack = CanUseRack.Min(rack => rack.EndPointOptions.StorageMonitorPriority);
            //var FirstTargetRack = CanUseRack.FindAll(rack => rack.EndPointOptions.StorageMonitorPriority == MaxPriorityOfRack).First();

            //int MaxPriorityOfPort = FirstTargetRack.PortsStatus.ToList().Min(port => port.Properties.StoragePriority);
            //var FirstMovePort = FirstTargetRack.PortsStatus.ToList().FirstOrDefault(port => port.Properties.StoragePriority == MaxPriorityOfPort);

            //return FirstMovePort;
        }

        /// <summary>
        /// 將Material Log匯出成CSV檔
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="MaterialID"></param>
        /// <param name="materialCondition"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string SaveTocsv(DateTime startTime, DateTime endTime, string MaterialID = "", MaterialCondition materialCondition = MaterialCondition.Add, string fileName = null)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, "Material History");
            var _fileName = fileName.IsNullOrEmpty() ? DateTime.Now.ToString("yyyy-MM-dd-HH") + ".csv" : fileName;
            Directory.CreateDirectory(folder);
            using (var dbhelper = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                var _materialInfo = dbhelper._context.Set<clsMaterialInfo>().Where(materialInfo => materialInfo.RecordTime >= startTime && materialInfo.RecordTime <= endTime && materialInfo.Condition == materialCondition);
                List<string> list = new List<string> {
                    "紀錄時間," +
                    "Material ID," +
                    "Actual ID," +
                    "Remove From," +
                    "Install To," +
                    "Task Source Station," +
                    "Task Target Station," +
                    "Install Status," +
                    "Material ID Status," +
                    "Material Type," +
                    "Material Condition" };
                list.AddRange(_materialInfo.Select(materialInfo =>
                $"{materialInfo.RecordTime}," +
                $"{materialInfo.MaterialID}," +
                $"{materialInfo.ActualID}," +
                $"{materialInfo.SourceStation}," +
                $"{materialInfo.TargetStation}," +
                $"{materialInfo.TaskSourceStation}," +
                $"{materialInfo.TaskTargetStation}," +
                $"{materialInfo.InstallStatus}," +
                $"{materialInfo.IDStatus}," +
                $"{materialInfo.Type}," +
                $"{materialInfo.Condition}"));
                File.WriteAllLines(_fileName, list, Encoding.UTF8);
            };
            return _fileName;
        }
        public static async void HandlePortExistChanged(object sender, EventArgs e)
        {
            EquipmentManagment.MainEquipment.clsEQ eq = sender as EquipmentManagment.MainEquipment.clsEQ;
            string strEQname = eq.EQName;
            clsTransferMaterial info = MaterialManager.CheckEqMaterialOverLevel();

            if (info != null)
            {
                clsTaskDto task = new clsTaskDto();
                task.State = AGVDispatch.Messages.TASK_RUN_STATUS.WAIT;
                task.Action = AGVDispatch.Messages.ACTION_TYPE.Carry;
                task.From_Station = info.SourceTag.ToString();
                task.From_Station_AGV_Type = clsEnums.AGV_TYPE.FORK;
                task.From_Slot = info.SourceSlot.ToString();
                task.To_Station = info.TargetTag.ToString();
                task.To_Station_AGV_Type = clsEnums.AGV_TYPE.FORK;
                task.To_Slot = info.TargetSolt.ToString();

                (bool confirm, string token, string strUsername) login = await AGVSSerivces.Login();
                bool b = await AGVSSerivces.TRANSFER_TASK.call_AGVs_carry_api(login.token, login.strUsername, task);
            }
        }
    }
}
