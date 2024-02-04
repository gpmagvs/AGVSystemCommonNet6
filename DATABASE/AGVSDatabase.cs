using AGVSystemCommonNet6.Alarm;
using AGVSystemCommonNet6.DATABASE.Helpers;
using AGVSystemCommonNet6.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public static void Initialize()
        {
            using (var database = new AGVSDatabase())
            {
                _DefaultUsersCreate(database.tables.Users);

                _UnCheckedAlarmsSetAsCheckes(database.tables.SystemAlarms);

                _ = database.SaveChanges().Result;
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

            if (dev_user == null)
                users.Add(new UserEntity { UserName = "dev", Password = "12345678", Role = ERole.Developer });

            if (eng_user == null)
                users.Add(new UserEntity { UserName = "eng", Password = "12345678", Role = ERole.Engineer });

            if (op_user == null)
                users.Add(new UserEntity { UserName = "op", Password = "op", Role = ERole.Operator });
        }
    }
}
