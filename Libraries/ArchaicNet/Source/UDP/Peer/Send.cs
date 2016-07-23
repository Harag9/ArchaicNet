using System;
using System.Net;

namespace ArchaicNet.UDP
{
    public partial class Peer
    {
        /// <summary>
        /// Send a ping to another peer.
        /// (Intended for testing latency)
        /// </summary>
        public void SendPing(IPEndPoint ep)
        {
            _pingTime = Environment.TickCount;
            if (_socket == null)
                return;
            var data = new byte[2];
            _socket?.BeginSend(data, 2, ep, DoSend, null);
        }

        /// <summary>
        /// Send a ping to another peer.
        /// (Intended for testing latency)
        /// </summary>
        public void SendPing(string ip, int port)
        {
            _pingTime = Environment.TickCount;
            if (_socket == null)
                return;
            var data = new byte[2];
            _socket?.BeginSend(data, 2, new IPEndPoint(IPAddress.Parse(ip),port), DoSend, null);
        }

        private void SendReturnPing(IPEndPoint ep)
        {
            if (_socket == null)
                return;
            var data = new byte[1];
            _socket?.BeginSend(data, 1, ep, DoSend, null);
        }

        /// <summary>
        /// Send data to another peer.
        /// </summary>
        public void SendData(IPEndPoint ep, ref byte[] data)
        {
            try
            {
                var dataLength = data.Length;
                var newData = new byte[dataLength];
                Buffer.BlockCopy(data, 0, newData, 0, dataLength);
                _socket?.BeginSend(newData, dataLength, ep, DoSend, null);
            }
            catch { return; }
        }

        /// <summary>
        /// Send data to another peer.
        /// </summary>
        public void SendData(string ip, int port, ref byte[] data)
        {
            try
            {
                var dataLength = data.Length;
                var newData = new byte[dataLength];
                Buffer.BlockCopy(data, 0, newData, 0, dataLength);
                _socket?.BeginSend(newData, dataLength, new IPEndPoint(IPAddress.Parse(ip), port), DoSend, null);
            }
            catch { return; }
        }

        /// <summary>
        /// Send data to another peer. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
		public void SendData(IPEndPoint ep, ref byte[] data, int location)
        {
            try
            {
                var newData = new byte[location];
                Buffer.BlockCopy(data, 0, newData, 0, location);
                _socket?.BeginSend(newData, location, ep, DoSend, null);
            }
            catch { return; }
        }

        /// <summary>
        /// Send data to another peer. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
		public void SendData(string ip, int port, ref byte[] data, int location)
        {
            try
            {
                var newData = new byte[location];
                Buffer.BlockCopy(data, 0, newData, 0, location);
                _socket?.BeginSend(newData, location, new IPEndPoint(IPAddress.Parse(ip), port), DoSend, null);
            }
            catch { return; }
        }

        private void DoSend(IAsyncResult ar)
        {
            _socket.EndSend(ar);
        }
    }
}