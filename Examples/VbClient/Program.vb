Imports ArchaicNet

'###############################################################
'## NOTE: This application IS compatible with the C# version! ##
'###############################################################

Module Program

    Private _tcpClient As ArchaicNet.TCP.Client
    Private _udpClient As ArchaicNet.UDP.Client
    Private UdpMode As Boolean = False

    Sub Main()
        StartClient()

        Console.WriteLine("Ready to connect! Type /connect when ready, /tcp or /udp to change network mode or /end when done!")

        While True
            Dim s As String = Console.ReadLine() ' Get input.

            Select Case s
                Case "/connect"
                    ConnectToServer()
                    Exit Select
                Case "/end"
                    EndClient()
                    Return
                Case "/tcp"
                    UdpMode = False
                    Console.WriteLine("Entered Tcp Mode.")
                    Exit Select
                Case "/udp"
                    If (_udpClient Is Nothing) Then
                        Dim udpPort As Integer = StartUdpClient(5002)
                        UdpMode = True
                        Console.WriteLine("Entered Udp Mode.")
                    End If
                    Exit Select
                Case Else
                    SendMessage(s)
                    Exit Select
            End Select
        End While
    End Sub
#Region "Configuration"
    Sub StartClient()
        _tcpClient = New ArchaicNet.TCP.Client(CInt(ServerPackets.Count)) ' Initializes socket data and tells it how many different types of packets can be received.
        RoutePacket() ' Initializes the handler for incoming packets.
    End Sub

    Function StartUdpClient(ByVal port As Integer) As Integer
        Try
            _udpClient = New ArchaicNet.UDP.Client(0, "127.0.0.1", port, 5001) ' In this case, server doesnt send anything UDP although it can.
        Catch
            Return StartUdpClient(port + 1) ' So we dont put a count Or initialize UDP based server packet handeling.
        End Try
        Return port
    End Function

    Sub ConnectToServer()
        If _tcpClient.IsConnected Then Return ' No need to connect if already connected!
        _tcpClient.Connect("127.0.0.1", 5000)
    End Sub

    Sub EndClient()
        _tcpClient.EndNetwork() ' Auto disconnects and disposes all socket data.
        _udpClient.EndNetwork() ' Clear Udp Data.
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
#End Region

#Region "Receive"
    Sub RoutePacket() ' This will automatically handle the packets received asyncronously.
        ' PacketId(PacketName) = AddressOf Sub to call.
        _tcpClient.PacketId(ServerPackets.MessageRelay) = AddressOf ReceiveMessage
    End Sub

    Sub ReceiveMessage(ByRef data As Byte())
        Dim msg As Message = New Message(data) ' Unpacks byte array and prepairs to read.
        Console.WriteLine(msg.ReadString) ' Reads the string from the message. If multiple data is present, data must be read in order!
        msg.Dispose() ' Cleanup.
    End Sub
#End Region

#Region "Send"
    Sub SendMessage(s As String)
        '/* V THIS Is TO MAKE SURE IT TELLS WHAT MODE YOU ARE IN! V */

        If UdpMode Then
            s = "(=UDP=) " + s
        Else
            s = "(=TCP=) " + s
        End If

        '/* ^ THIS Is TO MAKE SURE IT TELLS WHAT MODE YOU ARE IN! ^ */

        Dim msg As Message = New Message(4 + s.Length + 4) ' Presize for optimization - Size of packet name + Size of message + size of integer that stores the length.
        msg.Write(ClientPackets.Message) ' Must send the name FIRST! This is autoextracted in the server.
        msg.Write(s) ' Already knows how to pack a string!
        If UdpMode Then
            _udpClient.SendData(msg.Data, msg.Location) ' Sends the packet data from our message. Included sending the location For optimization!
        Else
            _tcpClient.SendData(msg.Data, msg.Location) ' Sends the packet data from our message. Included sending the location For optimization!
        End If
        msg.Dispose() ' Cleanup.
    End Sub

    Sub SendUdpPort(ByVal port As Integer)
        Dim msg As Message = New Message(8) ' Presize For optimization - Size Of packet name + Size Of Integer.
        msg.Write(ClientPackets.UdpPort)
        msg.Write(port)
        _tcpClient.SendData(msg.Data, msg.Location) ' Sends the packet data from our message. Included sending the location For optimization!
        msg.Dispose()
    End Sub
#End Region

End Module
