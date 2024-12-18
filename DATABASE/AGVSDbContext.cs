using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Availability;
using AGVSystemCommonNet6.Equipment;
using AGVSystemCommonNet6.Equipment.AGV;
using AGVSystemCommonNet6.Maintainance;
using AGVSystemCommonNet6.Material;
using AGVSystemCommonNet6.Microservices.MCSCIM;
using AGVSystemCommonNet6.StopRegion;
using AGVSystemCommonNet6.Sys;
using AGVSystemCommonNet6.User;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace AGVSystemCommonNet6.DATABASE
{
    public class AGVSDbContext : DbContext
    {

        private static bool _isDualDbTransactionInterceptorInjuected = false;
        public DbSet<AGVSSystemStatus> SysStatus { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<clsTaskDto> Tasks { get; set; }
        public DbSet<clsAGVStateDto> AgvStates { get; set; }
        public DbSet<Alarm.clsAlarmDto> SystemAlarms { get; set; }
        public DbSet<AvailabilityDto> Availabilitys { get; set; }
        public DbSet<RTAvailabilityDto> RealTimeAvailabilitys { get; set; }
        public DbSet<clsTaskTrajecotroyStore> TaskTrajecotroyStores { get; set; }
        /// <summary>
        /// 儀器量測結果
        /// </summary>
        public DbSet<clsMeasureResult> InstrumentMeasureResult { get; set; }
        /// <summary>
        /// AGV 變成 Idle的地方、起始時間、結束時間
        /// </summary>
        public DbSet<clsStopRegionDto> StopRegionData { get; set; }

        public DbSet<clsPointPassInfo> PointPassTime { get; set; }

        public DbSet<VehicleMaintain> VehicleMaintain { get; set; }

        public DbSet<clsMaterialInfo> MaterialInfo { get; set; }

        public DbSet<clsStationStatus> StationStatus { get; set; }

        public DbSet<EqUnloadState> EqpUnloadStates { get; set; }

        public DbSet<AGVStatus> EQStatus_AGV { get; set; }

        public DbSet<MainEQStatus> EQStatus_MainEQ { get; set; }
        public DbSet<RackStatus> EQStatus_Rack { get; set; }

        public DbSet<DeepChargeRecord> DeepChargeRecords { get; set; }

        public DbSet<SecsMessageLog> SecsLog { get; set; }

        private bool _isWarRoomUse = false;

        public AGVSDbContext(DbContextOptions<AGVSDbContext> options, bool isWarRoomUse = false)
            : base(options)
        {
            _isWarRoomUse = isWarRoomUse;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!_isWarRoomUse && !_isWarRoomUse)
            {
                _isDualDbTransactionInterceptorInjuected = true;
                optionsBuilder.AddInterceptors(new DualDbTransactionInterceptor());
            }
            else
            {

            }
            base.OnConfiguring(optionsBuilder);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public bool IsAgvStatesTableLocking()
        {
            return IsTableLocking("AgvStates");
        }

        public bool IsTaskTableLocking()
        {
            return IsTableLocking("Tasks");
        }

        public bool IsAlarmTableLocking()
        {
            return IsTableLocking("SystemAlarms");
        }

        private bool IsTableLocking(string tableName)
        {
            var result = new List<LockInfo>();
            string sql = @"
                            SELECT request_session_id AS spid,
                                resource_type AS rt,
                                resource_databASe_id AS rdb,
                                (CASE resource_type 
                                WHEN 'OBJECT' then object_name(resource_ASsociated_entity_id) 
                                ELSE 
                                (SELECT object_name(object_id) FROM sys.partitions 
                                    WHERE hobt_id = resource_ASsociated_entity_id) END) AS objname,
                                resource_description AS rd,
                                request_mode AS rm,
                                request_status AS rs
                            FROM sys.dm_tran_locks
                            WHERE request_mode = 'X' OR request_mode = 'IX'";

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;

                if (command.Connection.State != System.Data.ConnectionState.Open)
                    command.Connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new LockInfo
                        {
                            Spid = reader.GetInt32(0),
                            Rt = reader.GetString(1),
                            Rdb = reader.GetInt32(2),
                            Objname = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Rd = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Rm = reader.GetString(5),
                            Rs = reader.GetString(6)
                        });
                    }
                }
            }
            return result.Where(ret => ret.Objname == tableName).Any();
        }

        public bool isPointPassInfoTableLocking()
        {
            throw new NotImplementedException();
        }

        // 首先需要定義一個對應的模型類
        public class LockInfo
        {
            public int Spid { get; set; }
            public string Rt { get; set; }
            public int Rdb { get; set; }
            public string Objname { get; set; }
            public string Rd { get; set; }
            public string Rm { get; set; }
            public string Rs { get; set; }
        }
    }
}
