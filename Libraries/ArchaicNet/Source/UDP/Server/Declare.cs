using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ArchaicNet.UDP
{
    /// <summary>
    /// Object Oriented server that handles all background
    /// processes for you.
    /// 
    /// This version is handled with UDP and not intended to be
    /// used standalone.
    /// </summary>
    public partial class Server
    {
        private UdpClient _socket;
        private Dictionary<int, IPEndPoint> _peer;
        
        #region Events
        public delegate void DataArgs(int index, ref byte[] data);
        
        public delegate void NullArgs();

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

        protected virtual void OnPacketReceived()
        {
            PacketReceived?.Invoke();
        }

        private void DisposeEvents()
        {
            PacketReceived = null;
            PacketId = null;
        }

        #endregion
    }
}