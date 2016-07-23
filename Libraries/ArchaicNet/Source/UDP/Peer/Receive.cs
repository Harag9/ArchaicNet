using System;
using System.Net;
using System.Net.Sockets;
namespace ArchaicNet.UDP
{
    public partial class Peer
    {
        private void ReceivedPing()
        {
            var pingTime = Environment.TickCount - _pingTime;
            _pingTime = 0;
            PingReceived?.Invoke(pingTime);
        }

        private void DoReceive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ep = null;

                var rec = _socket.EndReceive(ar, ref ep);
                if (ep == null)
                {
                    _socket.BeginReceive(DoReceive, null);
                    return;
                }

                var recSize = rec.Length;
                if (recSize < 1)
                {
                    _socket.BeginReceive(DoReceive, null);
                    return;
                }
                if (recSize > 3)
                {
                    var packetName = BitConverter.ToInt32(rec, 0);
                    var packet = new byte[recSize - 4];
                    Buffer.BlockCopy(rec, 4, packet, 0, recSize - 4);
                    PacketId[packetName].Invoke(ep, ref packet);
                    _socket.BeginReceive(DoReceive, null);
                    return;
                }
                else if (recSize == 2) SendReturnPing(ep);
                else if (recSize == 1) ReceivedPing();

                _socket.BeginReceive(DoReceive, null);
                return;
            }
            catch (IndexOutOfRangeException)
            {
                _socket.BeginReceive(DoReceive, null);
                return;
            }
            catch (NullReferenceException)
            {
                _socket.BeginReceive(DoReceive, null);
                return;
            }
            catch (SocketException)
            {
                _socket.BeginReceive(DoReceive, null);
                return;
            }
            catch (Exception)
            {
                _socket.BeginReceive(DoReceive, null);
                return;
            }
        }
    }
}