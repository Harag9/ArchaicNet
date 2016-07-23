using System.Collections.Generic;
using System.Net.Sockets;

namespace ArchaicNet.TCP
{
    /// <summary>
    /// Object Oriented server that handles all background
    /// processes for you. This version is used with a dictionary
    /// storing sockets dynamicly.
    /// </summary>
    public partial class Server
    {
        private TcpListener _listener;
        private int _receiveBufferSize = 8192;
        private Dictionary<int, Socket> _socket;
        private List<int> _unsignedIndex;

        /// <summary>
        /// Checks if server is currently listening for clients.
        /// </summary>
        public bool IsListening { get; private set; }

        /// <summary>
        /// Returns highest "used" index.
        /// </summary>
        public int HighIndex { get; private set; }

        /// <summary>
        /// Gets or sets the buffer receive size.
        /// 
        /// Only settable when no server has no active connections.
        /// </summary>
        public int ReceiveBufferSize
        {
            get { return _receiveBufferSize; }
            set { if (!IsListening && HighIndex == 0) _receiveBufferSize = value; }
        }

        /// <summary>
        /// Finds next empty/unused index.
        /// </summary>
        public int FindEmptyPlayerSlot
        {
            get
            {
                var slot = 0;
                for (var b = _unsignedIndex.Count - 1; b > 0; b--)
                {
                    if (HighIndex == _unsignedIndex[b])
                        HighIndex--;
                    else
                        break;
                }
                if (_unsignedIndex.Count > 0)
                    foreach (var i in _unsignedIndex)
                    {
                        slot = i;
                        _unsignedIndex.Remove(i);
                        break;
                    }
                else
                {
                    if (_socket.Count > 0)
                        HighIndex++;
                    slot = HighIndex;
                }
                return slot;
            }
        }

        #region Events

        public delegate void ConnectionArgs(int index);

        public delegate void DataArgs(int index, ref byte[] data);

        public delegate void CrashReportArgs(int index, string reason);

        public delegate void NullArgs();

        /// <summary>
        /// Allows for handeling events when a connection is received.
        /// The newly connected sockets' index is returned.
        /// </summary>
        public event ConnectionArgs ConnectionReceived;

        /// <summary>
        /// Allows for handeling events when a connection is lost.
        /// The old connected sockets' index is returned.
        /// </summary>
        public event ConnectionArgs ConnectionLost;

        /// <summary>
        /// Allows for handling events when a connection is lost due
        /// to some type of error.
        /// </summary>
        public event CrashReportArgs CrashReport;

        /// <summary>
        /// Allows for handeling events when a packet is received.
        /// This has benefits for checking traffic.
        /// </summary>
        public event NullArgs PacketReceived;

        /// <summary>
        /// Should be used with an initializer sub.
        /// Example: (Cs)Server.PacketId[(int)Enum.PacketName] = Function
        /// Example: (VB)Server.PacketId(PacketName) = AddressOf Sub
        /// </summary>
        public DataArgs[] PacketId;

        protected virtual void OnConnectionReceived(int index)
        {
            ConnectionReceived?.Invoke(index);
        }

        protected virtual void OnConnectionLost(int index)
        {
            ConnectionLost?.Invoke(index);
        }

        protected virtual void OnConnectionCrash(int index, string reason)
        {
            CrashReport?.Invoke(index, reason);
        }

        protected virtual void OnPacketReceived()
        {
            PacketReceived?.Invoke();
        }

        private void DisposeEvents()
        {
            ConnectionReceived = null;
            ConnectionLost = null;
            PacketReceived = null;
            PacketId = null;
        }

        #endregion
    }
}