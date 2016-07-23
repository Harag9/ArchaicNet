using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ArchaicNet.UDP
{
    public partial class Server
    {
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

                var index = GetIndexFromEndPoint(ep);
                if (index == -1)
                {
                    _socket.BeginReceive(DoReceive, null);
                    return;
                }

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
                    PacketId[packetName].Invoke(index, ref packet);
                    _socket.BeginReceive(DoReceive, null);
                    return;
                }
                else if (recSize == 2)
                {
                    SendReturnPing(index);
                    _socket.BeginReceive(DoReceive, null);
                    return;
                }
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

        private int GetIndexFromEndPoint(IPEndPoint ep)
        {
            int index = -1;
            if (_peer.ContainsValue(ep))
            {
                List<int> ids = new List<int>(_peer.Keys);
                foreach (int id in ids)
                    if (_peer.ContainsKey(id))
                    {
                        try
                        {
                            var tmpPeer = _peer[id].Address.ToString();
                            var tmpEp = ep.Address.ToString();
                            if (_peer[id].Address.ToString() == ep.Address.ToString() && _peer[id].Port == ep.Port)
                            {
                                index = id;
                                break;
                            }
                        }
                        catch { }
                    }
            }
            return index;
        }
    }
}