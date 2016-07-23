using System;
using System.Collections.Generic;

namespace ArchaicNet.UDP
{
    public partial class Server
    {
        /// <summary>
        /// Intended use to check for broken connections to
        /// keep IsConnected returning accurate values.
        /// </summary>
		public void SendCheckAlive(int index)
        {
            if (_peer.ContainsKey(index))
                if (_peer[index] == null) { return; }

            var data = new byte[1];
            if (_peer.ContainsKey(index))
                _socket.BeginSend(data, 1, _peer[index], DoSend, null);
        }

        private void SendReturnPing(int index)
        {
            if (_peer.ContainsKey(index))
                if (_peer[index] == null) { return; }

            var data = new byte[2];
            if (_peer.ContainsKey(index))
                _socket.BeginSend(data, 2, _peer[index], DoSend, null);
        }

        /// <summary>
        /// Send data to specified client.
        /// </summary>
        public void SendDataTo(int index, ref byte[] data)
        {
            if (_peer.ContainsKey(index))
                if (_peer[index] == null) { return; }

            var dataLength = data.Length;
            var newData = new byte[dataLength];
            Buffer.BlockCopy(data, 0, newData, 0, dataLength);
            if (_peer.ContainsKey(index))
                _socket.BeginSend(data, data.Length, _peer[index], DoSend, null);
        }

        /// <summary>
        /// Send data to specified client. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
		public void SendDataTo(int index, ref byte[] data, int location)
        {
            if (_peer.ContainsKey(index))
                if (_peer[index] == null) { return; }

            var newData = new byte[location];
            Buffer.BlockCopy(data, 0, newData, 0, location);
            if (_peer.ContainsKey(index))
                _socket.BeginSend(data, location, _peer[index], DoSend, null);
        }

        /// <summary>
        /// Send data to all clients.
        /// </summary>
        public void SendDataToAll(ref byte[] data)
        {
            List<int> ids = new List<int>(_peer.Keys);

            foreach (int id in ids)
                if (_peer.ContainsKey(id))
                    SendDataTo(id, ref data);
        }

        /// <summary>
        /// Send data to all clients. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
        public void SendDataToAll(ref byte[] data, int location)
        {
            List<int> ids = new List<int>(_peer.Keys);

            foreach (int id in ids)
                if (_peer.ContainsKey(id))
                    SendDataTo(id, ref data, location);
        }

        /// <summary>
        /// Send data to all but specified client.
        /// </summary>
        public void SendDataToAllBut(int index, ref byte[] data)
        {
            List<int> ids = new List<int>(_peer.Keys);

            foreach (int id in ids)
                if (_peer.ContainsKey(id))
                    if (id != index)
                        SendDataTo(id, ref data);
        }

        /// <summary>
        /// Send data to all but specified client. Optimizes based on
        /// used packet data without unintentional empty data.
        /// </summary>
        public void SendDataToAllBut(int index, ref byte[] data, int location)
        {
            List<int> ids = new List<int>(_peer.Keys);

            foreach (int id in ids)
                if (_peer.ContainsKey(id))
                    if (id != index)
                        SendDataTo(id, ref data, location);
        }

        private void DoSend(IAsyncResult ar)
        {
            _socket.EndSend(ar);
        }
    }
}