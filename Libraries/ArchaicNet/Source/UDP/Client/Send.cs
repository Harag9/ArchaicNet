using System;
namespace ArchaicNet.UDP
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
            _socket?.BeginSend(data, 2, _peer, DoSend, null);
        }

        /// <summary>
        /// Send data to server.
        /// </summary>
        public void SendData(ref byte[] data)
        {
            try
            {
                var dataLength = data.Length;
                var newData = new byte[dataLength];
                Buffer.BlockCopy(data, 0, newData, 0, dataLength);
                _socket?.BeginSend(newData, dataLength, _peer, DoSend, null);
            }
            catch { return; }
        }

        /// <summary>
        /// Send data to server. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
		public void SendData(ref byte[] data, int location)
        {
            try
            {
                var newData = new byte[location];
                Buffer.BlockCopy(data, 0, newData, 0, location);
                _socket?.BeginSend(newData, location, _peer, DoSend, null);
            }
            catch { return; }
        }

        private void DoSend(IAsyncResult ar)
        {
            _socket.EndSend(ar);
        }
    }
}