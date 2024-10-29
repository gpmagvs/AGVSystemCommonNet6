﻿using AGVSystemCommonNet6.AGVDispatch.RunMode;
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
                _agvsDb.SysStatus.First().RunMode = RUN_MODE.MAINTAIN;
                _agvsDb.SysStatus.First().HostOperMode = HOST_OPER_MODE.LOCAL;
                _agvsDb.SysStatus.First().HostConnMode = HOST_CONN_MODE.OFFLINE;
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

        public async Task ModifyHostOperMode(HOST_OPER_MODE mode)
        {
            if (_agvsDb.SysStatus.Any())
            {
                _agvsDb.SysStatus.First().HostOperMode = mode;
            }
            await _agvsDb.SaveChangesAsync();
        }

        public async Task ModifyHostConnMode(HOST_CONN_MODE mode)
        {
            if (_agvsDb.SysStatus.Any())
            {
                _agvsDb.SysStatus.First().HostConnMode = mode;
            }
            await _agvsDb.SaveChangesAsync();
        }
    }
}
