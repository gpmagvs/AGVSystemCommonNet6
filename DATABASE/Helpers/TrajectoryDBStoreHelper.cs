using AGVSystemCommonNet6.AGVDispatch.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public List<List<clsTrajCoordination>> GetTrajectorys(string taskID)
        {
            using (var db = new AGVSDatabase())
            {
                IQueryable<clsTaskTrajecotroyStore> trajDatas = db.tables.TaskTrajecotroyStores.AsNoTracking().Where(t => t.TaskName.Contains(taskID));
                if (trajDatas.Any())
                {
                    return trajDatas.Select(traj => traj.Coordinations).ToList();
                }
                else
                    return new List<List<clsTrajCoordination>>();
            }
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
                            db.tables.TaskTrajecotroyStores.Add(new clsTaskTrajecotroyStore
                            {
                                TaskName = taskID + "_2",
                                AGVName = agvName,
                                CoordinationsJson = trajRecordjson
                            });

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

        public async Task<List<clsTaskTrajecotroyViewModel>> GetTrajectorysWithTimeRange(DateTime from, DateTime to)
        {
            using (var db = new AGVSDatabase())
            {
                try
                {

                    IQueryable<AGVDispatch.clsTaskDto> allTasksInRange = db.tables.Tasks.AsNoTracking().Where(order => order.RecieveTime >= from && order.FinishTime <= to)
                                                                                                       .OrderBy(order => order.RecieveTime);
                    IQueryable<clsTaskTrajecotroyStore> trajectoryStores = db.tables.TaskTrajecotroyStores.AsNoTracking();
                    IQueryable<string> allTaskNames = allTasksInRange.Select(order => order.TaskName);
                    IQueryable<clsTaskTrajecotroyStore> allTrajRecords = trajectoryStores.Where(trajRecord => allTaskNames.Contains(trajRecord.TaskName));

                    return allTrajRecords.Select(record => new clsTaskTrajecotroyViewModel(record.TaskName, record.AGVName, record.CoordinationsJson)).ToList();
                }
                catch (Exception ex)
                {
                    return new List<clsTaskTrajecotroyViewModel>();
                }

            }
        }


        public class clsTaskTrajecotroyViewModel : clsTaskTrajecotroyStore
        {

            public clsTaskTrajecotroyViewModel(string taskName, string agvName, string coordinationsJson)
            {
                base.TaskName = taskName;
                base.AGVName = agvName;
                CoordinationsJson = coordinationsJson;
            }
            internal new string CoordinationsJson { get; set; } = "";
            public new List<clsTrajCoordination> Coordinations
            {
                get
                {
                    return JsonConvert.DeserializeObject<List<clsTrajCoordination>>(CoordinationsJson);
                }
            }
        }
    }
}
