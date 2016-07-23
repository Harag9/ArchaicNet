using System;
using System.Net.Sockets;
namespace ArchaicNet.TCP
{
    public partial class Client
    {
        /// <summary>
        /// Send a ping to the server.
        /// (Intended for testing latency)
        /// </summary>
        public void SendPing()
        {
            _pingTime = Environment.TickCount;
            if (_socket == null)
                return;
            var data = new byte[2];
            _socket?.BeginSend(data, 0, 2, SocketFlags.None, DoSend, null);
        }

        /// <summary>
        /// Send data to server.
        /// </summary>
        public void SendData(ref byte[] data)
        {
            try
            {
                var dataLength = data.Length;
                var newData = new byte[dataLength + 4];
                Buffer.BlockCopy(BitConverter.GetBytes(dataLength), 0, newData, 0, 4);
                Buffer.BlockCopy(data, 0, newData, 4, dataLength);
                _socket?.BeginSend(newData, 0, dataLength + 4, SocketFlags.None, DoSend, null);
            }
            catch
            {
                CrashReport?.Invoke("ConnectionNotEstablishedException");
                Disconnect();
            }
        }

        /// <summary>
        /// Send data to server. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
		public void SendData(ref byte[] data, int location)
        {
            try
            {
                var newData = new byte[location + 4];
                Buffer.BlockCopy(BitConverter.GetBytes(location), 0, newData, 0, 4);
                Buffer.BlockCopy(data, 0, newData, 4, location);
                _socket?.BeginSend(newData, 0, location + 4, SocketFlags.None, DoSend, null);
            }
            catch
            {
                CrashReport?.Invoke("ConnectionNotEstablishedException");
                Disconnect();
            }
        }

        private void DoSend(IAsyncResult ar)
        {
            _socket.EndSend(ar);
        }
    }
}