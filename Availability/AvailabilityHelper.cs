using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AGVSystemCommonNet6.clsEnums;
using AGVSystemCommonNet6.Configuration;
using SQLite;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.Log;
using Microsoft.EntityFrameworkCore;

namespace AGVSystemCommonNet6.Availability
{
    public class AvailabilityHelper
    {
        public AvailabilityDto availability = new AvailabilityDto();

        private Dictionary<MAIN_STATUS, Stopwatch> StateWatchers = new Dictionary<MAIN_STATUS, Stopwatch>()
        {
            { MAIN_STATUS.IDLE,new Stopwatch() },
            { MAIN_STATUS.RUN,new Stopwatch() },
            { MAIN_STATUS.DOWN,new Stopwatch() },
            { MAIN_STATUS.Charging,new Stopwatch() },
            { MAIN_STATUS.Unknown,new Stopwatch() },
        };
        private MAIN_STATUS previousMainState;
        private RTAvailabilityDto currentAvailability = new RTAvailabilityDto();
        private SemaphoreSlim _DbActSemaphore = new SemaphoreSlim(1, 1);
        private MAIN_STATUS MainState
        {
            get => previousMainState;
            set
            {
                if (previousMainState != value)
                {
                    if (currentAvailability != null)
                    {
                        currentAvailability.EndTime = DateTime.Now;
                        SaveRealTimeAvailbilityToDatabase(currentAvailability);
                    }
                    AllWatchStop();
                    try
                    {

                        StateWatchers[value].Start();
                        currentAvailability.StartTime = DateTime.Now;
                        currentAvailability.Main_Status = value;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    currentAvailability.EndTime = DateTime.Now;
                    UpdateRealTimeAvailbilityToDataBase(currentAvailability);
                }
                previousMainState = value;
            }
        }

        public double IDLING_TIME => currentAvailability.Idle_duraction;

        public AvailabilityHelper(string agv_name)
        {
            availability.AGVName = agv_name;
            availability.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            currentAvailability = new RTAvailabilityDto()
            {
                StartTime = DateTime.Now,
                AGVName = agv_name
            };
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(3000);
                await RestoreDataFromDatabase();
                SyncAvaliabilityDataWorker();
            });
        }
        int lastDay = -1;
        private void SyncAvaliabilityDataWorker()
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                Stopwatch write_db_stopwatch = Stopwatch.StartNew();
                while (true)
                {
                    if (lastDay != DateTime.Now.Day)
                    {
                        availability.IDLE_TIME =
                        availability.RUN_TIME =
                        availability.DOWN_TIME =
                        availability.CHARGE_TIME =
                        availability.UNKNOWN_TIME = 0;
                    }
                    availability.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    availability.IDLE_TIME = StateWatchers[MAIN_STATUS.IDLE].IsRunning ? availability.IDLE_TIME + 1 : availability.IDLE_TIME;
                    availability.RUN_TIME = StateWatchers[MAIN_STATUS.RUN].IsRunning ? availability.RUN_TIME + 1 : availability.RUN_TIME;
                    availability.DOWN_TIME = StateWatchers[MAIN_STATUS.DOWN].IsRunning ? availability.DOWN_TIME + 1 : availability.DOWN_TIME;
                    availability.CHARGE_TIME = StateWatchers[MAIN_STATUS.Charging].IsRunning ? availability.CHARGE_TIME + 1 : availability.CHARGE_TIME;
                    availability.UNKNOWN_TIME = StateWatchers[MAIN_STATUS.Unknown].IsRunning ? availability.UNKNOWN_TIME + 1 : availability.UNKNOWN_TIME;

                    await Task.Delay(1000);
                    if (write_db_stopwatch.ElapsedMilliseconds > 10000)
                    {
                        await SaveDayAvailbilityToDatabase();
                        write_db_stopwatch.Restart();
                    }

                    lastDay = DateTime.Now.Day;
                }
            });
        }
        private async Task RestoreDataFromDatabase()
        {
            try
            {
                await Task.Delay(10);
                using (DbContextHelper aGVSDbContext = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    var avaExist = aGVSDbContext._context.Availabilitys.AsNoTracking().FirstOrDefault(av => av.KeyStr == availability.GetKey());
                    if (avaExist != null)
                    {
                        lastDay = avaExist.Time.Day;

                        availability.IDLE_TIME = avaExist.IDLE_TIME;
                        availability.DOWN_TIME = avaExist.DOWN_TIME;
                        availability.RUN_TIME = avaExist.RUN_TIME;
                        availability.CHARGE_TIME = avaExist.CHARGE_TIME;
                        availability.UNKNOWN_TIME = avaExist.UNKNOWN_TIME;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task UpdateRealTimeAvailbilityToDataBase(RTAvailabilityDto currentAvailability)
        {
            await _DbActSemaphore.WaitAsync();
            try
            {
                using (DbContextHelper aGVSDbContext = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    var currentData = aGVSDbContext._context.RealTimeAvailabilitys.FirstOrDefault(data => data.AGVName == currentAvailability.AGVName && data.StartTime == currentAvailability.StartTime);
                    if (currentData == null)
                    {
                        aGVSDbContext._context.RealTimeAvailabilitys.Add(currentAvailability);
                    }
                    else
                    {
                        currentData.EndTime = currentAvailability.EndTime;
                    }
                    var ret = await aGVSDbContext._context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _DbActSemaphore.Release();
            }
        }

        private async Task SaveRealTimeAvailbilityToDatabase(RTAvailabilityDto currentAvailability)
        {
            await _DbActSemaphore.WaitAsync();

            using (DbContextHelper aGVSDbContext = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
            {
                try
                {
                    var exist_data = aGVSDbContext._context.RealTimeAvailabilitys.FirstOrDefault(ad => ad.AGVName == currentAvailability.AGVName && ad.StartTime == currentAvailability.StartTime);
                    if (exist_data == null)
                    {
                        aGVSDbContext._context.RealTimeAvailabilitys.Add(currentAvailability);
                    }
                    else
                    {
                        exist_data.EndTime = currentAvailability.EndTime;
                    }
                    var ret = await aGVSDbContext._context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _DbActSemaphore.Release();
                }
            }
        }

        /// <summary>
        /// 儲存每日稼動
        /// </summary>
        private async Task SaveDayAvailbilityToDatabase()
        {
            await _DbActSemaphore.WaitAsync();

            try
            {
                using (DbContextHelper aGVSDbContext = new DbContextHelper(AGVSConfigulator.SysConfigs.DBConnection))
                {
                    var avaExist = aGVSDbContext._context.Availabilitys.FirstOrDefault(av => av.KeyStr == availability.GetKey());
                    if (avaExist == null)
                    {
                        aGVSDbContext._context.Availabilitys.Add(new AvailabilityDto
                        {
                            KeyStr = availability.GetKey(),
                            Time = availability.Time
                        });
                    }
                    else
                    {
                        avaExist.AGVName = availability.AGVName;
                        avaExist.IDLE_TIME = availability.IDLE_TIME;
                        avaExist.DOWN_TIME = availability.DOWN_TIME;
                        avaExist.RUN_TIME = availability.RUN_TIME;
                        avaExist.CHARGE_TIME = availability.CHARGE_TIME;
                        avaExist.UNKNOWN_TIME = availability.UNKNOWN_TIME;

                    }
                    var ret = await aGVSDbContext._context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }
            finally
            {
                _DbActSemaphore.Release();
            }

        }



        public void UpdateAGVMainState(MAIN_STATUS main_state)
        {
            MainState = main_state;
        }

        private void AllWatchStop()
        {
            foreach (var watch in StateWatchers)
            {
                watch.Value.Stop();
            }
        }

        public void ResetIDLEStartTime()
        {
            if (currentAvailability.Main_Status == MAIN_STATUS.IDLE)
            {
                var Y = currentAvailability.EndTime.Year;
                var M = currentAvailability.EndTime.Month;
                var D = currentAvailability.EndTime.Day;
                var h = currentAvailability.EndTime.Hour;
                var mm = currentAvailability.EndTime.Minute;
                var ss = currentAvailability.EndTime.Second;
                currentAvailability.StartTime = new DateTime(Y, M, D, h, mm, ss);
            }
        }
    }
}
