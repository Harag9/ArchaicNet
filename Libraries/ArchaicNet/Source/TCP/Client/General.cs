using System;
using System.Net;
using System.Net.Sockets;
namespace ArchaicNet.TCP
{
    public partial class Client
    {
        /// <summary>
        /// Initializes client with the Packet count.
        /// Proper use of packetcount would be based on an
        /// enumerator and the "Enum.Count" value would be
        /// placed here.
        /// </summary>
        public Client(int packetCount)
        {
            if (_socket != null)
                return;
            _socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            PacketId = new DataArgs[packetCount];
        }

        /// <summary>
        /// Ends the network and clears remaining data.
        /// </summary>
        public void EndNetwork()
        {
            Disconnect();
            _socket = null;
            PacketId = null;
            DisposeEvents();
        }

        /// <summary>
        /// Attempts to connect to server asynchronously.
        /// </summary>
        public void Connect(string ip, int port)
        {
            if (_socket == null)
                return;
            if (_socket.Connected)
                return;
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port),
                DoConnect, null);
        }

        private void DoConnect(IAsyncResult ar)
        {
            try
            {
                _socket.EndConnect(ar);
                if (!_socket.Connected) return;
                ConnectionSuccess?.Invoke();
                _socket.ReceiveBufferSize = _receiveBufferSize;
                BeginReceiveData();
            }
            catch
            {
                ConnectionFailed?.Invoke();
            }
        }

        /// <summary>
        /// Returns if client is connected to a server. Does not
        /// update if network connection to server breaks 
        /// without proper disconnect.
        /// </summary>
        public bool IsConnected => _socket != null && _socket.Connected;

        /// <summary>
        /// Ends connection with Server and closes socket.
        /// </summary>
        public void Disconnect()
        {
            if (_socket == null) return;
            if (_socket.Connected)
                _socket.BeginDisconnect(false, DoDisconnect, null);
        }

        private void DoDisconnect(IAsyncResult ar)
        {
            try
            {
                _socket.EndDisconnect(ar);
            }
            catch
            {
                // ignored
            }
            ConnectionLost?.Invoke();
        }
    }
}