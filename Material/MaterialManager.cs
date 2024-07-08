using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
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

        public static async Task<clsMaterialInfo> CreateMaterialInfo(string MaterialID, MaterialType materialType = MaterialType.None, string ActualID = "", string SourceStation = "", string TargetStation = "", string TaskSource = "", string TaskTarget = "", MaterialInstallStatus installStatus = MaterialInstallStatus.None, MaterialIDStatus IDStatus = MaterialIDStatus.None, MaterialCondition materialCondition = MaterialCondition.Add)
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
    }
}
