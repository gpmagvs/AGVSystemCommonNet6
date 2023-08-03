using AGVSystemCommonNet6.Alarm.VMS_ALARM;

namespace AGVSystemCommonNet6.Abstracts
{
    public abstract class Connection:AGVAlarmReportable
    {
        public string IP;
        public int Port;
        public Connection()
        {

        }
        public Connection(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
        }
        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract bool IsConnected();

    }
}
