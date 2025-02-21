using AGVSystemCommonNet6.AGVDispatch.RunMode;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.Sys;

namespace AGVSystem.Service
{
    public class SystemStatusDbStoreService

    {
        private AGVSDbContext _agvsDb;
        public SystemStatusDbStoreService(AGVSDbContext agvsDb)
        {
            _agvsDb = agvsDb;
        }

        public async Task InitSysStatusWithAppVersion(string appVersion)
        {
            try
            {

                if (_agvsDb.SysStatus.FirstOrDefault() == null)
                {
                    AGVSSystemStatus status = new AGVSSystemStatus
                    {
                        FieldName = AGVSConfigulator.SysConfigs.FieldName,
                        Version = appVersion
                    };
                    _agvsDb.SysStatus.Add(status);
                }
                else
                {
                    await ResetModesStore();
                    _agvsDb.SysStatus.First().Version = appVersion;
                    _agvsDb.SysStatus.First().FieldName = AGVSConfigulator.SysConfigs.FieldName;
                }
                await _agvsDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public async Task ResetModesStore()
        {
            if (_agvsDb.SysStatus.Any())
            {
                AGVSSystemStatus statusStore = _agvsDb.SysStatus.First();
                statusStore.RunMode = RUN_MODE.MAINTAIN;
                statusStore.HostConnMode = HOST_CONN_MODE.OFFLINE;
                statusStore.HostOperMode = HOST_OPER_MODE.LOCAL;
            }
            await _agvsDb.SaveChangesAsync();
        }

        public async Task ModifyRunModeStored(RUN_MODE mode)
        {
            if (_agvsDb.SysStatus.Any())
            {
                _agvsDb.SysStatus.First().RunMode = mode;
            }
            await _agvsDb.SaveChangesAsync();
        }

    }
}
