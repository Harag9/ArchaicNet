using System;
using ArchaicNet;

/*###############################################################*/
/*## NOTE: This application IS compatible with the VB version! ##*/
/*###############################################################*/

namespace CsServer
{
    class Program
    {
        private static ArchaicNet.TCP.Server _tcpServer; // Has Events!
        private static ArchaicNet.UDP.Server _udpServer;

        static void Main()
        {
            StartServer();

            Console.WriteLine("Server has begun successfully. Clients may now connect!");

            while (true)
            {
                string s = Console.ReadLine();

                if (s == "/end") break;
            }
            EndServer();
        }

        #region Configuration
        static void StartServer()
        {
            _tcpServer = new ArchaicNet.TCP.Server((int)ClientPackets.Count); // Initializes socket data and tells it how many different types of packets can be received.
            _udpServer = new ArchaicNet.UDP.Server((int)ClientPackets.Count, 5001); // Transaction Port.

            _tcpServer.ConnectionReceived += ConnectionReceived;
            _tcpServer.ConnectionLost += ConnectionLost;

            RoutePacket(); // Initializes the handler for incoming TCP packets.
            _tcpServer.StartListening(5000); // Server is now accepting connections and running! Nothing more to do until people are on!
        }

        static void EndServer()
        {
            _tcpServer.EndNetwork(); // Auto disconnects all connections and disposes all socket data.
            _udpServer.EndNetwork(); // Clear Udp Data.
        }

        enum ClientPackets // Packets Sent by Client to the Server
        {
            Message,
            UdpPort,
            Count // Must remain at bottom.
        }

        enum ServerPackets // Packets Sent by Server to the Client
        {
            MessageRelay,
            Count // Must remain at bottom.
        }
        
        static void ConnectionReceived(int index) // Asyncronously handles a received connection!
        {
            Console.WriteLine("Receieved Connection from: " + _tcpServer.ClientIp(index));
        }

        static void ConnectionLost(int index) // Asyncronously handles a lost connection!
        {
            _udpServer.RemoveEntry(index);
            Console.WriteLine("Receieved Lost from: " + _tcpServer.ClientIp(index));
        }
        #endregion

        #region Receive
        static void RoutePacket() // This will automatically handle the packets received asyncronously.
        {
            // PacketId[PacketName] = Function to call.
            _tcpServer.PacketId[(int)ClientPackets.Message] = ReceiveMessage;
            _tcpServer.PacketId[(int)ClientPackets.UdpPort] = ReceiveUdpPort;

            _udpServer.PacketId[(int)ClientPackets.Message] = ReceiveMessage;
        }

        static void ReceiveMessage(int index, ref byte[] data)
        {
            // In here we take our packet data and send it right back to EVERYONE connected!
            Message msg = new Message(data);
            // Now we have to reattach a new name, server autoripped the name off so only our message was left!
            string s = msg.ReadString; // Grab the message sent by client
            Console.WriteLine(s);
            Message msgToSend = new Message(4 + "Server:".Length + s.Length); // Size of Packet Name + our data to relay.
            msgToSend.Write((int)ServerPackets.MessageRelay); // Message Name
            msgToSend.Write("Server:" + s); // Reading the "message" from our msg
            _tcpServer.SendDataToAll(ref msgToSend.Data, msgToSend.Location); // Send to everyone connected.
            msg.Dispose(); // Cleanup.
            msgToSend.Dispose(); // Cleanup.
        }

        static void ReceiveUdpPort(int index, ref byte[] data)
        {
            Message msg = new Message(data);
            int port = msg.ReadInteger;
            _udpServer.AddEntry(index, _tcpServer.ClientIp(index), port);
            msg.Dispose();
        }
        #endregion
    }
}
