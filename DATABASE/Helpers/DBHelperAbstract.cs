using AGVSystemCommonNet6.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE.Helpers
{
    public abstract class DBHelperAbstract
    {

        protected readonly string connection_str;
        private DbContextHelper dbhelper;
        protected AGVSDbContext dbContext => dbhelper._context;
        public DBHelperAbstract()
        {
            connection_str = AGVSConfigulator.SysConfigs.DBConnection;
            dbhelper = new DbContextHelper(connection_str);
        }
    }
}
