﻿using AGVSystemCommonNet6.Abstracts;
using AGVSystemCommonNet6.Configuration;
using System.Net;
using System.Net.Sockets;

namespace AGVSystemCommonNet6.AGVDispatch
{
    public partial class clsAGVSTcpServer : Connection
    {
        public Socket SocketServer;
        public event EventHandler<clsAGVSTcpClientHandler> OnClientConnected;
        public override async Task<bool> Connect()
        {
            try
            {
                SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IP = AGVSConfigulator.SysConfigs.VMSTcpServerIP;
                VMSPort = AGVSConfigulator.SysConfigs.VMSTcpServerPort;
                SocketServer.Bind(new IPEndPoint(IPAddress.Parse(IP), VMSPort));
                SocketServer.Listen(1000);
                Task.Factory.StartNew(() =>
                {
                    AcceptListen();
                });
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AcceptListen()
        {
            Socket client = SocketServer.Accept();
            Task.Factory.StartNew(() =>
            {
                AcceptListen();
            });
            OnClientConnected?.Invoke(this, new clsAGVSTcpClientHandler { SocketClient = client });
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
