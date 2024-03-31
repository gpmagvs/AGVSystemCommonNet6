#define ping_debug

using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace AGVSystemCommonNet6.Abstracts
{
    public abstract class Connection : AGVAlarmReportable
    {
        public string IP;
        public int VMSPort;
        public int AGVsPort = 5216;
        public Action OnPingFail;
        public Action OnPingSuccess;

        public bool AutoPingServerCheck { get; set; } = true;
        private bool _ping_success = true;
        private bool ping_success
        {
            get => _ping_success;
            set
            {
                if (_ping_success != value)
                {
                    _ping_success = value;
                    if (!value)
                        OnPingFail();
                    else
                        OnPingSuccess();
                }
            }
        }
        public Connection()
        {

        }
        public Connection(string IP, int Port, bool AutoPingServerCheck = false)
        {
            this.IP = IP;
            this.VMSPort = Port;
            if (AutoPingServerCheck)
            {
                PingServerCheckProcess();
            }
        }
        public abstract Task<bool> Connect();
        public abstract void Disconnect();
        public abstract bool IsConnected();


        protected async Task PingServerCheckProcess()
        {
            while (AutoPingServerCheck)
            {
                try
                {
                    await Task.Delay(1000);
                    ping_success = await PingServer();

                }
                catch (Exception ex)
                {
                    LOG.ERROR($"Ping-{IP} 的過程中發生例外-{ex.Message}", ex, false);
                    ping_success = false;
                }
            }
        }

        public async Task<bool> PingServer()
        {
            const int timeout = 1000; // 1秒超時
            byte[] buffer = new byte[32];
            PingOptions options = new PingOptions { Ttl = 128 };

            using (Ping pingSender = new Ping())
            {
                try
                {
#if ping_debug
                    PingReply reply = await pingSender.SendPingAsync("192.168.23.23", timeout, buffer, options);
#else
                    PingReply reply = await pingSender.SendPingAsync(IP, timeout, buffer, options);
#endif
                    return reply.Status == IPStatus.Success;
                }
                catch (PingException ex)
                {
                    return false;
                }
            }
        }

        public virtual void ResetErrors()
        {
            _ping_success = true;
        }
    }
}
