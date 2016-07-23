using System;
using System.Net.Sockets;
namespace ArchaicNet.TCP.Static
{
    public partial class Server
    {
        /// <summary>
        /// Intended use to check for broken connections to
        /// keep IsConnected returning accurate values.
        /// </summary>
        public void SendCheckAlive(int index)
        {
            if (_socket[index] == null)
            {
                _unsignedIndex.Add(index);
                return;
            }
            var data = new byte[1];
            _socket[index].BeginSend(data, 0, 1, SocketFlags.None, DoSend, index);
        }

        private void SendReturnPing(int index)
        {
            if (_socket[index] == null)
            {
                _unsignedIndex.Add(index);
                return;
            }
            var data = new byte[2];
            _socket[index].BeginSend(data, 0, 2, SocketFlags.None, DoSend, index);
        }

        /// <summary>
        /// Send data to specified client.
        /// </summary>
        public void SendDataTo(int index, ref byte[] data)
        {
            if (_socket[index] == null)
            {
                _unsignedIndex.Add(index);
                return;
            }
            var dataLength = data.Length;
            var newData = new byte[dataLength + 4];
            Buffer.BlockCopy(BitConverter.GetBytes(dataLength), 0, newData, 0, 4);
            Buffer.BlockCopy(data, 0, newData, 4, dataLength);
            _socket[index].BeginSend(newData, 0, dataLength + 4, SocketFlags.None, DoSend, index);
        }

        /// <summary>
        /// Send data to specified client. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
        public void SendDataTo(int index, ref byte[] data, int location)
        {
            if (_socket[index] == null)
            {
                _unsignedIndex.Add(index);
                return;
            }
            var newData = new byte[location + 4];
            Buffer.BlockCopy(BitConverter.GetBytes(location), 0, newData, 0, 4);
            Buffer.BlockCopy(data, 0, newData, 4, location);
            _socket[index].BeginSend(newData, 0, location + 4, SocketFlags.None, DoSend, index);
        }

        /// <summary>
        /// Send data to all clients.
        /// </summary>
        public void SendDataToAll(ref byte[] data)
        {
            int i;
            for (i = 0; i <= HighIndex; i++)
                if (_socket[i] != null)
                    SendDataTo(i, ref data);
        }

        /// <summary>
        /// Send data to all clients. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
        public void SendDataToAll(ref byte[] data, int location)
        {
            int i;
            for (i = 0; i <= HighIndex; i++)
                if (_socket[i] != null)
                    SendDataTo(i, ref data, location);
        }

        /// <summary>
        /// Send data to all but specified client.
        /// </summary>
        public void SendDataToAllBut(int index, ref byte[] data)
        {
            int i;
            for (i = 0; i <= HighIndex; i++)
                if (_socket[i] != null)
                    if (i != index)
                        SendDataTo(i, ref data);
        }

        /// <summary>
        /// Send data to all but specified client. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
        public void SendDataToAllBut(int index, ref byte[] data, int location)
        {
            int i;
            for (i = 0; i <= HighIndex; i++)
                if (_socket[i] != null)
                    if (i != index)
                        SendDataTo(i, ref data, location);
        }

        private void DoSend(IAsyncResult ar)
        {
            var index = (int)ar.AsyncState;
            _socket[index].EndSend(ar);
        }
    }
}