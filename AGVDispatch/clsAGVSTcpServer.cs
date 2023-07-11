using AGVSystemCommonNet6.Abstracts;
using AGVSystemCommonNet6.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSTcpServer : Connection
    {
        public Socket SocketServer;
        public event EventHandler<clsAGVSTcpIPClient> OnClientConnected;
        public override bool Connect()
        {
            try
            {
                SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                string ip = AGVSConfigulator.SysConfigs.VMSTcpServerIP;
                int port = AGVSConfigulator.SysConfigs.VMSTcpServerPort;
                SocketServer.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                SocketServer.Listen(1000);
                Task.Factory.StartNew(() =>
                {
                    AcceptListen();
                });

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void AcceptListen()
        {
            Socket client = SocketServer.Accept();
            Task.Factory.StartNew(() =>
            {
                AcceptListen();
            });
            OnClientConnected?.Invoke(this, new clsAGVSTcpIPClient { SocketClient = client });
        }


        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override bool IsConnected()
        {
            throw new NotImplementedException();
        }
    }
}
