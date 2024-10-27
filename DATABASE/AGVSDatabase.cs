using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Availability;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Equipment;
using AGVSystemCommonNet6.Material;
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
                    try
                    {
                        await DatabaseColumnCheck(database);
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
            await schemaUpdater.EnsureFieldExists<clsMeasureResult>(nameof(database.tables.InstrumentMeasureResult));
            await schemaUpdater.EnsureFieldExists<clsStopRegionDto>(nameof(database.tables.StopRegionData));
            await schemaUpdater.EnsureFieldExists<clsMaterialInfo>(nameof(database.tables.MaterialInfo));
            await schemaUpdater.EnsureFieldExists<clsStationStatus>(nameof(database.tables.StationStatus));
            await schemaUpdater.EnsureFieldExists<EqUnloadState>(nameof(database.tables.EqpUnloadStates));

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
