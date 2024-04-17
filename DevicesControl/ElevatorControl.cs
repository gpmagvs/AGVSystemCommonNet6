using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.DevicesControl
{
    public class ElevatorControl
    {
        public string Host = "192.168.0.165";
        public int Port = 5020;

        public ElevatorControl()
        {
        }
        public async Task<bool> CallElevatorComeAndWait(int floor)
        {
            var result = await _SendToElevatorAndWaitResponse(new
            {
                floor = floor,
                door_open = true,
            });
            return result.Item1;
        }
        public async Task<bool> GoTo(int floor)
        {
            var result = await _SendToElevatorAndWaitResponse(new
            {
                floor = floor,
                door_open = true,
            });
            return result.Item1;
        }
        public async Task<bool> CloseDoor(int currentFloor)
        {
            var result = await _SendToElevatorAndWaitResponse(new
            {
                floor = currentFloor,
                door_open = false,
            });
            return result.Item1;
        }

        private async Task<(bool, string)> _SendToElevatorAndWaitResponse(object obj)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(Host, Port);
                byte[] sendBytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
                socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);
                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                while (socket.Available == 0)
                {
                    await Task.Delay(100);
                    if (cts.IsCancellationRequested)
                    {
                        return (false, "Elevator no response.");
                    }
                }
                return (true, "");
            }
        }
    }
}
