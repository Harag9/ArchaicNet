using System.Net;
using System.Net.Sockets;
namespace ArchaicNet.UDP
{
    /// <summary>
    /// Object Oriented client that handles all background
    /// processes for you.
    /// 
    /// This version is handled with UDP and not intended to be
    /// used standalone.
    /// </summary>
    public partial class Client
    {
        private int _pingTime;
        private UdpClient _socket;
        private IPEndPoint _peer;

        #region Events

        public delegate void ConnectionArgs();

        public delegate void DataArgs(ref byte[] data);

        public delegate void PingArgs(int pingTime);

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