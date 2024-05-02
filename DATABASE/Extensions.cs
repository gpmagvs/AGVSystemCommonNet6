using AGVSystemCommonNet6.Microservices.VMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE
{
    public static class Extensions
    {
        public static clsAGVStateDto GetAGVState(this AGVSDatabase db, string agvName)
        {
            return db.tables.AgvStates.FirstOrDefault(dto => dto.AGV_Name == agvName);
        }
    }
}
