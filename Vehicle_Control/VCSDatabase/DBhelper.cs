using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.User;
using AGVSystemCommonNet6.Vehicle_Control.Models;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using SQLite;
using static AGVSystemCommonNet6.Log.clsAGVSLogAnaylsis;

namespace AGVSystemCommonNet6.Vehicle_Control.VCSDatabase
{
    public class DBhelper
    {
        public static SQLiteConnection db;
        public static string databasePath { get; private set; } = "database.db";
        public static Action<string> OnDataBaseChanged;
        public static void Initialize(string dbName = "VMS")
        {
            try
            {
                databasePath = Path.Combine(Environment.CurrentDirectory, $"{dbName}.db");
                db = new SQLiteConnection(databasePath);
                db.CreateTable<clsAlarmCode>();
                db.CreateTable<UserEntity>();
                db.CreateTable<clsParkingAccuracy>();
                CreateDefaultUsers();
                db.TableChanged += Db_TableChanged;
            }
            catch (Exception ex)
            {
                LOG.Critical($"初始化資料庫時發生錯誤＿{ex.Message}");
            }
        }

        private static void Db_TableChanged(object? sender, NotifyTableChangedEventArgs e)
        {
            RaiseDataBaseChangedEvent();
        }

        public static void InsertAlarm(clsAlarmCode alarm)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    db?.Insert(alarm);
                }
                catch (Exception ex)
                {
                    LOG.ERROR(ex);
                }
            });
        }

        public static void InsertParkingAccuracy(clsParkingAccuracy parkingAccuracy)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {

                    db?.Insert(parkingAccuracy);
                }
                catch (Exception ex)
                {
                    LOG.ERROR(ex);
                }
            });
        }

        public static void InsertUser(UserEntity user)
        {
            try
            {
                if (db.Table<UserEntity>().FirstOrDefault(user_ => user_.UserName == user.UserName) != null)
                    return;
                db.Insert(user);
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }
        }

        /// <summary>
        /// 儲存AGV狀態
        /// </summary>
        /// <param name="status"></param>
        public static void AddAgvStatusData(clsAGVStatusTrack status)
        {
            try
            {
                var _exist_status = db.Table<clsAGVStatusTrack>().FirstOrDefault(d => d.Time == status.Time);
                bool _already_added = _exist_status != null;

                if (_already_added)
                {
                    db.Delete(_exist_status);
                }
                db.Insert(status);
            }
            catch (SQLite.SQLiteException ex)
            {
                db.CreateTable(typeof(clsAGVStatusTrack));
                db.Insert(status);
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }
        }

        /// <summary>
        /// 儲存振動記錄數據
        /// </summary>
        /// <param name="status"></param>
        public static void AddVibrationStatusRecord(clsVibrationStatusWhenAGVMoving status)
        {
            try
            {
                db.Insert(status);
            }
            catch (SQLite.SQLiteException ex)
            {
                db.CreateTable(typeof(clsVibrationStatusWhenAGVMoving));
                db.Insert(status);
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }
        }
        public static int AlarmsTotalNum(DateTime from, DateTime to, string alarm_type = "All", int code = 0)
        {
            try
            {

                if (alarm_type.ToLower() == "all")
                    return db.Table<clsAlarmCode>().Where(al => al.Time >= from && al.Time <= to).ToList().Where(al => code == 0 ? al.Code > 0 : al.Code == code).Count();
                else if (alarm_type.ToLower() == "alarm")
                    return db.Table<clsAlarmCode>().Where(al => al.Time >= from && al.Time <= to).Where(al => al.ELevel == clsAlarmCode.LEVEL.Alarm).ToList().Where(al => code == 0 ? al.Code > 0 : al.Code == code).Count();
                else
                    return db.Table<clsAlarmCode>().Where(al => al.Time >= from && al.Time <= to).Where(al => al.ELevel == clsAlarmCode.LEVEL.Warning).ToList().Where(al => code == 0 ? al.Code > 0 : al.Code == code).Count();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static int AddUDULDRecord(LDULDRecord record)
        {
            try
            {
                return db.Insert(record);
            }
            catch (SQLite.SQLiteException ex)
            {
                db.CreateTable(typeof(LDULDRecord));
                return db.Insert(record);
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
                return 0;
            }
        }
        public static int ModifyUDLUDRecord(LDULDRecord reoc)
        {
            try
            {
                var exist_record = db.Table<LDULDRecord>().FirstOrDefault(rd => rd.StartTime == reoc.StartTime);
                if (exist_record != null)
                {
                    return db.Update(reoc);
                }
                else
                    return AddUDULDRecord(reoc);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static int ClearAllAlarm()
        {
            try
            {
                return db.Table<clsAlarmCode>().Delete(a => a.Time != null);
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
                return 0;
            }

        }

        private static void CreateDefaultUsers()
        {
            InsertUser(new UserEntity
            {
                Role = ERole.Engineer,
                UserName = "ENG",
                Password = "12345678"
            });
            InsertUser(new UserEntity
            {
                Role = ERole.Developer,
                UserName = "DEV",
                Password = "12345678"
            });
            InsertUser(new UserEntity
            {
                Role = ERole.GOD,
                UserName = "GOD",
                Password = "66669999"
            });
        }

        private static void RaiseDataBaseChangedEvent()
        {
            if (OnDataBaseChanged != null)
            {
                OnDataBaseChanged(databasePath);
            }
        }

        public static List<clsTransferResult> QueryTodayTransferRecord()
        {
            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime to = from.AddDays(1);
            return QueryTransferRecord(from, to);

        }
        public static List<clsTransferResult> QueryTransferRecord(DateTime from, DateTime to)
        {
            List<clsTransferResult> outputs = new List<clsTransferResult>();
            var filtered = db.Table<clsAGVStatusTrack>().Where(stat => stat.Time >= from && stat.Time <= to);
            if (filtered.Count() == 0)
                return new List<clsTransferResult>();
            //掰運-放貨
            var loadActions = filtered.Where(stat => stat.Status == clsEnums.SUB_STATUS.RUN && stat.TaskAction == AGVDispatch.Messages.ACTION_TYPE.Load);
            foreach (clsAGVStatusTrack state in loadActions)
            {
                var taskName = state.ExecuteTaskName;
                //取貨
                var transferActions = filtered.Where(stat => stat.ExecuteTaskName == taskName);
                if (transferActions.Any(st => st.TaskAction == AGVDispatch.Messages.ACTION_TYPE.Unload))
                {
                    var initState = transferActions.First();
                    var unloadActionStartState = transferActions.First(st => st.TaskAction == AGVDispatch.Messages.ACTION_TYPE.Unload);
                    var loadActionStartState = transferActions.First(st => st.TaskAction == AGVDispatch.Messages.ACTION_TYPE.Load);
                    var finalState = filtered.FirstOrDefault(st => st.Time > loadActionStartState.Time);
                    finalState = finalState == null ? loadActionStartState : finalState;

                    clsTransferResult transferInfo = new clsTransferResult
                    {
                        StartTime = initState.Time,
                        EndTime = finalState.Time,
                        StartStatus = new AGVDispatch.Messages.RunningStatus
                        {
                            Odometry = initState.Odometry,
                            Electric_Volume = new double[2] { initState.BatteryLevel1, initState.BatteryLevel2 }
                        },
                        EndStatus = new AGVDispatch.Messages.RunningStatus
                        {
                            Odometry = finalState.Odometry,
                            Electric_Volume = new double[2] { finalState.BatteryLevel1, finalState.BatteryLevel2 }
                        },
                        From = unloadActionStartState.DestineTag,
                        To = loadActionStartState.DestineTag,
                        TaskName = taskName,
                        StartLoc = initState.DestineTag
                    };
                    outputs.Add(transferInfo);
                }
            }

            return outputs;
        }

        public struct Query
        {
            public static List<clsParkingAccuracy> QueryParkingAccuracy(int tag, string startTimeStr, string endTimeStr, string taskName)
            {
                try
                {

                    DateTime startTime = DateTime.Parse(startTimeStr);
                    DateTime endTime = DateTime.Parse(endTimeStr);
                    var Timematch = db?.Table<clsParkingAccuracy>().Where(acq => acq.Time >= startTime && acq.Time <= endTime);
                    if (tag != -1)
                        return Timematch.Where(acq => acq.ParkingTag == tag).OrderBy(acq => acq.ParkingTag).ToList();
                    else
                        return Timematch.Where(acq => acq.TaskName.Contains(taskName)).OrderBy(acq => acq.ParkingTag).ToList();
                }
                catch (Exception ex)
                {
                    return new List<clsParkingAccuracy>();
                }
            }

            public static List<clsAlarmCode> QueryAlarmCodeClassifies()
            {
                try
                {
                    return db.Table<clsAlarmCode>().ToList().DistinctBy(al => al.EAlarmCode).OrderBy(al => al.Code).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static List<clsAlarmCode> QueryAlarm(DateTime from, DateTime to, int page, int page_size = 16, string alarm_type = "All", int code = 0)
            {
                try
                {
                    var query = new List<clsAlarmCode>();
                    if (alarm_type.ToLower() == "all")
                    {
                        query = db.Table<clsAlarmCode>().Where(al => al.Time >= from && al.Time <= to).ToList().Where(al => code == 0 ? al.Code > 0 : al.Code == code).OrderByDescending(f => f.Time).Skip(page_size * (page - 1)).Take(page_size).ToList();
                    }
                    else
                    {
                        clsAlarmCode.LEVEL filterLevel = alarm_type.ToLower() == "alarm" ? clsAlarmCode.LEVEL.Alarm : clsAlarmCode.LEVEL.Warning;
                        query = db.Table<clsAlarmCode>().Where(al => al.Time >= from && al.Time <= to).ToList().Where(al => code == 0 ? al.Code > 0 : al.Code == code).OrderByDescending(f => f.Time).Where(al => al.ELevel == filterLevel).Skip(page_size * (page - 1)).Take(page_size).ToList();
                    }
                    return query;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static UserEntity QueryUserByName(string userName)
            {
                try
                {
                    return db.Table<UserEntity>().FirstOrDefault(user => user.UserName.ToUpper() == userName.ToUpper());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            public static List<string>? QueryAllParkLoc()
            {
                return db?.Table<clsParkingAccuracy>().Select(record => $"{record.ParkingTag}:{record.ParkingLocation}").Distinct().ToList();
            }

            public static object QueryVibrationRecordsByTaskName(string taskName)
            {
                var tasksStaus = db?.Table<clsVibrationStatusWhenAGVMoving>().Where(s => s.TaskName == taskName);
                var data = tasksStaus.OrderBy(t => t.Time).SelectMany(t => t.VibrationRecords).ToList();
                var output = new
                {
                    data = data,
                    max_x = data.Max(v => Math.Abs(v.AccelermetorValue.x)),
                    max_y = data.Max(v => Math.Abs(v.AccelermetorValue.y)),
                    max_z = data.Max(v => Math.Abs(v.AccelermetorValue.z)),
                    avg_x = data.Average(v => Math.Abs(v.AccelermetorValue.x)),
                    avg_y = data.Average(v => Math.Abs(v.AccelermetorValue.y)),
                    avg_z = data.Average(v => Math.Abs(v.AccelermetorValue.z))
                };
                return output;
            }


            public static object QueryVibrationRecordsByTime(DateTime from, DateTime to)
            {
                try
                {

                    var tasksStaus = db?.Table<clsVibrationStatusWhenAGVMoving>().Where(s => s.Time >= from && s.Time <= to);
                    var filtered = tasksStaus.OrderBy(t => t.Time).Where(t => t.VirbrationRecordsJsonString != "");
                    var data = filtered.SelectMany(t => t.VibrationRecords).ToList();
                    var output = new
                    {
                        data = data,
                        max_x = data.Max(v => Math.Abs(v.AccelermetorValue.x)),
                        max_y = data.Max(v => Math.Abs(v.AccelermetorValue.y)),
                        max_z = data.Max(v => Math.Abs(v.AccelermetorValue.z)),
                        avg_x = data.Average(v => Math.Abs(v.AccelermetorValue.x)),
                        avg_y = data.Average(v => Math.Abs(v.AccelermetorValue.y)),
                        avg_z = data.Average(v => Math.Abs(v.AccelermetorValue.z))
                    };
                    return output;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static clsEnums.SUB_STATUS QueryStatusWithTime(DateTime time)
            {
                var lastStatus = db.Table<clsAGVStatusTrack>().LastOrDefault(status => status.Time < time);
                if (lastStatus != null)
                    return lastStatus.Status;
                else
                    return clsEnums.SUB_STATUS.UNKNOWN;
            }

            public static List<clsAGVStatusTrack> QueryStatusWithTimeRange(DateTime from, DateTime to)
            {
                var lastStatus = db.Table<clsAGVStatusTrack>().Where(status => status.Time >= from && status.Time <= to);
                return lastStatus.ToList();
            }
        }
    }
}
