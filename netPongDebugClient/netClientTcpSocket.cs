using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace netPongDebugClient
{
    class netClientTcpSocket
    {
        private static ArrayList openInvitations = new ArrayList();
        private static Socket m_hServerSock = null;
        private static string m_strUsername = "";

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

        public netClientTcpSocket(string strUsername, string strServerIP, int nServerPort)
        {
            m_strUsername = strUsername;

            try
            {
                IPAddress ipAddress = IPAddress.Parse(strServerIP);
                IPEndPoint srvEndPoint = new IPEndPoint(ipAddress, nServerPort);
                m_hServerSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                netPongClientProgram.m_cSettingsForm.strLogString += ("Connecting to server (" + srvEndPoint.ToString() + ")...");

                m_hServerSock.BeginConnect(srvEndPoint, new AsyncCallback(ConnectCallBack), m_hServerSock);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static void ConnectCallBack(IAsyncResult ar)
        {
            Socket hServerSock = (Socket)ar.AsyncState;
            try
            {
                hServerSock.EndConnect(ar);
                netPongClientProgram.m_cSettingsForm.strLogString += ("OK\r\n");
                SendHelloCommand();
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static void SendHelloCommand()
        {
            byte[] byteData = null;
            try
            {
                byteData = Encoding.ASCII.GetBytes(m_strUsername);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
                return;
            }
            byte[] nPacket = new byte[byteData.GetLength(0) + 3];

            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_HELL0;
            nPacket[1] = 0x00;
            nPacket[2] = (byte)byteData.GetLength(0);
            for (int i = 0; i < byteData.GetLength(0); i++)
                nPacket[i + 3] = byteData[i];

            try
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Logging in using username " + m_strUsername + "...");
                m_hServerSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(SendHelloCallback), m_hServerSock);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }

        }

        public static void SendHelloCallback(IAsyncResult ar)
        {
            try
            {
                int nBytesSent = m_hServerSock.EndSend(ar);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = m_hServerSock;
                state.neededBytes = 3;
                state.recvData = new ArrayList();
                m_hServerSock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static void SendCloseConnectionCommand()
        {
            byte[] nPacket = new byte[3];
            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_CLOSECONNECTION;
            nPacket[1] = 0x00;
            nPacket[2] = 0x00;

            try
            {
                m_hServerSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(CloseConnectionCallback), m_hServerSock);
            }
            catch (Exception)
            {
            }
        }

        public static void CloseConnectionCallback(IAsyncResult ar)
        {
            try
            {
                int nBytesSent = m_hServerSock.EndSend(ar);

                m_hServerSock.Shutdown(SocketShutdown.Both);
                m_hServerSock.Close();
                netPongClientProgram.m_cSettingsForm.strLogString += ("Connection closed\r\n");
            }
            catch (Exception e)
            {
                m_hServerSock.Shutdown(SocketShutdown.Both);
                m_hServerSock.Close();
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
            netPongClientProgram.m_cSettingsForm.bLoggedIn = false;
        }

        public static void SendTextMessageCommand(string username, string message)
        {
            byte[] byteUser = null;
            try
            {
                byteUser = Encoding.ASCII.GetBytes(username);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
                return;
            }

            byte[] byteMsg = null;
            try
            {
                byteMsg = Encoding.ASCII.GetBytes(message);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
                return;
            }

            byte nUsrLen = (byte)byteUser.Length;
            byte nMsgLen = (byte)byteMsg.Length;
            if (byteMsg.Length > 255)
                nMsgLen = 255;

            byte[] nPacket = new byte[nUsrLen + nMsgLen + 3];
            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_TEXTMESSAGE;
            nPacket[1] = nUsrLen;
            nPacket[2] = nMsgLen;

            int nCurPos = 3;
            for (int i = 0; i < nUsrLen; i++)
            {
                nPacket[nCurPos] = byteUser[i];
                nCurPos++;
            }

            for (int i = 0; i < nMsgLen; i++)
            {
                nPacket[nCurPos] = byteMsg[i];
                nCurPos++;
            }

            try
            {
                m_hServerSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(SendTextMessageCallback), m_hServerSock);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static void SendTextMessageCallback(IAsyncResult ar)
        {
            try
            {
                int nBytesSent = m_hServerSock.EndSend(ar);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = m_hServerSock;
                state.neededBytes = 3;
                state.recvData = new ArrayList();
                m_hServerSock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static void SendListPlayersCommand()
        {
            byte[] nPacket = new byte[3];
            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_LISTPLAYERS;
            nPacket[1] = 0x00;
            nPacket[2] = 0x00;

            try
            {
                m_hServerSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(SendListPlayersCallback), m_hServerSock);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static void SendListPlayersCallback(IAsyncResult ar)
        {
            try
            {
                int nBytesSent = m_hServerSock.EndSend(ar);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = m_hServerSock;
                state.neededBytes = 3;
                state.recvData = new ArrayList();
                m_hServerSock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static void SendInvitePlayerCommand(string strPlayerName)
        {
            byte[] byteData = null;
            try
            {
                byteData = Encoding.ASCII.GetBytes(strPlayerName);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
                return;
            }
            byte[] nPacket = new byte[byteData.GetLength(0) + 3];
            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_INVITEPLAYER;
            nPacket[1] = 0x00;
            nPacket[2] = (byte)byteData.Length;

            for (int i = 0; i < byteData.Length; i++)
                nPacket[i + 3] = byteData[i];

            try
            {
                m_hServerSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(SendInvitePlayerCallback), strPlayerName);
                netPongClientProgram.m_cSettingsForm.strLogString += ("Sending invitation to " + strPlayerName + "\r\n");
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        private static void SendInvitePlayerCallback(IAsyncResult ar)
        {
            string strPlayerName = (string)ar.AsyncState;
            try
            {
                int nBytesSent = m_hServerSock.EndSend(ar);
                openInvitations.Add(strPlayerName);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = m_hServerSock;
                state.neededBytes = 3;
                state.recvData = new ArrayList();
                m_hServerSock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static ArrayList ParsePlayersList(byte[] nPacket)
        {
            ArrayList names = new ArrayList();

            byte[] nListLen = new byte[2];
            nListLen[0] = nPacket[2];
            nListLen[1] = nPacket[1];
            Int16 nConnectedClientCount = BitConverter.ToInt16(nPacket, 1);

            byte[] pNames = new byte[nPacket.Length - 3];
            for (int i = 0; i < nPacket.Length - 3; i++)
                pNames[i] = nPacket[i + 3];

            int nPos = 0;
            for (int x = 0; x < nConnectedClientCount; x++)
            {
                byte namelen = pNames[nPos];
                nPos++;

                byte[] name = new byte[namelen];
                for (int i = 0; i < namelen; i++)
                {
                    name[i] = pNames[nPos];
                    nPos++;
                }
                string strPlayerName = "";
                MakePlayerName(name, ref strPlayerName);
                names.Add(strPlayerName);
            }

            return names;
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

        public static void ParseRejectInvite(byte[] nPacket)
        {
            Int16 nNameLen = (Int16)nPacket[2];
            byte[] name = new byte[nNameLen];

            for (int i = 0; i < nNameLen; i++)
                name[i] = nPacket[i + 3];

            string strPlayerName = "";
            if (!MakePlayerName(name, ref strPlayerName))
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("No valid username found\r\n");
                return;
            }

            for (int i = 0; i < openInvitations.Count; i++)
            {
                string strPlayer = (string)openInvitations[i];
                if (strPlayerName.Equals(strPlayer))
                {
                    netPongClientProgram.m_cSettingsForm.strLogString += ("Your invitation has been rejected from " + strPlayer + "\r\n");
                    openInvitations.RemoveAt(i);
                    break;
                }
            }
        }

        public static void ParsePlayerInvite(byte[] nPacket)
        {
            Int16 nNameLen = (Int16)nPacket[2];
            byte[] name = new byte[nNameLen];

            for (int i = 0; i < nNameLen; i++)
                name[i] = nPacket[i + 3];

            string strPlayerName = "";
            if (!MakePlayerName(name, ref strPlayerName))
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("No valid username found\r\n");
                return;
            }

            for (int i = 0; i < openInvitations.Count; i++)
            {
                string strPlayer = (string)openInvitations[i];
                if (strPlayerName.Equals(strPlayer))
                {
                    netPongClientProgram.m_cSettingsForm.strLogString += ("Recieved invitation confirmation from " + strPlayerName + "\r\n");
                    netPongClientProgram.m_cNetClient.StartGameSession();
                    return;
                }
            }
            netPongClientProgram.m_cSettingsForm.strLogString += ("Recieved invitation from " + strPlayerName + "\r\n");
            netPongClientProgram.m_cSettingsForm.strInvitationName = strPlayerName;
        }

        private static void ParseTextMessageCommand(byte[] nPacket)
        {
            byte nUsrLen = nPacket[1];
            byte nMsgLen = nPacket[2];

            byte[] bytesUsr = new byte[nUsrLen];
            string strSender = "";

            int nPos = 3;
            for (int i = 0; i < nUsrLen; i++)
            {
                bytesUsr[i] = nPacket[nPos];
                nPos++;
            }
            if (!MakePlayerName(bytesUsr, ref strSender))
                return;

            byte[] bytesMsg = new byte[nMsgLen];
            string strMsg = "";
            for (int i = 0; i < nMsgLen; i++)
            {
                bytesMsg[i] = nPacket[nPos];
                nPos++;
            }
            if (!MakePlayerName(bytesMsg, ref strMsg))
                return;

            ArrayList msgChatArray = new ArrayList();
            msgChatArray.Add(strSender);
            msgChatArray.Add(strMsg);

            netPongClientProgram.m_chatWindow.AddChatMessage = msgChatArray;
            netPongClientProgram.m_cSettingsForm.strLogString += "Incoming message from " + strSender + "\r\n";
        }


        public static void SendRejectInviteCommand(string strPlayerName)
        {
            byte[] byteData = null;
            try
            {
                byteData = Encoding.ASCII.GetBytes(strPlayerName);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
                return;
            }
            byte[] nPacket = new byte[byteData.GetLength(0) + 3];
            nPacket[0] = (byte)netPongTcpOpCodes.NP_TCP_CMD_REJECTINVITATION;
            nPacket[1] = 0x00;
            nPacket[2] = (byte)byteData.Length;

            for (int i = 0; i < byteData.Length; i++)
                nPacket[i + 3] = byteData[i];

            try
            {
                m_hServerSock.BeginSend(nPacket, 0, nPacket.Length, 0, new AsyncCallback(SendRejectInviteCallback), strPlayerName);
                netPongClientProgram.m_cSettingsForm.strLogString += ("Rejecting invitation from " + strPlayerName + "\r\n");
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        private static void SendRejectInviteCallback(IAsyncResult ar)
        {
            string strPlayerName = (string)ar.AsyncState;
            try
            {
                int nBytesSent = m_hServerSock.EndSend(ar);
                for (int i = 0; i < openInvitations.Count; i++)
                {
                    string strTmp = (string)openInvitations[i];
                    if (strPlayerName.Equals(strTmp))
                    {
                        openInvitations.RemoveAt(i);
                        break;
                    }
                }

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = m_hServerSock;
                state.neededBytes = 3;
                state.recvData = new ArrayList();
                m_hServerSock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                netPongClientProgram.m_cSettingsForm.strLogString += ("Error\r\n");
                netPongClientProgram.m_cSettingsForm.strLogString += (e.ToString() + "\r\n");
            }
        }

        public static void ReadCallback(IAsyncResult ar)
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
                case ((int)netPongTcpOpCodes.NP_TCP_CMD_WELCOME):
                    netPongClientProgram.m_cSettingsForm.strLogString += ("Logged In\r\n");
                    netPongClientProgram.m_cSettingsForm.bLoggedIn = true;
                    SendListPlayersCommand();
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_PLAYERSLIST):
                    netPongClientProgram.m_cSettingsForm.strLogString += ("Recieved players list\r\n");
                    ArrayList players = ParsePlayersList(nPacket);
                    netPongClientProgram.m_cSettingsForm.strLogString += ("Players online " + players.Count + ":\r\n");
                    netPongClientProgram.m_cSettingsForm.playerOnline = players;
                    netPongClientProgram.m_chatWindow.playersArray = players;
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_PLAYERINVITE):
                    ParsePlayerInvite(nPacket);
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_REJECTINVITATION):
                    ParseRejectInvite(nPacket);
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_NEWCLIENTCONNECTED):
                    SendListPlayersCommand();
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_TEXTMESSAGE):
                    ParseTextMessageCommand(nPacket);
                    break;

                case ((int)netPongTcpOpCodes.NP_TCP_CMD_STARTGAME):
                    ParseStartGame(nPacket, ((IPEndPoint)handler.RemoteEndPoint).Address);
                    break;

                default:
                    netPongClientProgram.m_cSettingsForm.bLoggedIn = false;
                    netPongClientProgram.m_cSettingsForm.strLogString += ("Invalid command recieved, closing connection\r\n");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    return;
            }

            // Create the state object.
            StateObject state2 = new StateObject();
            state2.workSocket = m_hServerSock;
            state2.neededBytes = 3;
            state2.recvData = new ArrayList();

            try
            {
                m_hServerSock.BeginReceive(state2.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state2);
            }
            catch (Exception)
            {
                netPongClientProgram.m_cSettingsForm.bLoggedIn = false;
            }
        }

        private static void ParseStartGame(byte[] bytePacket, IPAddress serverIP)
        {
            byte nPlayerID = bytePacket[3];
            Int16 nServerPort = BitConverter.ToInt16(bytePacket, 4);
            Int32 nSessionID  = BitConverter.ToInt32(bytePacket, 6);

            IPEndPoint serverEP = new IPEndPoint(serverIP, nServerPort);
            netPongClientProgram.m_udpGameSession = new netPongUdpGameClientSession(nSessionID, nPlayerID, serverEP);

            netPongClientProgram.m_cSettingsForm.strLogString += ("Establishing UDP game session (ID: " + nSessionID  + " / Player ID: " + nPlayerID + ")\r\n");
            netPongClientProgram.m_cSettingsForm.strLogString += ("UDP game session is at " + serverEP.Address.ToString() + ":" + serverEP.Port + "\r\n");
        }
    }
}
