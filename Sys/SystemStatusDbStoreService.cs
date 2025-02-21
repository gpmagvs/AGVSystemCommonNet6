using AGVSystemCommonNet6.AGVDispatch.RunMode;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.DATABASE;
using AGVSystemCommonNet6.Sys;
using Microsoft.EntityFrameworkCore;

namespace AGVSystem.Service
{
    public class SystemStatusDbStoreService

    {
        private AGVSDbContext _agvsDb;

        private bool _isSysstatusDataExist => _agvsDb.SysStatus.AsNoTracking().Any();

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
            await ModifyRunModeStored(RUN_MODE.MAINTAIN);
            await ModifyHostModeStored(HOST_CONN_MODE.OFFLINE, HOST_OPER_MODE.LOCAL);
            await _agvsDb.SaveChangesAsync();
        }

        public async Task ModifyRunModeStored(RUN_MODE mode)
        {
            if (_isSysstatusDataExist)
            {
                _agvsDb.SysStatus.First().RunMode = mode;
            }
            await _agvsDb.SaveChangesAsync();
        }

        public async Task ModifyHostModeStored(HOST_CONN_MODE connectMode, HOST_OPER_MODE operMode)
        {
            try
            {

                if (_isSysstatusDataExist)
                {
                    AGVSSystemStatus statusStore = _agvsDb.SysStatus.First();
                    statusStore.HostConnMode = connectMode;
                    statusStore.HostOperMode = operMode;
                }
                await _agvsDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
