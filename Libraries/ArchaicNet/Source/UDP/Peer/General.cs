using System.Net.Sockets;
namespace ArchaicNet.UDP
{
    public partial class Peer
    {
        /// <summary>
        /// Initializes peer with the Packet count.
        /// Proper use of packetcount would be based on an
        /// enumerator and the "Enum.Count" value would be
        /// placed here.
        /// </summary>
        public Peer(int packetCount, int hostPort)
        {
            if (_socket != null) return;
            _socket = new UdpClient(hostPort);
            PacketId = new DataArgs[packetCount];
            _socket.BeginReceive(DoReceive, null);
        }

        /// <summary>
        /// Ends the network and clears remaining data.
        /// </summary>
        public void EndNetwork()
        {
            _socket.Close();
            _socket = null;
            DisposeEvents();
        }
    }
}