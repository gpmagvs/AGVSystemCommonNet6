using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.Availability;
using AGVSystemCommonNet6.Equipment;
using AGVSystemCommonNet6.Maintainance;
using AGVSystemCommonNet6.Material;
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
    }
}
