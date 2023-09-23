using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Availability;
using AGVSystemCommonNet6.TASK;
using AGVSystemCommonNet6.User;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace AGVSystemCommonNet6.DATABASE
{
    public class AGVSDbContext : DbContext
    {
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
        public AGVSDbContext(DbContextOptions<AGVSDbContext> options)
            : base(options)
        {
        }
    }
}
