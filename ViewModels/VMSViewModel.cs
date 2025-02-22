﻿using AGVSystemCommonNet6;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using static AGVSystemCommonNet6.clsEnums;

namespace AGVSystemCommonNet6.ViewModels
{
    public class VMSViewModel : IDisposable
    {
        private bool disposedValue;

        public clsRunningStatus RunningStatus { get; set; }
        public ONLINE_STATE OnlineStatus { get; set; }
        public VMSBaseProp BaseProps { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                RunningStatus = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
