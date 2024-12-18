using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Availability;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Equipment;
using AGVSystemCommonNet6.Equipment.AGV;
using AGVSystemCommonNet6.Material;
using AGVSystemCommonNet6.Microservices.MCSCIM;
using AGVSystemCommonNet6.StopRegion;
using AGVSystemCommonNet6.Sys;
using AGVSystemCommonNet6.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE
{
    public class AGVSDatabase : DBHelperAbstract
    {
        public AGVSDbContext tables => base.dbContext;
        public AGVSDatabase() : base()
        {
        }

        public static async Task Initialize()
        {
            try
            {
                using (AGVSDatabase database = new AGVSDatabase())
                {
                    database.dbContext.Database.EnsureCreated();
                    await database.SaveChanges();
                    try
                    {
                        await DatabaseColumnCheck(database);
                        await TablePrimaryKeyCheck(database);
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        await database.SaveChanges();
                    }
                }

                using (AGVSDatabase database = new AGVSDatabase())
                {
                    _DefaultUsersCreate(database.tables.Users);
                    _UnCheckedAlarmsSetAsCheckes(database.tables.SystemAlarms);
                    _ = database.SaveChanges().Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task TablePrimaryKeyCheck(AGVSDatabase database)
        {
            try
            {
                var checkPKStatus = @"
                                    SELECT 
                                        t.name AS TableName,
                                        CASE WHEN pk.name IS NULL THEN 'No PK' ELSE 'Has PK' END AS PKStatus,
                                        pk.name AS PKName
                                    FROM sys.tables t
                                    LEFT JOIN sys.key_constraints pk 
                                        ON t.object_id = pk.parent_object_id 
                                        AND pk.type = 'PK'
                                    ORDER BY t.name;
                                ";

                var tablesNeedingPK = new List<string>();

                using (var command = database.dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = checkPKStatus;

                    database.dbContext.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            var tableName = result.GetString(0);
                            var pkStatus = result.GetString(1);
                            var pkName = result.IsDBNull(2) ? "N/A" : result.GetString(2);

                            Debug.WriteLine($"Table: {tableName}, Status: {pkStatus}, PK Name: {pkName}");

                            if (pkStatus == "No PK")
                            {
                                tablesNeedingPK.Add(tableName);
                            }
                        }
                    }
                }

                // 修復缺少主鍵的表
                if (tablesNeedingPK.Any())
                {
                    foreach (var tableName in tablesNeedingPK)
                    {
                        string pkColumnName = GetPrimaryKeyColumnName(tableName); // 根據表名確定主鍵列

                        try
                        {
                            if (!string.IsNullOrEmpty(pkColumnName))
                            {
                                var alterTableSql = $@"
                                                IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = '{tableName}')
                                                BEGIN
                                                    ALTER TABLE {tableName}
                                                    ADD CONSTRAINT PK_{tableName} PRIMARY KEY ({pkColumnName});
                                                END
                                            ";
                                database.dbContext.Database.ExecuteSqlRaw(alterTableSql);
                                Debug.WriteLine($"Added primary key constraint to table {tableName} on column {pkColumnName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n\n");
                            Console.WriteLine($"錯誤訊息:{ex.Message}");
                            Console.WriteLine("\n\n");
                            Console.WriteLine($"Oooooooops! 操作資料表{tableName} 檢查確認主鍵發生錯誤，需使用SSMS或其他資料庫工具將 '{pkColumnName}'欄位設定為[不允許Null]。 \n也可以在資料庫工具中直接將 '{pkColumnName}' 欄位設為 Primary Key.");
                            Console.WriteLine("\n\n");
                            Console.WriteLine("按下任意按鍵繼續");
                            Console.ReadLine();
                            Environment.Exit(1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(":" + ex.Message);
            }
        }

        // 輔助方法：根據表名確定主鍵列
        private static string GetPrimaryKeyColumnName(string tableName)
        {
            // 這裡定義每個表的主鍵列名
            switch (tableName)
            {
                case "SysStatus":
                    return "FieldName";
                case "Users":
                    return "UserName";
                case "Tasks":
                    return "TaskName";
                case "AgvStates":
                    return "AGV_Name";
                case "SystemAlarms":
                    return "Time";
                case "Availabilitys":
                    return "KeyStr";
                case "RealTimeAvailabilitys":
                    return "StartTime";
                case "TaskTrajecotroyStores":
                    return "TaskName";
                case "InstrumentMeasureResult":
                    return "StartTime";
                case "StopRegionData":
                    return "StartTime";
                case "PointPassTime":
                    return "DataKey";
                case "VehicleMaintain":
                    return "VehicleMaintainId";
                case "MaterialInfo":
                    return "RecordTime";
                case "StationStatus":
                    return "StationName";
                case "EqpUnloadStates":
                    return "StartWaitUnloadTime";
                case "EQStatus_AGV":
                    return "Name";
                case "EQStatus_MainEQ":
                    return "Name";
                case "EQStatus_Rack":
                    return "Name";
                case "DeepChargeRecords":
                    return "OrderRecievedTime";
                case "SecsLog":
                    return "LogTime";
                default:
                    Debug.WriteLine($"Warning: No primary key mapping defined for table {tableName}");
                    return null;
            }
        }
        private static async Task<bool> DatabaseColumnCheck(AGVSDatabase database)
        {
            SQLNativ.DatabaseSchemaUpdater schemaUpdater = new SQLNativ.DatabaseSchemaUpdater(AGVSConfigulator.SysConfigs.DBConnection);
            await schemaUpdater.EnsureFieldExists<AGVSSystemStatus>(nameof(database.tables.SysStatus));
            await schemaUpdater.EnsureFieldExists<UserEntity>(nameof(database.tables.Users));
            await schemaUpdater.EnsureFieldExists<clsTaskDto>(nameof(database.tables.Tasks));
            await schemaUpdater.EnsureFieldExists<clsAGVStateDto>(nameof(database.tables.AgvStates));
            await schemaUpdater.EnsureFieldExists<Alarm.clsAlarmDto>(nameof(database.tables.SystemAlarms));
            await schemaUpdater.EnsureFieldExists<AvailabilityDto>(nameof(database.tables.Availabilitys));
            await schemaUpdater.EnsureFieldExists<RTAvailabilityDto>(nameof(database.tables.RealTimeAvailabilitys));
            await schemaUpdater.EnsureFieldExists<clsTaskTrajecotroyStore>(nameof(database.tables.TaskTrajecotroyStores));
            await schemaUpdater.EnsureFieldExists<clsPointPassInfo>(nameof(database.tables.PointPassTime));
            await schemaUpdater.EnsureFieldExists<clsMeasureResult>(nameof(database.tables.InstrumentMeasureResult));
            await schemaUpdater.EnsureFieldExists<clsStopRegionDto>(nameof(database.tables.StopRegionData));
            await schemaUpdater.EnsureFieldExists<clsMaterialInfo>(nameof(database.tables.MaterialInfo));
            await schemaUpdater.EnsureFieldExists<clsStationStatus>(nameof(database.tables.StationStatus));
            await schemaUpdater.EnsureFieldExists<EqUnloadState>(nameof(database.tables.EqpUnloadStates));
            await schemaUpdater.EnsureFieldExists<AGVStatus>(nameof(database.tables.EQStatus_AGV));
            await schemaUpdater.EnsureFieldExists<MainEQStatus>(nameof(database.tables.EQStatus_MainEQ));
            await schemaUpdater.EnsureFieldExists<RackStatus>(nameof(database.tables.EQStatus_Rack));
            await schemaUpdater.EnsureFieldExists<DeepChargeRecord>(nameof(database.tables.DeepChargeRecords));
            await schemaUpdater.EnsureFieldExists<SecsMessageLog>(nameof(database.tables.SecsLog));

            return true;
        }
        private static void DataBaseMirgration()
        {

            var ori_database_name = AGVSConfigulator.SysConfigs.DBConnection.Split(';');
            AGVSConfigulator.SysConfigs.DBConnection = AGVSConfigulator.SysConfigs.DBConnection.Replace(ori_database_name[1], $"Database=GPMAGVs_V{DateTime.Now.ToString("yyMMddHHmmssff")}");
            AGVSConfigulator.Save(AGVSConfigulator.SysConfigs);
        }

        private static void DatabaseVersionCheck(AGVSDatabase databse)
        {
            try
            {
                var nulls = databse.tables.Tasks.Where(t => t.TransferToDestineAGVName == null);
                databse.tables.Tasks.FirstOrDefault();
                databse.tables.Users.FirstOrDefault();
                databse.tables.AgvStates.FirstOrDefault();
                databse.tables.SystemAlarms.FirstOrDefault();
                databse.tables.Availabilitys.FirstOrDefault();
                databse.tables.RealTimeAvailabilitys.FirstOrDefault();
                databse.tables.TaskTrajecotroyStores.FirstOrDefault();
                databse.tables.InstrumentMeasureResult.FirstOrDefault();
                databse.tables.StopRegionData.FirstOrDefault();
                databse.tables.MaterialInfo.FirstOrDefault();
                databse.tables.StationStatus.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // 如果出現 Data is Null的例外, 可能是資料庫規劃有改版造成的, 直接把DB刪掉就好  
                throw ex;
            }
        }

        private static void _UnCheckedAlarmsSetAsCheckes(DbSet<clsAlarmDto> systemAlarms)
        {
            try
            {
                foreach (var alarm in systemAlarms.Where(al => !al.Checked))
                {
                    alarm.Checked = true;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static void _DefaultUsersCreate(Microsoft.EntityFrameworkCore.DbSet<User.UserEntity> users)
        {
            UserEntity? dev_user = users.FirstOrDefault(u => u.UserName == "dev");
            UserEntity? eng_user = users.FirstOrDefault(u => u.UserName == "eng");
            UserEntity? op_user = users.FirstOrDefault(u => u.UserName == "op");

            string dev_eng_permission_json = new ViewModels.WebFunctionViewPermissions().ToJson();
            if (dev_user == null)
                users.Add(new UserEntity { UserName = "dev", Password = "12345678", Role = ERole.Developer, WebFunctionPermissionsJson = dev_eng_permission_json });
            else if (dev_user.WebFunctionPermissionsJson == "")
            {
                dev_user.WebFunctionPermissionsJson = dev_eng_permission_json;
            }

            if (eng_user == null)
                users.Add(new UserEntity { UserName = "eng", Password = "12345678", Role = ERole.Engineer, WebFunctionPermissionsJson = dev_eng_permission_json });
            else if (eng_user.WebFunctionPermissionsJson == "")
            {
                eng_user.WebFunctionPermissionsJson = dev_eng_permission_json;
            }

            string opPermissionJson = new ViewModels.WebFunctionViewPermissions(ERole.Operator).ToJson();
            if (op_user == null)
                users.Add(new UserEntity { UserName = "op", Password = "op", Role = ERole.Operator, WebFunctionPermissionsJson = opPermissionJson });
            else if (op_user.WebFunctionPermissionsJson == "")
            {
                op_user.WebFunctionPermissionsJson = opPermissionJson;
            }
        }

    }
}
