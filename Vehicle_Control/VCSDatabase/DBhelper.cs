﻿using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.User;
using AGVSystemCommonNet6.Vehicle_Control.Models;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using SQLite;

namespace AGVSystemCommonNet6.Vehicle_Control.VCSDatabase
{
    public class DBhelper
    {
        private static SQLiteConnection db;
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
                db.Insert(status);
            }
            catch  (SQLite.SQLiteException ex)
            {
                db.CreateTable(typeof(clsAGVStatusTrack));
                db.Insert(status);
            }
            catch (Exception ex)
            {
                LOG.ERROR(ex);
            }
        }

        public static int AlarmsTotalNum(string alarm_type = "All")
        {
            if (alarm_type.ToLower() == "all")
                return db.Table<clsAlarmCode>().Count();
            else if (alarm_type.ToLower() == "alarm")
                return db.Table<clsAlarmCode>().Where(al => al.ELevel == clsAlarmCode.LEVEL.Alarm).Count();
            else
                return db.Table<clsAlarmCode>().Where(al => al.ELevel == clsAlarmCode.LEVEL.Warning).Count();
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

        public static List<clsAlarmCode> QueryAlarm(int page, int page_size = 16, string alarm_type = "All")
        {
            try
            {
                TableQuery<clsAlarmCode> query = null;
                if (alarm_type.ToLower() == "all")
                {
                    query = db.Table<clsAlarmCode>().OrderByDescending(f => f.Time).Skip(page_size * (page - 1)).Take(page_size);
                }
                else
                {
                    clsAlarmCode.LEVEL filterLevel = alarm_type.ToLower() == "alarm" ? clsAlarmCode.LEVEL.Alarm : clsAlarmCode.LEVEL.Warning;
                    query = db.Table<clsAlarmCode>().OrderByDescending(f => f.Time).Where(al => al.ELevel == filterLevel).Skip(page_size * (page - 1)).Take(page_size);
                }
                return query.ToList();
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

        public static List<string>? QueryAllParkLoc()
        {
            return db?.Table<clsParkingAccuracy>().Select(record => $"{record.ParkingTag}:{record.ParkingLocation}").Distinct().ToList();
        }

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

        private static void RaiseDataBaseChangedEvent()
        {
            if (OnDataBaseChanged != null)
            {
                OnDataBaseChanged(databasePath);
            }
        }
    }
}
