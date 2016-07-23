using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ArchaicNet.UDP
{
    public partial class Server
    {
        /// <summary>
        /// Initializes server with the Packet count.
        /// Proper use of packetcount would be based on an
        /// enumerator and the "Enum.Count" value would be
        /// placed here.
        /// </summary>
        public Server(int packetCount, int port)
        {
            if (_socket != null) return;
            _socket = new UdpClient(port);
            _peer = new Dictionary<int, IPEndPoint>();
            PacketId = new DataArgs[packetCount];
            _socket.BeginReceive(DoReceive, null);
        }

        /// <summary>
        /// Creates and adds an IpEndPoint tied to an index to
        /// send/receive data from on the network.
        /// </summary>
        public void AddEntry(int index, string ip, int port)
        {
            if (_peer.ContainsKey(index)) return;
            _peer.Add(index, new IPEndPoint(IPAddress.Parse(ip), port));
        }

        /// <summary>
        /// Removes an index from the list of IpEndPoints valid to
        /// send/receive from.
        /// </summary>
        public void RemoveEntry(int index)
        {
            if (!_peer.ContainsKey(index)) return;
            _peer.Remove(index);
        }

        /// <summary>
        /// Returns if server can send/receive from the index provided.
        /// 
        /// Warning: If entry has not been properly handled may return
        /// a false value such as working with an old or incorrect
        /// IpEndPoint.
        /// </summary>
        public bool ContainsEntry(int index)
        {
            return _peer.ContainsKey(index);
        }

        /// <summary>
        /// Returns internal IP server's router uses to identify the
        /// servers computer on the network.
        /// </summary>
        public string GetIPv4()
        {
            var host = Dns.GetHostName();
            return Dns.GetHostEntry(host).AddressList[0].ToString();
        }

        /// <summary>
        /// Ends the network and clears remaining data.
        /// </summary>
        public void EndNetwork()
        {
            _socket.Close();
            _socket = null;
            PacketId = null;
            DisposeEvents();
        }

        /// <summary>
        /// Returns the IP of the connected Client.
        /// </summary>
        public string ClientIp(int index)
        {
            return _peer[index].Address.ToString();
        }
    }
}