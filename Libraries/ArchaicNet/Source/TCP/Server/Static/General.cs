﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ArchaicNet.TCP.Static
{
    public partial class Server
    {
        /// <summary>
        /// Initializes server with the Packet count and 
        /// player limit.
        /// Proper use of packetcount would be based on an
        /// enumerator and the "Enum.Count" value would be
        /// placed here.
        /// </summary>
        public Server(int packetCount, int playerLimit)
        {
            if (_listener != null || _socket != null)
                return;
            _socket = new Socket[playerLimit];
            _unsignedIndex = new List<int>();
            PacketId = new DataArgs[packetCount];
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
            StopListening();
            int i;
            for (i = 0; i <= _socket.Length - 1; i++)
                Disconnect(i);
            _socket = null;
            PacketId = null;
            _unsignedIndex.Clear();
            _unsignedIndex = null;
            DisposeEvents();
        }

        /// <summary>
        /// Returns if client(index) is connected. Does not
        /// update if network connection to client breaks 
        /// without proper disconnect.
        /// </summary>
        public bool IsConnected(int index)
        {
            return _socket[index].Connected;
        }

        /// <summary>
        /// Returns the IP of the connected Client.
        /// </summary>
        public string ClientIp(int index)
        {
            var ipEndpoint = (IPEndPoint)_socket[index].RemoteEndPoint;
            return ipEndpoint.Address.ToString();
        }

        /// <summary>
        /// Ends connection with Client(index) and closes socket.
        /// </summary>
        public void Disconnect(int index)
        {
            ConnectionLost?.Invoke(index);
            _socket[index].BeginDisconnect(false, DoDisconnect, index);
        }

        private void DoDisconnect(IAsyncResult ar)
        {
            var index = (int)ar.AsyncState;
            try
            {
                _socket[(int)ar.AsyncState].EndDisconnect(ar);
            }
            catch
            {
                // ignored
            }
            _socket[(int)ar.AsyncState] = null;
            _unsignedIndex.Add(index);
        }
    }
}