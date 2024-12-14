using AGVSystemCommonNet6.User;
using AGVSystemCommonNet6.Vehicle_Control.Models;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using NLog;
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
        public static bool database_created => db != null;

        private static Logger LOG = LogManager.GetCurrentClassLogger();
        public static void Initialize(string dbName = "VMS")
        {
            try
            {
                databasePath = Path.Combine(Environment.CurrentDirectory, $"{dbName}.db");
                db = new SQLiteConnection(databasePath);
                CreateTablesResult results = db.CreateTables<clsAlarmCode, UserEntity, clsParkingAccuracy, clsAGVStatusTrack, clsVibrationStatusWhenAGVMoving>();
                LOG.Info(string.Join("\r\n", results.Results.Select(kp => $"Database table created result-{kp.Key.Name}=>{kp.Value}")));
                CreateTableResult result = db.CreateTable<LDULDRecord>();
                LOG.Info($"Database table created result-LDULDRecord=>{result}");

                CreateDefaultUsers();
                db.TableChanged += Db_TableChanged;
            }
            catch (Exception ex)
            {
                LOG.Fatal($"初始化資料庫時發生錯誤＿{ex.Message}");
                db = null;
            }
        }

        private static void Db_TableChanged(object? sender, NotifyTableChangedEventArgs e)
        {
            RaiseDataBaseChangedEvent();
        }

        public static void InsertAlarm(clsAlarmCode alarm)
        {
            if (!database_created)
                return;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    db?.Insert(alarm);
                }
                catch (Exception ex)
                {
                    LOG.Error(ex);
                }
            });
        }

        public static void InsertParkingAccuracy(clsParkingAccuracy parkingAccuracy)
        {
            if (!database_created)
                return;

            Task.Factory.StartNew(() =>
            {
                try
                {

                    db?.Insert(parkingAccuracy);
                }
                catch (Exception ex)
                {
                    LOG.Error(ex);
                }
            });
        }

        public static void InsertUser(UserEntity user)
        {
            if (!database_created)
                return;

            try
            {
                if (db.Table<UserEntity>().FirstOrDefault(user_ => user_.UserName == user.UserName) != null)
                    return;
                db.Insert(user);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }

        /// <summary>
        /// 儲存AGV狀態
        /// </summary>
        /// <param name="status"></param>
        public static void AddAgvStatusData(clsAGVStatusTrack status)
        {
            if (!database_created)
                return;

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
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }

        /// <summary>
        /// 儲存振動記錄數據
        /// </summary>
        /// <param name="status"></param>
        public static void AddVibrationStatusRecord(clsVibrationStatusWhenAGVMoving status)
        {
            if (!database_created)
                return;

            try
            {
                db.Insert(status);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }
        public static int AlarmsTotalNum(DateTime from, DateTime to, string alarm_type = "All", int code = 0)
        {
            if (!database_created)
                return -1;

            try
            {
                TableQuery<clsAlarmCode> query = db.Table<clsAlarmCode>().Where(al => al.Time >= from && al.Time <= to);
                // 處理 Code 條件
                if (code == 0)
                    query = query.Where(al => al.Code > 0);
                else
                    query = query.Where(al => al.Code == code);

                // 處理 alarm_type 條件
                if (alarm_type.ToLower() != "all")
                {
                    clsAlarmCode.LEVEL filterLevel = alarm_type.ToLower() == "alarm" ? clsAlarmCode.LEVEL.Alarm : clsAlarmCode.LEVEL.Warning;
                    query = query.Where(al => al.ELevel == filterLevel);
                }

                return query.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int AddUDULDRecord(LDULDRecord record)
        {
            if (!database_created)
                return -1;

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
                LOG.Error(ex);
                return 0;
            }
        }
        public static int ModifyUDLUDRecord(LDULDRecord reoc)
        {
            if (!database_created)
                return -1;

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
            if (!database_created)
                return -1;

            try
            {
                return db.Table<clsAlarmCode>().Delete(a => a.Time != null);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
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

        internal static void RecoveryWithAlarmTable(List<clsAlarmCode> alarmList)
        {
            try
            {
                TableQuery<clsAlarmCode>? alarms = db?.Table<clsAlarmCode>();
                foreach (clsAlarmCode item in alarms)
                {
                    clsAlarmCode? definedAlarm = alarmList.FirstOrDefault(al => al.Code == item.Code);
                    if (definedAlarm != null && item.CN != definedAlarm.CN)
                    {
                        db?.Table<clsAlarmCode>().Delete(t => t.Time == item.Time);
                        string oriCN = item.CN;
                        string oriDes = item.Description;
                        item.CN = definedAlarm.CN;
                        item.Description = definedAlarm.Description;
                        db?.Insert(item);
                        Console.WriteLine($"Update alarm description from {oriCN}({oriDes}) to {item.CN}({item.Description})");
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }
        internal static void RemoveOldAlarm(DateTime occurTimeLessThan)
        {
            try
            {
                LOG.Trace($"Start Delete Old Alarm Where Occur Time Less than {occurTimeLessThan}");
                TableQuery<clsAlarmCode>? alarms = db?.Table<clsAlarmCode>();
                List<clsAlarmCode> oldAlarms = alarms.Where(al => al.Time < occurTimeLessThan).ToList();
                foreach (clsAlarmCode item in oldAlarms)
                {
                    db?.Table<clsAlarmCode>().Delete(t => t.Time == item.Time);
                }
                LOG.Trace($"Delete {oldAlarms.Count()} alarms.");
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
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
                    return db.Table<clsAlarmCode>().DistinctBy(al => al.EAlarmCode).OrderBy(al => al.Code).ToList();
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
                    // 如果表是空的，直接返回空列表
                    if (!db.Table<clsAlarmCode>().Any())
                        return new List<clsAlarmCode>();

                    // 建立基礎查詢
                    TableQuery<clsAlarmCode> query = db.Table<clsAlarmCode>().Where(al => al.Time >= from && al.Time <= to);

                    // 處理 Code 條件
                    if (code == 0)
                        query = query.Where(al => al.Code > 0);
                    else
                        query = query.Where(al => al.Code == code);

                    // 處理 alarm_type 條件
                    if (alarm_type.ToLower() != "all")
                    {
                        clsAlarmCode.LEVEL filterLevel = alarm_type.ToLower() == "alarm" ? clsAlarmCode.LEVEL.Alarm : clsAlarmCode.LEVEL.Warning;
                        query = query.Where(al => al.ELevel == filterLevel);
                    }

                    // 排序、分頁並返回結果
                    return query.OrderByDescending(f => f.Time)
                                .Skip(page_size * (page - 1))
                                .Take(page_size)
                                .ToList();
                }
                catch (Exception)
                {
                    throw; // 直接 throw 即可
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
