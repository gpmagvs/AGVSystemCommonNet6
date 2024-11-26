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
    public class TrajectoryDBStoreHelper
    {
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public TrajectoryDBStoreHelper()
        {
        }

        public List<clsTrajCoordination> GetTrajectory(string taskID)
        {
            using (var db = new AGVSDatabase())
            {
                clsTaskTrajecotroyStore? trajData = db.tables.TaskTrajecotroyStores.FirstOrDefault(t => t.TaskName == taskID);
                if (trajData != null)
                {
                    return trajData.Coordinations;
                }
                else
                    return new List<clsTrajCoordination>();
            }
        }
        public async Task<(bool success, string error_msg)> StoreTrajectory(string taskID, string agvName, string trajRecordjson)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                using (var db = new AGVSDatabase())
                {
                    try
                    {
                        clsTaskTrajecotroyStore? existData = db.tables.TaskTrajecotroyStores.FirstOrDefault(t => t.TaskName == taskID);

                        if (existData == null) //新增
                        {
                            db.tables.TaskTrajecotroyStores.Add(new clsTaskTrajecotroyStore
                            {
                                TaskName = taskID,
                                AGVName = agvName,
                                CoordinationsJson = trajRecordjson
                            });
                        }
                        else
                        {
                            existData.AGVName = agvName;
                            existData.CoordinationsJson = trajRecordjson;

                        }
                        await db.SaveChanges();
                        return (true, "");
                    }
                    catch (Exception ex)
                    {
                        return (false, ex.Message);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }

        }

        public async Task<(bool success, string error_msg)> StoreTrajectory(string taskID, string agvName, double x, double y, double theta)
        {
            var coordination = new clsTrajCoordination
            {
                X = x,
                Y = y,
                Theta = theta
            };

            using (var db = new AGVSDatabase())
            {
                try
                {
                    clsTaskTrajecotroyStore? existData = db.tables.TaskTrajecotroyStores.FirstOrDefault(t => t.TaskName == taskID);

                    if (existData == null) //新增
                    {
                        List<clsTrajCoordination> trajCoordinationList = new List<clsTrajCoordination>() { coordination };
                        db.tables.TaskTrajecotroyStores.Add(new clsTaskTrajecotroyStore
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
                    await db.SaveChanges();
                    return (true, "");
                }
                catch (Exception ex)
                {
                    return (false, ex.Message);
                }

            }

        }
    }
}
