using AGVSystemCommonNet6.DATABASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Material
{
    public class clsStationInfoManager
    {
        private static AGVSDatabase database;

        private static bool Initialized = false;
        public clsStationInfoManager() { }

        public static void Initialize()
        {
            database = new AGVSDatabase();
            Initialized = true;
        }

        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 更新WIP/撈爪的料況
        /// </summary>
        /// <param name="stationStatus"></param>
        /// <returns></returns>
        private static async Task UpdateStationInfo(clsStationStatus stationStatus)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                if (!Initialized)
                    Initialize();
                database.tables.StationStatus.Add(stationStatus);
                await database.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { semaphoreSlim.Release(); }
        }

    }
}
