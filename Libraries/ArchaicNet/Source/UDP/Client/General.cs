using System.Net;
using System.Net.Sockets;
namespace ArchaicNet.UDP
{
    public partial class Client
    {
        /// <summary>
        /// Initializes client with the Packet count.
        /// Proper use of packetcount would be based on an
        /// enumerator and the "Enum.Count" value would be
        /// placed here.
        /// </summary>
        public Client(int packetCount, string serverIp, int clientPort, int serverPort)
        {
            if (_socket != null) return;
            _socket = new UdpClient(clientPort);
            _peer = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
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