using AGVSystemCommonNet6.DATABASE.Helpers;
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
        public AGVSDatabase():base() { }
    }
}
