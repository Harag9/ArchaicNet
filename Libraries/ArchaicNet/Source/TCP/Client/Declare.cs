using System.Net.Sockets;
namespace ArchaicNet.TCP
{
    /// <summary>
    /// Object Oriented client that handles all background
    /// processes for you.
    /// </summary>
    public partial class Client
    {
        private int _packetSize;
        private int _pingTime;
        private byte[] _receiveBuffer;
        private int _receiveBufferSize = 8192;
        private int _receivedSize;
        private Socket _socket;
        private byte[] _tempPacket;

        /// <summary>
        /// Gets or sets the buffer receive size.
        /// 
        /// Only settable when not connected to a server.
        /// </summary>
        public int ReceiveBufferSize
        {
            get { return _receiveBufferSize; }
            set { if (!_socket.Connected) _receiveBufferSize = value; }
        }

        #region Events

        public delegate void ConnectionArgs();

        public delegate void DataArgs(ref byte[] data);

        public delegate void CrashReportArgs(string reason);

        public delegate void PingArgs(int pingTime);

        /// <summary>
        /// Allows for handeling events when connection to a server is
        /// successful.
        /// </summary>
        public event ConnectionArgs ConnectionSuccess;

        /// <summary>
        /// Allows for handeling events when connection to a server
        /// has failed.
        /// </summary>
        public event ConnectionArgs ConnectionFailed;

        /// <summary>
        /// Allows for handeling events when connection to a server is
        /// lost.
        /// </summary>
        public event ConnectionArgs ConnectionLost;

        /// <summary>
        /// Allows for handling events when a connection is lost due
        /// to some type of error.
        /// </summary>
        public event CrashReportArgs CrashReport;

        /// <summary>
        /// Allows for handeling events when server has returned
        /// a ping response.
        /// (Intended use in coordination with SendPing for
        /// testing latency)
        /// </summary>
        public event PingArgs PingReceived;

        /// <summary>
        /// Should be used with an initializer sub.
        /// Example: (Cs)Client.PacketId[(int)Enum.PacketName] = Function
        /// Example: (VB)Client.PacketId(PacketName) = AddressOf Sub
        /// </summary>
        public DataArgs[] PacketId;

        protected virtual void OnConnectionSuccess()
        {
            ConnectionSuccess?.Invoke();
        }

        protected virtual void OnConnectionFailed()
        {
            ConnectionFailed?.Invoke();
        }

        protected virtual void OnConnectionLost()
        {
            ConnectionLost?.Invoke();
        }

        protected virtual void OnConnectionCrash(string reason)
        {
            CrashReport?.Invoke(reason);
        }

        protected virtual void OnPingReceived(int pingtime)
        {
            PingReceived?.Invoke(pingtime);
        }

        private void DisposeEvents()
        {
            ConnectionSuccess = null;
            ConnectionFailed = null;
            ConnectionLost = null;
            PacketId = null;
            PingReceived = null;
        }

        #endregion
    }
}