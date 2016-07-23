using System;
using System.Net.Sockets;

namespace ArchaicNet.TCP
{
    public partial class Server
    {
        private void BeginReceiveData(int index)
        {
            var so = new ReceiveState
            {
                Index = index,
                Buffer = new byte[_receiveBufferSize]
            };
            _socket[index].BeginReceive(so.Buffer, 0, _receiveBufferSize,
                SocketFlags.None, DoReceive, so);
        }

        private void DoReceive(IAsyncResult ar)
        {
            var so = (ReceiveState)ar.AsyncState;
            try
            {
                var size = _socket[so.Index].EndReceive(ar);
                if (size < 1)
                {
                    CrashReport?.Invoke(so.Index, "BufferUnderflowException");
                    Disconnect(so.Index);
                    return;
                }
                if (size > 3)
                {
                    if (so.PacketSize == 0)
                        so.PacketSize = BitConverter.ToInt32(so.Buffer, 0);
                    if (so.PacketSize < size)
                    {
                        var tempSize = size;
                        if (so.TempPacket == null)
                            so.TempPacket = new byte[so.PacketSize + 1];
                        else
                        {
                            if (so.ReceivedSize + size > so.PacketSize)
                                tempSize = so.PacketSize - so.ReceivedSize;
                        }
                        if (so.ReceivedSize == 0)
                        {
                            Buffer.BlockCopy(so.Buffer, 4, so.TempPacket, 0, tempSize - 4);
                            so.ReceivedSize += tempSize - 4;
                        }
                        else
                        {
                            Buffer.BlockCopy(so.Buffer, 0, so.TempPacket, so.ReceivedSize, tempSize);
                            so.ReceivedSize += tempSize;
                        }
                        if (so.PacketSize == so.ReceivedSize)
                        {
                            var packetName = BitConverter.ToInt32(so.TempPacket, 0);
                            var packet = new byte[so.PacketSize - 4];
                            Buffer.BlockCopy(so.TempPacket, 4, packet, 0, so.PacketSize - 4);
                            PacketId[packetName].Invoke(so.Index, ref packet);
                            so.TempPacket = null;
                            so.ReceivedSize = 0;
                            so.PacketSize = 0;
                        }
                        else
                        {
                            so.Buffer = new byte[_receiveBufferSize];
                            _socket[so.Index].BeginReceive(so.Buffer, 0, _socket[so.Index].ReceiveBufferSize,
                                SocketFlags.None,
                                DoReceive, so);
                            return;
                        }
                    }
                    else
                    {
                        var packetName = BitConverter.ToInt32(so.Buffer, 4);
                        var packet = new byte[so.PacketSize - 4];
                        Buffer.BlockCopy(so.Buffer, 8, packet, 0, so.PacketSize - 4);
                        PacketId[packetName].Invoke(so.Index, ref packet);
                        PacketReceived?.Invoke();
                    }
                }
                else if (size == 2) SendReturnPing(so.Index);

                so.Buffer = new byte[_receiveBufferSize];
                _socket[so.Index].BeginReceive(so.Buffer, 0, _socket[so.Index].ReceiveBufferSize,
                    SocketFlags.None, DoReceive, so);
            }
            catch (IndexOutOfRangeException)
            {
                CrashReport?.Invoke(so.Index, "IndexOutOfRangeException");
                Disconnect(so.Index);
                return;
            }
            catch (NullReferenceException)
            {
                CrashReport?.Invoke(so.Index, "NullReferenceException");
                Disconnect(so.Index);
                return;
            }
            catch (SocketException)
            {
                CrashReport?.Invoke(so.Index, "SocketException");
                Disconnect(so.Index);
                return;
            }
            catch (Exception)
            {
                CrashReport?.Invoke(so.Index, "UnknownException");
                Disconnect(so.Index);
                return;
            }
        }

        private struct ReceiveState
        {
            public int Index;
            public byte[] Buffer;
            public int PacketSize;
            public int ReceivedSize;
            public byte[] TempPacket;
        }
    }
}