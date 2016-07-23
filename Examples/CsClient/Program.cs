using System;
using ArchaicNet;

/*###############################################################*/
/*## NOTE: This application IS compatible with the VB version! ##*/
/*###############################################################*/

namespace CsClient
{
    class Program
    {
        private static ArchaicNet.TCP.Client _tcpClient;
        private static ArchaicNet.UDP.Client _udpClient;
        private static bool UdpMode = false;

        static void Main()
        {
            StartTcpClient();

            Console.WriteLine("Ready to connect! Type /connect when ready, /tcp or /udp to change network mode or /end when done!");

            while (true)
            {
                string s = Console.ReadLine(); // Get input.

                switch (s)
                {
                    case "/connect":
                        ConnectToServer();
                        break;
                    case "/end":
                        EndClient();
                        return;
                    case "/tcp":
                        UdpMode = false;
                        Console.WriteLine("Entered Tcp Mode.");
                        break;
                    case "/udp":
                        if (_udpClient == null)
                        {
                            var udpPort = StartUdpClient(5002);
                            SendUdpPort(udpPort);
                            UdpMode = true;
                            Console.WriteLine("Entered Udp Mode.");
                        }
                        break;
                    default:
                        SendMessage(s);
                        break;
                }
            }
        }
        #region Configuration
        static void StartTcpClient()
        {
            _tcpClient = new ArchaicNet.TCP.Client((int)ServerPackets.Count); // Initializes socket data and tells it how many different types of packets can be received.
            RoutePacket(); // Initializes the handler for incoming packets.
        }

        static int StartUdpClient(int port) // Returns int so we can tell server what port we are on.
        {
            try { _udpClient = new ArchaicNet.UDP.Client(0, "127.0.0.1", port, 5001); } // In this case, server doesnt send anything UDP although it can.
            catch { return StartUdpClient(port + 1); }                                  // So we dont put a count or initialize UDP based server packet handeling.
            return port;
        }

        static void ConnectToServer()
        {
            if (_tcpClient.IsConnected) return; // No need to connect if already connected!
            _tcpClient.Connect("127.0.0.1", 5000);
        }

        static void EndClient()
        {
            _tcpClient.EndNetwork(); // Auto disconnects and disposes all socket data.
            _udpClient.EndNetwork(); // Clear Udp Data.
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
        #endregion

        #region Receive
        static void RoutePacket() // This will automatically handle the packets received asyncronously.
        {
            // PacketId[PacketName] = Function to call.
            _tcpClient.PacketId[(int)ServerPackets.MessageRelay] = ReceiveMessage;
        }

        static void ReceiveMessage(ref byte[] data)
        {
            Message msg = new Message(data); // Unpacks byte array and prepairs to read.
            Console.WriteLine(msg.ReadString); // Reads the string from the message. If multiple data is present, data must be read in order!
            msg.Dispose(); // Cleanup.
        }
        #endregion

        #region Send
        static void SendMessage(string s)
        {
            /* V THIS IS TO MAKE SURE IT TELLS WHAT MODE YOU ARE IN! V */

            if (UdpMode)
                s = "(=UDP=) " + s;
            else
                s = "(=TCP=) " + s;

            /* ^ THIS IS TO MAKE SURE IT TELLS WHAT MODE YOU ARE IN! ^ */

            Message msg = new Message(4 + s.Length + 4); // Presize for optimization - Size of packet name + Size of message + size of integer that stores the length.
            msg.Write((int)ClientPackets.Message); // Must send the name FIRST! This is autoextracted in the server.
            msg.Write(s); // Already knows how to pack a string!
            if (UdpMode)
                _udpClient.SendData(ref msg.Data, msg.Location); // Sends the packet data from our message. Included sending the location for optimization!
            else
                _tcpClient.SendData(ref msg.Data, msg.Location); // Sends the packet data from our message. Included sending the location for optimization!
            msg.Dispose(); // Cleanup.
        }

        static void SendUdpPort(int port)
        {
            Message msg = new Message(8); // Presize for optimization - Size of packet name + Size of integer.
            msg.Write((int)ClientPackets.UdpPort);
            msg.Write(port);
            _tcpClient.SendData(ref msg.Data, msg.Location); // Sends the packet data from our message. Included sending the location for optimization!
            msg.Dispose();
        }
        #endregion
    }
}
