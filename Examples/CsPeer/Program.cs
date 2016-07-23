using System;
using ArchaicNet;
using System.Collections.Generic;
using System.Net;

/*###############################################################*/
/*## NOTE: This application IS compatible with the VB version! ##*/
/*###############################################################*/

namespace CsClient
{
    class Program
    {
        private static ArchaicNet.UDP.Peer _udpPeer;
        private static List<int> ports = new List<int>(); // We will use this to look for other people on this computer. The example does not use multiple Ip's or we would have to keep track of IPEndPoints instead of ports.

        static void Main()
        {
            Console.WriteLine("Ready to connect! Type /start (port) to begin listening on that port, /add (port) to add a person to send to, or /end when done!");

            while (true)
            {
                string s = Console.ReadLine(); // Get input.

                if (s.StartsWith("/start "))
                {
                    int p = 0;
                    int.TryParse(s.Substring("/start ".Length), out p);
                    StartPeer(p);
                }
                else if (s.StartsWith("/add "))
                {
                    int p = 0;
                    int.TryParse(s.Substring("/add ".Length), out p);
                    ports.Add(p);
                }
                else if (s == "/end") { EndPeer(); }
                else { SendMessage(s); }
            }
        }
        #region Configuration
        static void StartPeer(int port)
        {
            _udpPeer = new ArchaicNet.UDP.Peer((int)PeerPackets.Count, ArchaicNet.Unique.GeneratePort(port));
            RoutePacket(); // Initializes the handler for incoming packets.
        }

        static void EndPeer()
        {
            ports.Clear();
            _udpPeer.EndNetwork(); // Clear Udp Data.
        }

        enum PeerPackets // Packets Sent or Received
        {
            Message,
            Count // Must remain at bottom.
        }
        #endregion

        #region Receive
        static void RoutePacket() // This will automatically handle the packets received asyncronously.
        {
            // PacketId[PacketName] = Function to call.
            _udpPeer.PacketId[(int)PeerPackets.Message] = ReceiveMessage;
        }

        static void ReceiveMessage(IPEndPoint ep, ref byte[] data)
        {
            Message msg = new Message(data); // Unpacks byte array and prepairs to read.
            Console.WriteLine(msg.ReadString); // Reads the string from the message. If multiple data is present, data must be read in order!
            msg.Dispose(); // Cleanup.
        }
        #endregion

        #region Send
        static void SendMessage(string s)
        {
            Message msg = new Message(4 + s.Length + 4); // Presize for optimization - Size of packet name + Size of message + size of integer that stores the length.
            msg.Write((int)PeerPackets.Message); // Must send the name FIRST! This is autoextracted in the server.
            msg.Write(s); // Already knows how to pack a string!

            // We only care to send to whoever we have added to our port list, so well send to everyone!
            foreach (int p in ports)
                _udpPeer.SendData(new IPEndPoint(IPAddress.Parse("127.0.0.1"), p), ref msg.Data, msg.Location); // Sends the packet data from our message. Included sending the location for optimization!
            msg.Dispose(); // Cleanup.
        }
        #endregion
    }
}
