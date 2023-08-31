using AGVSystemCommonNet6.AGVDispatch.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DATABASE.Helpers
{
    public class TrajectoryDBStoreHelper : DBHelperAbstract
    {
        DbSet<clsTaskTrajecotroyStore> TrajSets => dbContext.TaskTrajecotroyStores;
        public TrajectoryDBStoreHelper()
        {
        }

        public List<clsTrajCoordination> GetTrajectory(string taskID)
        {
            clsTaskTrajecotroyStore? trajData = TrajSets.FirstOrDefault(t => t.TaskName == taskID);
            if (trajData != null) {
                return trajData.Coordinations;
            }
            else
                return new List<clsTrajCoordination>();
        }

        public void StoreTrajectory(string taskID, string agvName, double x, double y, double theta)
        {
            var coordination = new clsTrajCoordination
            {
                X = x,
                Y = y,
                Theta = theta
            };
            clsTaskTrajecotroyStore? existData = TrajSets.FirstOrDefault(traj => traj.TaskName == taskID);
            if (existData == null) //新增
            {
                List<clsTrajCoordination> trajCoordinationList = new List<clsTrajCoordination>() {
                   coordination
                };
              TrajSets.Add(new clsTaskTrajecotroyStore
                {
                    TaskName = taskID,
                    AGVName = agvName,
                    CoordinationsJson = JsonConvert.SerializeObject(trajCoordinationList)
                });
            }
            else
            {
                existData.AGVName = agvName;
                List<clsTrajCoordination>? exidtTrajList = JsonConvert.DeserializeObject<List<clsTrajCoordination>>(existData.CoordinationsJson);
                exidtTrajList.Add(coordination);
                existData.CoordinationsJson = JsonConvert.SerializeObject(exidtTrajList);

            }
            SaveChanges();
        }
    }
}
