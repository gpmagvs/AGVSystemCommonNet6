using AGVSystemCommonNet6.Vehicle_Control.VCS_ALARM;
using System.Net.NetworkInformation;

namespace AGVSystemCommonNet6.Abstracts
{
    public abstract class Connection : AGVAlarmReportable
    {
        public string IP;
        public int VMSPort;
        public int AGVsPort = 5216;
        public event EventHandler OnPingFail;
        public event EventHandler OnPingSuccess;
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
                        Task.Factory.StartNew(() => OnPingFail?.Invoke(this, EventArgs.Empty));
                    else
                        Task.Factory.StartNew(() => OnPingSuccess?.Invoke(this, EventArgs.Empty));
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


        protected async void PingServerCheckProcess()
        {
            while (AutoPingServerCheck)
            {
                await Task.Delay(1000);
                ping_success = await PingServer();
            }
        }

        public async Task<bool> PingServer()
        {
            PingOptions options = new PingOptions { Ttl = 128 };
            Ping pingSender = new Ping();
            string address = IP;
            try
            {
                PingReply reply = await pingSender.SendPingAsync(address, 5000, new byte[32], options);
                if (reply.Status != IPStatus.Success)
                {
                    Console.WriteLine(reply.Status);
                }
                return reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                Console.WriteLine($"Ping Error: {ex.Message}");
                return false;
            }
        }

    }
}
