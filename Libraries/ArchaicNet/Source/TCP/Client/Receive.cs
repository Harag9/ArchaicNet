using System;
using System.Net.Sockets;
namespace ArchaicNet.TCP
{
    public partial class Client
    {
        private void ReceivedPing()
        {
            var pingTime = Environment.TickCount - _pingTime;
            _pingTime = 0;
            PingReceived?.Invoke(pingTime);
        }

        private void BeginReceiveData()
        {
            _receiveBuffer = new byte[_receiveBufferSize];
            _socket.BeginReceive(_receiveBuffer, 0, _socket.ReceiveBufferSize,
                SocketFlags.None, DoReceive, null);
        }

        private void DoReceive(IAsyncResult ar)
        {
            try
            {
                var size = _socket.EndReceive(ar);
                if (size < 1)
                {
                    CrashReport?.Invoke("BufferUnderflowException");
                    Disconnect();
                }
                if (size > 3)
                {
                    if (_packetSize == 0)
                        _packetSize = BitConverter.ToInt32(_receiveBuffer, 0);
                    if (_packetSize > size)
                    {
                        var tempSize = size;
                        if (_tempPacket == null)
                            _tempPacket = new byte[_packetSize + 1];
                        else
                        {
                            if (_receivedSize + size > _packetSize)
                            {
                                tempSize = _packetSize - _receivedSize;
                            }
                        }
                        if (_receivedSize == 0)
                        {
                            Buffer.BlockCopy(_receiveBuffer, 4, _tempPacket, 0, tempSize - 4);
                            _receivedSize += tempSize - 4;
                        }
                        else
                        {
                            Buffer.BlockCopy(_receiveBuffer, 0, _tempPacket, _receivedSize, tempSize);
                            _receivedSize += tempSize;
                        }
                        if (_packetSize == _receivedSize)
                        {
                            var packetName = BitConverter.ToInt32(_tempPacket, 0);
                            var packet = new byte[_packetSize - 4];
                            Buffer.BlockCopy(_tempPacket, 4, packet, 0, _packetSize - 4);
                            PacketId[packetName].Invoke(ref packet);
                            _tempPacket = null;
                            _receivedSize = 0;
                            _packetSize = 0;
                        }
                        else
                        {
                            _receiveBuffer = new byte[_receiveBufferSize];
                            _socket.BeginReceive(_receiveBuffer, 0, _socket.ReceiveBufferSize, SocketFlags.None,
                                DoReceive, null);
                            return;
                        }
                    }
                    else
                    {
                        var packetName = BitConverter.ToInt32(_receiveBuffer, 4);
                        var packet = new byte[_packetSize - 4];
                        Buffer.BlockCopy(_receiveBuffer, 8, packet, 0, _packetSize - 4);
                        PacketId[packetName].Invoke(ref packet);
                        _packetSize = 0;
                    }
                }
                else if (size == 1) ReceivedPing();

                _receiveBuffer = new byte[_receiveBufferSize];
                _socket.BeginReceive(_receiveBuffer, 0, _socket.ReceiveBufferSize, SocketFlags.None,
                    DoReceive, null);
            }
            catch (IndexOutOfRangeException)
            {
                CrashReport?.Invoke("IndexOutOfRangeException");
                Disconnect();
                return;
            }
            catch (NullReferenceException)
            {
                CrashReport?.Invoke("NullReferenceException");
                Disconnect();
                return;
            }
            catch (SocketException)
            {
                CrashReport?.Invoke("SocketException");
                Disconnect();
                return;
            }
            catch (Exception)
            {
                CrashReport?.Invoke("UnknownException");
                Disconnect();
                return;
            }
        }
    }
}