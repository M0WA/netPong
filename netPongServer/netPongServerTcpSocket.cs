using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;

namespace netPongServer
{
    public struct netPongClientInfo
    {
        private string strPrivateClientName;
        private Socket hPrivateClientTcpSocket;

        public string strClientName
        {
            get { return strPrivateClientName; }
            set { strPrivateClientName = value; }
        }

        public Socket hClientTcpSocket
        {
            get { return hPrivateClientTcpSocket; }
            set { hPrivateClientTcpSocket = value; }
        }
    }

    public struct netPongInvitation
    {
        private netPongClientInfo player1;
        private bool isConfirmed1;

        private netPongClientInfo player2;
        private bool isConfirmed2;

        public netPongClientInfo infoPlayer1
        {
            get { return player1; }
            set { player1 = value; }
        }

        public bool isConfirmedPlayer1
        {
            get { return isConfirmed1; }
            set { isConfirmed1 = value; }
        }

        public netPongClientInfo infoPlayer2
        {
            get { return player2; }
            set { player2 = value; }
        }

        public bool isConfirmedPlayer2
        {
            get { return isConfirmed2; }
            set { isConfirmed2 = value; }
        }
    }

    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public ArrayList recvData;
        // Bytes needed for a complete packet
        public int neededBytes;
    }

    public struct playerInviteCallbackParam
    {
        public Socket hSocket;
        public bool bAddNewSession;
        public netPongInvitation newInvitation;
        public int nNewSessionID;
    }

    public enum netPongTcpOpCodes
    {
        NP_TCP_CMD_HELL0 = 0x01,
        NP_TCP_CMD_WELCOME = 0x02,
        NP_TCP_CMD_LISTPLAYERS = 0x03,
        NP_TCP_CMD_PLAYERSLIST = 0x04,
        NP_TCP_CMD_INVITEPLAYER = 0x05,
        NP_TCP_CMD_PLAYERINVITE = 0x06,
        NP_TCP_CMD_CLOSECONNECTION = 0x07,
        NP_TCP_CMD_TEXTMESSAGE = 0x08,
        NP_TCP_CMD_NEWCLIENTCONNECTED = 0x09,
        NP_TCP_CMD_REJECTINVITATION = 0x10,
        NP_TCP_CMD_STARTGAME = 0x11
    };

    public class netPongServerTcpSocket
    {
        // Thread signal.
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private static ArrayList connectedClients = new ArrayList();
        private static ArrayList openInvitations = new ArrayList();

        public netPongServerTcpSocket()
        {
        }

        public static bool StartListening(string strServerIP, int nServerPort)
        {
            IPAddress ipAddress;
            if (strServerIP.Equals(""))
                ipAddress = IPAddress.Any;
            else
            {
                try
                {
                    ipAddress = IPAddress.Parse(strServerIP);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Could not parse server settings, please recheck your IP and Port bindings");
                    return false;
                }
            }

            // Establish the local endpoint for the socket.
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, nServerPort);

            // Data buffer for incoming data.
            //byte[] bytes = new Byte[1024];

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - netPong TCP Control Server started");
                Console.WriteLine("|  * Server DNS Name: " + Dns.GetHostName());
                Console.WriteLine("|  * Server IP:Port : " + localEndPoint.ToString());
                Console.WriteLine("|========================");                

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //console-output
            Console.WriteLine("");
            Console.WriteLine("|========================");
            Console.WriteLine("| - new client is connecting");
            Console.WriteLine("|  * Info: " + handler.RemoteEndPoint.ToString());
            Console.WriteLine("|========================");

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.neededBytes = 3;
            state.recvData = new ArrayList();
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = -1;
            try
            {
                // Read data from the client socket. 
                bytesRead = handler.EndReceive(ar);
            }
            catch (Exception)
            {
                //error while reading from client (i.e. client closed connection)
                CleanDisconnectedClients();
                return;
            }

            //saving recieved bytes
            if (bytesRead > 0)
                for (int i = 0; i < bytesRead; i++)
                    state.recvData.Add(state.buffer[i]);
            else
                return;

            byte[] nPacket = new byte[state.recvData.Count];
            for (int i = 0; i < state.recvData.Count; i++)
                nPacket[i] = (byte)state.recvData[i];

            //switch recieved op-code
            byte nOpCode = nPacket[0];
            switch ((int)nOpCode)
            {
                case ((int)netPongTcpOpCodes.NP_TCP_CMD_HELL0):
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - HELLO Command recieved");
                    Console.WriteLine("|  * Info: " + handler.RemoteEndPoint.ToString());
                    Console.WriteLine("|========================");
                    SendWelcomeCommand(state.workSocket, nPacket);
                    break;

                case (int)netPongTcpOpCodes.NP_TCP_CMD_LISTPLAYERS:
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - LISTPLAYERS Command recieved");
                    Console.WriteLine("|  * Info: " + handler.RemoteEndPoint.ToString());
                    Console.WriteLine("|========================");
                    SendPlayersListCommand(handler);
                    break;

                case (int)netPongTcpOpCodes.NP_TCP_CMD_INVITEPLAYER:
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - INVITEPLAYER Command recieved");
                    Console.WriteLine("|  * Info: " + handler.RemoteEndPoint.ToString());
                    Console.WriteLine("|========================");
                    SendPlayerInviteCommand(nPacket, handler);
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_TEXTMESSAGE):
                    ParseTextMessageCommand(nPacket, handler);
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_REJECTINVITATION):
                    SendRejectInvitationCommand(nPacket, handler);
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_CLOSECONNECTION):
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - CLOSECONNECTION Command recieved");
                    Console.WriteLine("|  * Info: " + handler.RemoteEndPoint.ToString());
                    Console.WriteLine("|========================");
                    try
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    catch (Exception)
                    { }
                    CleanDisconnectedClients();
                    break;

                default:
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - invalid Command recieved, shutting down connection");
                    Console.WriteLine("|  * Info: " + handler.RemoteEndPoint.ToString());
                    Console.WriteLine("|========================");
                    try
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    catch (Exception)
                    { }
                    CleanDisconnectedClients();
                    break;
            }

            try
            {
                // Create the state object.
                StateObject state2 = new StateObject();
                state2.workSocket = handler;
                state2.neededBytes = 3;
                state2.recvData = new ArrayList();
                handler.BeginReceive(state2.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state2);
            }
            catch (Exception)
            {
                try
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                catch (Exception)
                { }
                CleanDisconnectedClients();
            }
        }

        private static void ParseTextMessageCommand(byte[] nPacket, Socket sender)
        {
            byte nUsrLen = nPacket[1];
            byte nMsgLen = nPacket[2];

            byte[] bytesUsr = new byte[nUsrLen];
            string strRecv = "";

            int nPos = 3;
            for (int i = 0; i < nUsrLen; i++)
            {
                bytesUsr[i] = nPacket[nPos];
                nPos++;
            }
            if (!MakePlayerName(bytesUsr, ref strRecv))
                return;

            byte[] bytesMsg = new byte[nMsgLen];
            for (int i = 0; i < nMsgLen; i++)
            {
                bytesMsg[i] = nPacket[nPos];
                nPos++;
            }

            string sendername = "";
            Socket recvSock = null;
            for (int i = 0; i < connectedClients.Count; i++)
            {
                netPongClientInfo cliInfo = (netPongClientInfo)connectedClients[i];

                if (cliInfo.hClientTcpSocket.Equals(sender))
                    sendername = cliInfo.strClientName;

                if (cliInfo.strClientName.Equals(strRecv))
                    recvSock = cliInfo.hClientTcpSocket;
            }
            if (sendername.Equals("") || (recvSock == null))
                return;

            byte[] byteSender = null;
            try
            {
                byteSender = Encoding.ASCII.GetBytes(sendername);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error");
                Console.WriteLine("|  * Info : " + e.ToString());
                Console.WriteLine("|========================");
                return;
            }

            SendTextMessageCommand(recvSock, byteSender, bytesMsg);
        }

        private static void SendTextMessageCommand(Socket recvSock, byte[] sendername, byte[] message)
        {
            byte[] nPacket = new byte[sendername.Length + message.Length + 3];
            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_TEXTMESSAGE;
            nPacket[1] = (byte)sendername.Length;
            nPacket[2] = (byte)message.Length;

            int nPos = 3;
            for (int i = 0; i < (byte)sendername.Length; i++)
            {
                nPacket[nPos] = sendername[i];
                nPos++;
            }

            for (int i = 0; i < (byte)message.Length; i++)
            {
                nPacket[nPos] = message[i];
                nPos++;
            }

            try
            {
                recvSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(SendTextMessageCallback), recvSock);
            }
            catch (Exception)
            {
                try
                {
                    recvSock.Shutdown(SocketShutdown.Both);
                    recvSock.Close();
                    CleanDisconnectedClients();
                }
                catch (Exception)
                {
                }
            }
        }

        private static void SendTextMessageCallback(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;

            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - TEXTMESSAGE command forwarded");
                Console.WriteLine("|  * Info       : " + handler.RemoteEndPoint.ToString());
                Console.WriteLine("|========================");
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error sending TEXTMESSAGE command");
                Console.WriteLine("|  * Exception  : " + e.ToString());
                Console.WriteLine("|  * Info       : " + handler.RemoteEndPoint.ToString());
                Console.WriteLine("|========================");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                CleanDisconnectedClients();
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.neededBytes = 3;
            state.recvData = new ArrayList();
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void NotifyAllNewClientConnected()
        {
            for (int i = 0; i < connectedClients.Count; i++)
            {
                netPongClientInfo cliInfo = (netPongClientInfo)connectedClients[i];
                SendNewClientConnectedCommand(cliInfo.hClientTcpSocket);
            }
        }

        private static void SendNewClientConnectedCommand(Socket cliSock)
        {
            byte[] nPacket = new byte[3];
            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_NEWCLIENTCONNECTED;
            nPacket[1] = 0x00;
            nPacket[2] = 0x00;

            try
            {
                cliSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(SendNewClientConnectedCallback), cliSock);
            }
            catch (Exception)
            {
                try
                {
                    cliSock.Shutdown(SocketShutdown.Both);
                    cliSock.Close();
                    CleanDisconnectedClients();
                }
                catch (Exception)
                {
                }
            }
        }

        private static void SendNewClientConnectedCallback(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;

            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error sending NEWCLIENTCONNECTED command");
                Console.WriteLine("|  * Exception  : " + e.ToString());
                Console.WriteLine("|  * Info       : " + handler.RemoteEndPoint.ToString());
                Console.WriteLine("|========================");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                CleanDisconnectedClients();
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.neededBytes = 3;
            state.recvData = new ArrayList();
            try
            {
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception)
            {
            }
        }

        private static void SendWelcomeCommand(Socket handler, byte[] data)
        {
            String strPlayerName = "";

            byte[] nPlayerName = new byte[data.GetLength(0) - 3];
            int nData = 3;
            for (int i = 0; i < data.GetLength(0) - 3; i++)
            {
                nPlayerName[i] = data[nData];
                nData++;
            }

            if (!MakePlayerName(nPlayerName, ref strPlayerName))
                return;

            if (!IsPlayerNameAvailable(strPlayerName))
                return;

            byte[] welcomePacket = new byte[3];
            welcomePacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_WELCOME;
            welcomePacket[1] = 0x00;
            welcomePacket[2] = 0x00;

            netPongClientInfo cliInfo = new netPongClientInfo();
            cliInfo.strClientName = strPlayerName;
            cliInfo.hClientTcpSocket = handler;

            //console-output
            Console.WriteLine("");
            Console.WriteLine("|========================");
            Console.WriteLine("| - Sending WELCOME command");
            Console.WriteLine("|  * Info       : " + handler.RemoteEndPoint.ToString());
            Console.WriteLine("|  * Player Name: " + cliInfo.strClientName);
            Console.WriteLine("|========================");

            try
            {
                handler.BeginSend(welcomePacket, 0, welcomePacket.Length, 0, new AsyncCallback(SendWelcomeCallback), cliInfo);
            }
            catch (Exception)
            {
                try
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    CleanDisconnectedClients();
                }
                catch (Exception)
                {
                }
            }
        }

        private static void SendWelcomeCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            netPongClientInfo cliInfo = (netPongClientInfo)ar.AsyncState;

            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = cliInfo.hClientTcpSocket.EndSend(ar);

                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Client connected");
                Console.WriteLine("|  * Info       : " + cliInfo.hClientTcpSocket.RemoteEndPoint.ToString());
                Console.WriteLine("|  * Player Name: " + cliInfo.strClientName);
                Console.WriteLine("|========================");

                NotifyAllNewClientConnected();

                connectedClients.Add(cliInfo);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error sending WELCOME command");
                Console.WriteLine("|  * Exception  : " + e.ToString());
                Console.WriteLine("|  * Info       : " + cliInfo.hClientTcpSocket.RemoteEndPoint.ToString());
                Console.WriteLine("|  * Player Name: " + cliInfo.strClientName);
                Console.WriteLine("|========================");

                cliInfo.hClientTcpSocket.Shutdown(SocketShutdown.Both);
                cliInfo.hClientTcpSocket.Close();
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = cliInfo.hClientTcpSocket;
            state.neededBytes = 3;
            state.recvData = new ArrayList();
            cliInfo.hClientTcpSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static bool MakePlayerName(byte[] data, ref string strPlayerName)
        {
            try
            {
                strPlayerName = Encoding.ASCII.GetString(data, 0, data.GetLength(0));
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Could not parse player name");
                Console.WriteLine("|  * Info : " + e.ToString());
                Console.WriteLine("|========================");
                return false;
            }
            return true;
        }

        private static bool IsPlayerNameAvailable(string strPlayerName)
        {
            for (int i = 0; i < connectedClients.Count; i++)
            {
                netPongClientInfo cliInfo = (netPongClientInfo)connectedClients[i];

                if (cliInfo.strClientName.Equals(strPlayerName))
                    return false;
            }
            return true;
        }

        private static void SendPlayersListCommand(Socket cliSock)
        {
            int nPacketSize = 3;
            for (int i = 0; i < connectedClients.Count; i++)
            {
                byte[] byteData = null;
                try
                {
                    byteData = Encoding.ASCII.GetBytes(((netPongClientInfo)connectedClients[i]).strClientName);
                }
                catch (Exception e)
                {
                    //console-output
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - Error");
                    Console.WriteLine("|  * Info : " + e.ToString());
                    Console.WriteLine("|========================");
                    return;
                }
                nPacketSize += (byteData.Length + 1);
            }

            byte[] nPacket = new byte[nPacketSize];
            byte[] nArgLen = BitConverter.GetBytes((Int16)connectedClients.Count);

            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_PLAYERSLIST;
            nPacket[1] = nArgLen[0];
            nPacket[2] = nArgLen[1];

            int nPacketStep = 3;
            for (int i = 0; i < connectedClients.Count; i++)
            {
                byte[] byteData = null;
                try
                {
                    byteData = Encoding.ASCII.GetBytes(((netPongClientInfo)connectedClients[i]).strClientName);
                }
                catch (Exception e)
                {
                    //console-output
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - Error");
                    Console.WriteLine("|  * Info : " + e.ToString());
                    Console.WriteLine("|========================");
                    return;
                }
                nPacket[nPacketStep] = (byte)byteData.Length;
                nPacketStep++;

                for (int x = 0; x < byteData.Length; x++)
                {
                    nPacket[nPacketStep] = byteData[x];
                    nPacketStep++;
                }
            }

            try
            {
                // Begin sending the data to the remote device.
                cliSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(SendPlayersListCallback), cliSock);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error");
                Console.WriteLine("|  * Info : " + e.ToString());
                Console.WriteLine("|========================");
            }
        }

        private static void SendPlayersListCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket)ar.AsyncState;

            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error sending PLAYERSLIST command");
                Console.WriteLine("|  * Exception  : " + e.ToString());
                Console.WriteLine("|  * Info       : " + handler.RemoteEndPoint.ToString());
                Console.WriteLine("|========================");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                CleanDisconnectedClients();
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.neededBytes = 3;
            state.recvData = new ArrayList();
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void SendRejectInvitationCommand(byte[] nPacket, Socket hSenderSocket)
        {
            byte[] arg = new byte[nPacket.Length - 3];
            for (int i = 0; i < arg.Length; i++)
                arg[i] = nPacket[i + 3];

            string strRecvName = "";
            if (!MakePlayerName(arg, ref strRecvName))
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Reciever not found");
                Console.WriteLine("|  * PlayerName : " + strRecvName);
                Console.WriteLine("|  * Invitation not sent");
                Console.WriteLine("|========================");
            }

            Socket recvSock = null;
            string strSenderName = "";
            for (int i = 0; i < connectedClients.Count; i++)
            {
                netPongClientInfo cliInfo = (netPongClientInfo)connectedClients[i];

                if (cliInfo.strClientName.Equals(strRecvName))
                    recvSock = cliInfo.hClientTcpSocket;

                if (cliInfo.hClientTcpSocket.Equals(hSenderSocket))
                    strSenderName = cliInfo.strClientName;
            }

            if ((recvSock == null) || strSenderName.Equals(""))
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Client not connected");
                Console.WriteLine("|  * PlayerName : " + strRecvName);
                Console.WriteLine("|  * Reject Invitation not sent");
                Console.WriteLine("|========================");
                return;
            }

            byte[] byteSender = null;
            try
            {
                byteSender = Encoding.ASCII.GetBytes(strSenderName);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error");
                Console.WriteLine("|  * Info : " + e.ToString());
                Console.WriteLine("|========================");
                return;
            }

            for (int i = 0; i < openInvitations.Count; i++)
            {
                netPongInvitation inv = (netPongInvitation)openInvitations[i];

                // invitation exists
                if ((inv.infoPlayer1.strClientName.Equals(strSenderName) || inv.infoPlayer2.strClientName.Equals(strSenderName)) &&
                     (inv.infoPlayer1.strClientName.Equals(strRecvName) || inv.infoPlayer2.strClientName.Equals(strRecvName))
                    )
                {
                    openInvitations.RemoveAt(i);
                    break;
                }
            }

            byte[] nSendPacket = new byte[byteSender.GetLength(0) + 3];
            nSendPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_REJECTINVITATION;
            nSendPacket[1] = 0x00;
            nSendPacket[2] = (byte)byteSender.Length;
            for (int i = 0; i < byteSender.Length; i++)
                nSendPacket[i + 3] = byteSender[i];

            try
            {
                // Begin sending the data to the remote device.
                recvSock.BeginSend(nSendPacket, 0, nSendPacket.Length, 0, new AsyncCallback(SendRejectInviteCallback), recvSock);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error");
                Console.WriteLine("|  * Info : " + e.ToString());
                Console.WriteLine("|========================");
            }
        }

        private static void SendRejectInviteCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket)ar.AsyncState;

            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error sending REJECTINVITE command");
                Console.WriteLine("|  * Exception  : " + e.ToString());
                Console.WriteLine("|  * Info       : " + handler.RemoteEndPoint.ToString());
                Console.WriteLine("|========================");
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.neededBytes = 3;
            state.recvData = new ArrayList();
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void SendPlayerInviteCommand(byte[] nPacket, Socket hSenderSocket)
        {
            byte[] arg = new byte[nPacket.Length - 3];
            for (int i = 0; i < arg.Length; i++)
                arg[i] = nPacket[i + 3];

            string strRecvName = "";
            if (!MakePlayerName(arg, ref strRecvName))
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Reciever not found");
                Console.WriteLine("|  * PlayerName : " + strRecvName);
                Console.WriteLine("|  * Invitation not sent");
                Console.WriteLine("|========================");
            }

            Socket recvSock = null;
            string strSenderName = "";
            for (int i = 0; i < connectedClients.Count; i++)
            {
                netPongClientInfo cliInfo = (netPongClientInfo)connectedClients[i];

                if (cliInfo.strClientName.Equals(strRecvName))
                    recvSock = cliInfo.hClientTcpSocket;

                if (cliInfo.hClientTcpSocket.Equals(hSenderSocket))
                    strSenderName = cliInfo.strClientName;
            }

            if ((recvSock == null) || strSenderName.Equals(""))
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Client not connected");
                Console.WriteLine("|  * PlayerName : " + strRecvName);
                Console.WriteLine("|  * Invitation not sent");
                Console.WriteLine("|========================");
                return;
            }

            byte[] byteData = null;
            try
            {
                byteData = Encoding.ASCII.GetBytes(strSenderName);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error");
                Console.WriteLine("|  * Info : " + e.ToString());
                Console.WriteLine("|========================");
                return;
            }

            bool bAddNewSession = false;
            netPongInvitation newSessionInvitation = new netPongInvitation();
            int newSessionID = -1;

            for (int i = 0; i < openInvitations.Count; i++)
            {
                netPongInvitation inv = (netPongInvitation)openInvitations[i];

                // invitation already exists, must be a confirm message
                if ((inv.infoPlayer1.strClientName.Equals(strSenderName) || inv.infoPlayer2.strClientName.Equals(strSenderName)) &&
                     (inv.infoPlayer1.strClientName.Equals(strRecvName) || inv.infoPlayer2.strClientName.Equals(strRecvName))
                    )
                {
                    if ((inv.infoPlayer1.strClientName.Equals(strSenderName) && inv.isConfirmedPlayer1) ||
                         (inv.infoPlayer2.strClientName.Equals(strSenderName) && inv.isConfirmedPlayer2))
                    {
                        //console-output
                        Console.WriteLine("");
                        Console.WriteLine("|========================");
                        Console.WriteLine("| - An invitation already exits between " + strRecvName + " and " + strSenderName);
                        Console.WriteLine("|========================");
                        return;
                    }
                    //console-output
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - Invitation confirmed between " + strRecvName + " and " + strSenderName);
                    Console.WriteLine("|========================");
                    bAddNewSession = true;
                    newSessionInvitation = inv;
                    netPongUdpGameServer.AddGameSession(newSessionInvitation, out newSessionID);
                    break;
                }

                //check if one of the player is already in a game
                if (inv.isConfirmedPlayer1)
                    if (inv.infoPlayer1.strClientName.Equals(strSenderName) || inv.infoPlayer1.strClientName.Equals(strRecvName))
                    {
                        //console-output
                        Console.WriteLine("");
                        Console.WriteLine("|========================");
                        Console.WriteLine("| - Error: cannot send invitation, player " + inv.infoPlayer1.strClientName + " is already in a game");
                        Console.WriteLine("|========================");
                        return;
                    }

                if (inv.isConfirmedPlayer2)
                    if (inv.infoPlayer2.strClientName.Equals(strSenderName) || inv.infoPlayer2.strClientName.Equals(strRecvName))
                    {
                        //console-output
                        Console.WriteLine("");
                        Console.WriteLine("|========================");
                        Console.WriteLine("| - Error: cannot send invitation, player " + inv.infoPlayer2.strClientName + " is already in a game");
                        Console.WriteLine("|========================");
                        return;
                    }
            }

            byte[] nSendPacket = new byte[byteData.GetLength(0) + 3];
            nSendPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_PLAYERINVITE;
            nSendPacket[1] = 0x00;
            nSendPacket[2] = (byte)byteData.Length;
            for (int i = 0; i < byteData.Length; i++)
                nSendPacket[i + 3] = byteData[i];

            if (!bAddNewSession)
            {
                netPongInvitation newInv = new netPongInvitation();

                netPongClientInfo player1Info = new netPongClientInfo();
                GetClientInfoFromName(strSenderName, out player1Info);
                newInv.infoPlayer1 = player1Info;
                newInv.isConfirmedPlayer1 = true;

                netPongClientInfo player2Info = new netPongClientInfo();
                GetClientInfoFromName(strRecvName, out player2Info);
                newInv.infoPlayer2 = player2Info;
                newInv.isConfirmedPlayer2 = false;

                openInvitations.Add(newInv);
            }

            playerInviteCallbackParam param = new playerInviteCallbackParam();
            param.bAddNewSession = false;
            param.hSocket = recvSock;
            if (bAddNewSession)
            {
                param.bAddNewSession = true;
                param.nNewSessionID = newSessionID;
                param.newInvitation = newSessionInvitation;
            }
                
            try
            {
                // Begin sending the data to the remote device.
                recvSock.BeginSend(nSendPacket, 0, nSendPacket.Length, 0, new AsyncCallback(SendPlayerInviteCallback), param);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error");
                Console.WriteLine("|  * Info : " + e.ToString());
                Console.WriteLine("|========================");
            }
        }

        private static void SendPlayerInviteCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            playerInviteCallbackParam param = (playerInviteCallbackParam)ar.AsyncState;
            Socket handler = param.hSocket;

            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);

                if (param.bAddNewSession)
                    SendStartGameCommand(param.newInvitation, param.nNewSessionID);
            }
            catch (Exception e)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - Error sending PLAYERINVITE command");
                Console.WriteLine("|  * Exception  : " + e.ToString());
                Console.WriteLine("|  * Info       : " + handler.RemoteEndPoint.ToString());
                Console.WriteLine("|========================");
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.neededBytes = 3;
            state.recvData = new ArrayList();
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void CleanDisconnectedClients()
        {
            bool bRefresh = false;
            ArrayList tmpCon = new ArrayList();
            for (int i = 0; i < connectedClients.Count; i++)
            {
                netPongClientInfo cliInfo = (netPongClientInfo)connectedClients[i];
                if (cliInfo.hClientTcpSocket.Connected)
                {
                    tmpCon.Add(cliInfo);
                }
                else
                {
                    bRefresh = true;
                    try
                    {
                        cliInfo.hClientTcpSocket.Shutdown(SocketShutdown.Both);
                        cliInfo.hClientTcpSocket.Close();
                    }
                    catch (Exception)
                    {
                    }

                    //console-output
                    Console.WriteLine("");
                    Console.WriteLine("|========================");
                    Console.WriteLine("| - Player disconnected");
                    Console.WriteLine("|  * PlayerName : " + cliInfo.strClientName);
                    Console.WriteLine("|========================");

                    ArrayList tmpInv = new ArrayList();
                    for (int x = 0; x < openInvitations.Count; x++)
                    {
                        netPongInvitation inv = (netPongInvitation)openInvitations[x];
                        if (inv.infoPlayer1.strClientName.Equals(cliInfo.strClientName) || inv.infoPlayer2.strClientName.Equals(cliInfo.strClientName))
                            continue;
                        else
                            tmpInv.Add(inv);
                    }
                    openInvitations = tmpInv;
                }
            }
            connectedClients = tmpCon;

            if (bRefresh)
                NotifyAllNewClientConnected();
        }

        private static bool GetClientInfoFromName(string strClientName, out netPongClientInfo clientInfo)
        {
            for (int i = 0; i < connectedClients.Count; i++)
            {
                if (((netPongClientInfo)connectedClients[i]).strClientName.Equals(strClientName))
                {
                    clientInfo = (netPongClientInfo)connectedClients[i];
                    return true;
                }
            }
            clientInfo = new netPongClientInfo();
            return false;
        }

        private static void SendStartGameCommand(netPongInvitation inv, int nSessionID)
        {
            byte[] byteArgLen = BitConverter.GetBytes((Int16)10);
            byte[] byteSessionID = BitConverter.GetBytes((Int32)nSessionID);
            byte[] byteServerPort = BitConverter.GetBytes((Int16)2411);

            byte[] bytePacket = new byte[10];
            bytePacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_STARTGAME;
            bytePacket[1] = byteArgLen[0];
            bytePacket[2] = byteArgLen[1];

            for (int i = 0; i < byteServerPort.Length; i++)
                bytePacket[i + 4] = byteServerPort[i];

            for (int i = 0; i < byteSessionID.Length; i++)
                bytePacket[i + 4 + byteServerPort.Length] = byteSessionID[i];

            bytePacket[3] = 0x01; //player id
            try
            {
                inv.infoPlayer1.hClientTcpSocket.BeginSend(bytePacket, 0, bytePacket.Length, 0, new AsyncCallback(SendStartGameCallback), inv.infoPlayer1.hClientTcpSocket);
            }
            catch (Exception)
            {
            }

            bytePacket[3] = 0x02; //player id
            try
            {
                inv.infoPlayer2.hClientTcpSocket.BeginSend(bytePacket, 0, bytePacket.Length, 0, new AsyncCallback(SendStartGameCallback), inv.infoPlayer2.hClientTcpSocket);
            }
            catch (Exception)
            {
            }
        }

        private static void SendStartGameCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket)ar.AsyncState;

            try
            {
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception)
            {
            }

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            state.neededBytes = 3;
            state.recvData = new ArrayList();
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }
    }
}
