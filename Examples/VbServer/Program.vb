Imports ArchaicNet

'###############################################################
'## NOTE: This application IS compatible with the C# version! ##
'###############################################################

Module Program

    Private WithEvents _tcpServer As ArchaicNet.TCP.Server ' Has Events!!!
    Private WithEvents _udpServer As ArchaicNet.UDP.Server

    Sub Main()
        StartServer()

        Console.WriteLine("Server has begun successfully. Clients may now connect!")

        While True
            Dim s As String = Console.ReadLine()

            If s = "/end" Then
                Exit While
            End If
        End While
        EndServer()
    End Sub

#Region "Configuration"
    Sub StartServer()
        _tcpServer = New ArchaicNet.TCP.Server(CInt(ClientPackets.Count)) ' Initializes socket data and tells it how many different types of packets can be received.
        _udpServer = New ArchaicNet.UDP.Server(CInt(ClientPackets.Count), 5001) ' Transaction Port.

        RoutePacket() ' Initializes the handler for incoming packets.
        _tcpServer.StartListening(5000) ' Server is now accepting connections and running! Nothing more to do until people are on!
    End Sub

    Sub EndServer()
        _tcpServer.EndNetwork() ' Auto disconnects all connections and disposes all socket data.
        _udpServer.EndNetwork() ' Clear Udp Data.
    End Sub

    Enum ClientPackets ' Packets Sent by Client to the Server
        Message
        UdpPort
        Count ' Must remain at bottom.
    End Enum

    Enum ServerPackets ' Packets Sent by Server to the Client
        MessageRelay
        Count ' Must remain at bottom.
    End Enum

    Sub ConnectionReceived(index As Integer) Handles _tcpServer.ConnectionReceived ' Asyncronously handles a received connection!
        Console.WriteLine("Receieved Connection from: " & _tcpServer.ClientIp(index))
    End Sub

    Sub ConnectionLost(index As Integer) Handles _tcpServer.ConnectionLost ' Asyncronously handles a lost connection!
        _udpServer.RemoveEntry(index)
        Console.WriteLine("Receieved Lost from: " & _tcpServer.ClientIp(index))
    End Sub
#End Region

#Region "Receive"
    Sub RoutePacket() ' This will automatically handle the packets received asyncronously.
        ' PacketId(PacketName) = AddressOf Sub to call.
        _tcpServer.PacketId(ClientPackets.Message) = AddressOf ReceiveMessage
        _tcpServer.PacketId(ClientPackets.UdpPort) = AddressOf ReceiveUdpPort

        _udpServer.PacketId(ClientPackets.Message) = AddressOf ReceiveMessage
    End Sub

    Sub ReceiveMessage(index As Integer, ByRef data As Byte())
        ' In here we take our packet data and send it right back to EVERYONE connected!
        Dim msg As Message = New Message(data)
        ' Now we have to reattach a new name, server autoripped the name off so only our message was left!
        Dim s = msg.ReadString ' Grab the message sent by client
        Console.WriteLine(s)
        Dim msgToSend As Message = New Message(4 + "Server:".Length + s.Length) ' Size of Packet Name + our data to relay.
        msgToSend.Write(ServerPackets.MessageRelay) ' Message Name
        msgToSend.Write("Server:" & s) ' Reading the "message" from our msg
        _tcpServer.SendDataToAll(msgToSend.Data, msgToSend.Location) ' Send to everyone connected.
        msg.Dispose() ' Cleanup.
        msgToSend.Dispose() ' Cleanup.
    End Sub

    Sub ReceiveUdpPort(index As Integer, ByRef data As Byte())
        Dim msg As Message = New Message(data)
        Dim port As Integer = msg.ReadInteger
        _udpServer.AddEntry(index, _tcpServer.ClientIp(index), port)
        msg.Dispose()
    End Sub
#End Region

End Module
