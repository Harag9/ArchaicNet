using System;
using System.Net;
using System.Net.Sockets;

namespace ArchaicNet.TCP
{
    public partial class Server
    {
        /// <summary>
        /// Starts normal server process. Used directly after
        /// server constructor to begin server activity.
        /// </summary>
        public void StartListening(int port)
        {
            if (IsListening || _socket == null)
                return;
            _listener = new TcpListener(IPAddress.Any, port);
            IsListening = true;
            _listener.Start();
            _listener.BeginAcceptTcpClient(DoAcceptClient, null);
        }

        /// <summary>
        /// Used when server no longer needs to collect more
        /// clients but still needs to continue processing
        /// active connections.
        /// </summary>
        public void StopListening()
        {
            if (!IsListening || _socket == null)
                return;
            IsListening = false;
            _listener.Stop();
            _listener.Server.Dispose();
            _listener = null;
        }

        private void DoAcceptClient(IAsyncResult asyncResult)
        {
            var client = _listener.EndAcceptTcpClient(asyncResult).Client;
            var index = FindEmptyPlayerSlot;
            _socket.Add(index, client);
            _socket[index].ReceiveBufferSize = _receiveBufferSize;
            BeginReceiveData(index);
            ConnectionReceived?.Invoke(index);
            if (IsListening)
                _listener.BeginAcceptTcpClient(DoAcceptClient, null);
        }
    }
}