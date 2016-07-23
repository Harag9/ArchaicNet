using System.Net;
using System.Net.Sockets;
namespace ArchaicNet.UDP
{
    /// <summary>
    /// Object Oriented Peer that handles all background
    /// processes for you.
    /// 
    /// This version is handled with UDP and is intended to be
    /// used standalone.
    /// </summary>
    public partial class Peer
    {
        private int _pingTime;
        private UdpClient _socket;

        #region Events

        public delegate void ConnectionArgs();

        public delegate void DataArgs(IPEndPoint ep, ref byte[] data);

        public delegate void PingArgs(int pingTime);

        /// <summary>
        /// Allows for handeling events when peer has returned
        /// a ping response.
        /// (Intended use in coordination with SendPing for
        /// testing latency)
        /// </summary>
        public event PingArgs PingReceived;

        /// <summary>
        /// Should be used with an initializer sub.
        /// Example: (Cs)Peer.PacketId[(int)Enum.PacketName] = Function
        /// Example: (VB)Peer.PacketId(PacketName) = AddressOf Sub
        /// </summary>
        public DataArgs[] PacketId;

        protected virtual void OnPingReceived(int pingtime)
        {
            PingReceived?.Invoke(pingtime);
        }

        private void DisposeEvents()
        {
            PacketId = null;
            PingReceived = null;
        }

        #endregion
    }
}