using SQLite;
using System.Drawing.Printing;
using System.Security.Claims;
using AGVSystemCommonNet6.Alarm.VMS_ALARM;
using AGVSystemCommonNet6.User;
using AGVSystemCommonNet6.Log;

namespace AGVSystemCommonNet6.Tools.Database
{
    public class DBhelper
    {
        private static SQLiteConnection db;

        public static void Initialize(string dbName = "VMS")
        {
            try
            {
                var databasePath = Path.Combine(Environment.CurrentDirectory, $"{dbName}.db");
                db = new SQLiteConnection(databasePath);
                db.CreateTable<clsAlarmCode>();
                db.CreateTable<UserEntity>();

                CreateDefaultUsers();

            }
            catch (System.Exception ex)
            {
                LOG.Critical($"初始化資料庫時發生錯誤＿{ex.Message}");
            }
        }

        public static void InsertAlarm(clsAlarmCode alarm)
        {
            db?.Insert(alarm);
        }


        public static void InsertUser(UserEntity user)
        {
            try
            {
                db.Insert(user);
            }
            catch (Exception ex)
            {
            }
        }

        public static int AlarmsTotalNum()
        {
            return db.Table<clsAlarmCode>().Count();
        }

        public static int ClearAllAlarm()
        {
            try
            {
                return db.Table<clsAlarmCode>().Delete(a => a.Time != null);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static List<clsAlarmCode> QueryAlarm(int page, int page_size = 16)
        {
            try
            {
                var query = db.Table<clsAlarmCode>().OrderByDescending(f => f.Time).Skip(page_size * (page - 1)).Take(page_size);
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
                Role = ERole.GOD,
                UserName = "GOD",
                Password = "66669999"
            });
        }


    }
}
