using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Microsoft.DirectX;

namespace netPongServer
{
    public struct netPongUDPParsePacketThreadParam
    {
        private IPEndPoint m_senderRemoteEP;
        private byte[] m_packetData;
        private int m_gameSessionIndex;

        public netPongUDPParsePacketThreadParam(IPEndPoint senderRemoteEP, int gameSessionIndex, byte[] packetData)
        {
            m_senderRemoteEP = senderRemoteEP;
            m_gameSessionIndex = gameSessionIndex;
            m_packetData = packetData;
        }

        public IPEndPoint senderRemoteEP
        {
            get { return m_senderRemoteEP; }
            set { m_senderRemoteEP = value; }
        }

        public int gameSessionIndex
        {
            get { return m_gameSessionIndex; }
            set { m_gameSessionIndex = value; }
        }

        public byte[] packetData
        {
            get { return m_packetData; }
            set { m_packetData = value; }
        }
    }

    public enum netPongUdpOpCodes
    {
        NP_UDP_CMD_READY = 0x01,
        NP_UDP_CMD_CHANGE_PADDLE_POS = 0x02,
        NP_UDP_CMD_SET_BALL_POS = 0x03
    };

    static class netPongUdpGameServer
    {
        static private int m_nNextSessionID = 0;
        static private List<netPongUdpGameSession> m_runningSessions = new List<netPongUdpGameSession>();
        static private object m_runningSessionsLock = new object();
        static private UdpClient m_udpClient = null;
        static private object m_udpClientLock = new object();
                
        public static void StartServer(string strServerListenIP, int nListenPort)
        {
            CreateServerSocket(strServerListenIP, nListenPort);
        }

        public static void AddGameSession(netPongInvitation invitation, out int nSessionID)
        {
            netPongUdpGameSession gameSession = new netPongUdpGameSession(m_nNextSessionID, invitation.infoPlayer1.strClientName, invitation.infoPlayer2.strClientName, null, null);
            
            nSessionID = m_nNextSessionID;
            m_nNextSessionID++;

            lock (m_runningSessionsLock)
            {
                m_runningSessions.Add(gameSession);
            }
        }

        private static void CreateServerSocket(string strServerIP, int nListenPort)
        {
            bool bIsInitialized = false;
            lock (m_udpClientLock)
            {
                //server sock is already initialized
                if (m_udpClient != null)
                    bIsInitialized = true;
            }
            if (bIsInitialized)
                return;

            IPAddress rip = IPAddress.Parse(strServerIP);
            IPEndPoint localEndPoint = new IPEndPoint(rip, nListenPort);

            try
            {
                lock (m_udpClientLock)
                {
                    m_udpClient = new UdpClient(localEndPoint);
                }
                
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - netPong UDP GameSession Server started");
                Console.WriteLine("|  * Server DNS Name: " + Dns.GetHostName());
                Console.WriteLine("|  * Server IP:Port : " + localEndPoint.ToString());
                Console.WriteLine("|========================");
            }
            catch (Exception)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - netPong UDP Game Server could not start on port " + nListenPort);
                Console.WriteLine("|========================");
            }

            lock (m_udpClientLock)
            {
                m_udpClient.BeginReceive(new AsyncCallback(ReadData), null );
            }
        }

        private static void ReadData(IAsyncResult ar)
        {
            IPEndPoint senderRemoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytePacket = null;

            try
            {
                lock (m_udpClientLock)
                {
                    bytePacket = m_udpClient.EndReceive(ar, ref senderRemoteEP);
                }
            }
            catch (Exception err)
            {
                //console-output
                Console.WriteLine("");
                Console.WriteLine("|========================");
                Console.WriteLine("| - netPong UDP Game Server");
                Console.WriteLine("|  * Error while recieving: ");
                Console.WriteLine("|    " + err.ToString());
                Console.WriteLine("|========================");
            }

            int gameSessionIndexc;
            IsValidSender(senderRemoteEP, out gameSessionIndexc);
            Thread parseThread = new Thread(new ParameterizedThreadStart(ParsePacket));
            netPongUDPParsePacketThreadParam parseThreadParam = new netPongUDPParsePacketThreadParam(senderRemoteEP, gameSessionIndexc, bytePacket);
            parseThread.Start(parseThreadParam);

            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient.BeginReceive(new AsyncCallback(ReadData), null);
                }
                catch (Exception)
                {
                }
            }
        }  

        private static bool IsValidSender(IPEndPoint sender, out int gameSessionIndex)
        {
            lock (m_runningSessionsLock)
            {
                for (int i = 0; i < m_runningSessions.Count; i++)
                {
                    if ( ( (m_runningSessions[i].epPlayer1 != null) && m_runningSessions[i].epPlayer1.Equals(sender) ) ||
                         ( (m_runningSessions[i].epPlayer2 != null) && m_runningSessions[i].epPlayer2.Equals(sender) ) )
                    {
                        gameSessionIndex = i;
                        return true;
                    }
                }
            }

            gameSessionIndex = -1;
            return false;
        }

        private static void ParsePacket(object param)
        {
            netPongUDPParsePacketThreadParam threadParam = (netPongUDPParsePacketThreadParam)param;

            if (threadParam.packetData == null)
                return;

            if (threadParam.packetData.Length < 3)
                return;

            int nOpCode = (int)threadParam.packetData[2];
            switch (nOpCode)
            {
                case (int)netPongUdpOpCodes.NP_UDP_CMD_READY:
                    ParseReadyCommand(threadParam);
                    break;
                case (int)netPongUdpOpCodes.NP_UDP_CMD_CHANGE_PADDLE_POS:
                    ParseChangePaddlePos(threadParam);
                    break;
                default:
                    break;
            }
        }

        private static void ParseReadyCommand(netPongUDPParsePacketThreadParam threadParam)
        {
            byte[] packet = threadParam.packetData;
            byte nPlayerID = packet[3];
            Int32 nSessionID = BitConverter.ToInt32(packet, 4);

            lock (m_runningSessionsLock)
            {
                for (int i = 0; i < m_runningSessions.Count; i++)
                {
                    if (m_runningSessions[i].nSessionID == nSessionID)
                    {
                        netPongUdpGameSession session = m_runningSessions[i];
                        if (nPlayerID == 1)
                        {
                            //console-output
                            Console.WriteLine("");
                            Console.WriteLine("|========================");
                            Console.WriteLine("| - netPong UDP Game Server");
                            Console.WriteLine("|  * Player 1 confirmed UDP Connection ");
                            Console.WriteLine("|========================");

                            session.epPlayer1 = threadParam.senderRemoteEP;
                            m_runningSessions[i] = session;
                        }
                        else if (nPlayerID == 2)
                        {
                            //console-output
                            Console.WriteLine("");
                            Console.WriteLine("|========================");
                            Console.WriteLine("| - netPong UDP Game Server");
                            Console.WriteLine("|  * Player 2 confirmed UDP Connection ");
                            Console.WriteLine("|========================");

                            session.epPlayer2 = threadParam.senderRemoteEP;
                            m_runningSessions[i] = session;
                        }
                        m_runningSessions[i] = session;

                        if ((session.epPlayer1 != null) && (session.epPlayer2 != null))
                        {
                            //console-output
                            Console.WriteLine("");
                            Console.WriteLine("|========================");
                            Console.WriteLine("| - netPong UDP Game Server");
                            Console.WriteLine("|  * Starting BallCalculationThread for Session ID " + session.nSessionID);
                            Console.WriteLine("|========================");

                            session.bIsInGame = true;
                            session.hBallCalcThread = new Thread(new ParameterizedThreadStart(BallCalculationThread));
                            session.hBallCalcThread.IsBackground = true;
                            m_runningSessions[i] = session;
                            session.hBallCalcThread.Start(i);
                        }
                        break;
                    }
                }
            }
        }

        private static void BallCalculationThread(object param)
        {
            int nSessionIndex = (int)param;
            netPongUdpGameSession session;

            lock (m_runningSessionsLock)
            {
                 session = (netPongUdpGameSession)m_runningSessions[nSessionIndex];
            }
            session.lastBallPosCalculation = DateTime.Now;
            lock (m_runningSessionsLock)
            {
                m_runningSessions[nSessionIndex] = session;
            }

            while (true)
            {
                Thread.Sleep(10);
                CheckCollision(nSessionIndex);
                CalculateBallPosition(nSessionIndex);
                SendSetBallPositionCommand(nSessionIndex);
            }
        }

        private static void CalculateBallPosition(int nSessionIndex)
        {
            netPongUdpGameSession session;
            lock (m_runningSessionsLock)
            {
                session = (netPongUdpGameSession)m_runningSessions[nSessionIndex];
            }

            //time since last calculation
            DateTime now = DateTime.Now;
            TimeSpan tDif = now.Subtract(session.lastBallPosCalculation);
            session.lastBallPosCalculation = now;

            Vector3 newBallPos = session.ballPos;

            //recalculate X pos
            newBallPos.X += (float)tDif.TotalMilliseconds * session.ballDirSpeed.X * 0.05f;

            //recalculate Z pos
            newBallPos.Z += (float)tDif.TotalMilliseconds * session.ballDirSpeed.Z;

            session.ballPos = newBallPos;

            lock (m_runningSessionsLock)
            {
                m_runningSessions[nSessionIndex] = session;
            }
        }

        private static void SendSetBallPositionCallback(IAsyncResult ar)
        {
            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient.EndSend(ar);
                }
                catch (Exception)
                {
                }
            }
        }

        private static void SendSetBallPositionCommand(int nSessionIndex)
        {
            netPongUdpGameSession session;
            lock (m_runningSessionsLock)
            {
                session = (netPongUdpGameSession)m_runningSessions[nSessionIndex];
            }


            byte[] bytePacket = new byte[11];
            byte[] byteLen = BitConverter.GetBytes((Int16)11);
            byte[] byteBallPosX = BitConverter.GetBytes(session.ballPos.X);
            byte[] byteBallPosZ = BitConverter.GetBytes(session.ballPos.Z);
            bytePacket[0] = byteLen[0];
            bytePacket[1] = byteLen[1];
            bytePacket[2] = (byte)netPongUdpOpCodes.NP_UDP_CMD_SET_BALL_POS;
            for (int i = 0; i < byteBallPosX.Length; i++)
                bytePacket[i+3] = byteBallPosX[i];

            for (int i = 0; i < byteBallPosX.Length; i++)
                bytePacket[i+3+byteBallPosZ.Length] = byteBallPosZ[i];

            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient.BeginSend(bytePacket, bytePacket.Length, session.epPlayer1, new AsyncCallback(SendSetBallPositionCallback), null);
                    m_udpClient.BeginSend(bytePacket, bytePacket.Length, session.epPlayer2, new AsyncCallback(SendSetBallPositionCallback), null);
                }
                catch (Exception)
                {
                }
            }
        }

        private static void CheckCollision(int nSessionIndex)
        {
            netPongUdpGameSession session;

            lock (m_runningSessionsLock)
            {
                session = (netPongUdpGameSession)m_runningSessions[nSessionIndex];
            }

            CheckPaddleCollisions(ref session);
            CheckWallCollisions(ref session);

            lock (m_runningSessionsLock)
            {
                m_runningSessions[nSessionIndex] = session;
            }
        }

        private static void CheckWallCollisions(ref netPongUdpGameSession session)
        {
            Vector3 newBallPos = session.ballPos;
            Vector3 newBallDirSpeed = session.ballDirSpeed;

            if (newBallPos.X > 30.0f)
            {
                newBallPos.X = 30.0f;
                newBallDirSpeed.X *= -1;
            }
            if (newBallPos.X < -30.0f)
            {
                newBallPos.X = -30.0f;
                newBallDirSpeed.X *= -1;
            }

            session.ballPos = newBallPos;
            session.ballDirSpeed = newBallDirSpeed;
        }

        private static void CheckPaddleCollisions(ref netPongUdpGameSession session)
        {
            Vector3 newBallPos = session.ballPos;
            Vector3 newBallDirSpeed = session.ballDirSpeed;

            //player1 must react
            if (newBallPos.Z > 24.0f)
            {
                float xStartPanel = session.fPlayer1PaddleXPos + 6.5f;
                float xEndPanel = session.fPlayer1PaddleXPos - 6.5f;

                //player1 caught ball
                if ((xEndPanel <= newBallPos.X) && (newBallPos.X <= xStartPanel))
                {
                    //adjust x-direction
                    float xLengthBallHitPos = (newBallPos.X - xStartPanel) + 6.5f;

                    //               = percentage where ball hit paddle * ~70 degree
                    newBallDirSpeed.X = (xLengthBallHitPos / 6.5f) * (float)(Math.PI / 5.5);

                    //adjust z-direction and set ball out ouf collision zone
                    newBallDirSpeed.Z *= -1.0f;
                    newBallPos.Z = 24.0f;
                }
                //player1 missed ball
                else
                {
                    //session.fPlayer1PaddleXPos = 0.0f;

                    newBallPos.X = 0.0f;
                    newBallDirSpeed.X = 0.0f;

                    newBallPos.Z = -24.0f;
                }
            }
            //player2 must react
            else if (newBallPos.Z < -24.0f)
            {

                float xStartPanel = session.fPlayer2PaddleXPos + 6.5f;
                float xEndPanel = session.fPlayer2PaddleXPos - 6.5f;

                //player2 caught ball
                if ((xEndPanel <= newBallPos.X) && (newBallPos.X <= xStartPanel))
                {
                    //adjust x-direction
                    float xLengthBallHitPos = (newBallPos.X - xStartPanel) + 6.5f;

                    //               = percentage where ball hit paddle * ~70 degree
                    newBallDirSpeed.X = (xLengthBallHitPos / 6.5f) * (float)(Math.PI / 5.5);

                    //adjust z-direction and set ball out ouf collision zone
                    newBallDirSpeed.Z *= -1.0f;
                    newBallPos.Z = -24.0f;
                }
                //player2 missed ball
                else
                {
                    //session.fPlayer2PaddleXPos = 0.0f;

                    newBallPos.X = 0.0f;
                    newBallDirSpeed.X = 0.0f;

                    newBallPos.Z = 24.0f;
                }
            }

            session.ballPos = newBallPos;
            session.ballDirSpeed = newBallDirSpeed;
        }

        private static void ParseChangePaddlePos(netPongUDPParsePacketThreadParam threadParam)
        {
            netPongUdpGameSession gameSession;

            if (threadParam.gameSessionIndex >= m_runningSessions.Count)
                return;

            lock (m_runningSessionsLock)
            {
                gameSession = m_runningSessions[threadParam.gameSessionIndex];
            }

            if (threadParam.packetData[3] == 0x1)
            {
                gameSession.fPlayer1PaddleXPos = (float)BitConverter.ToSingle(threadParam.packetData, 4);
                lock (m_runningSessionsLock)
                {
                    m_runningSessions[threadParam.gameSessionIndex] = gameSession;
                }
                SendSetPaddlePosCommand(gameSession.fPlayer1PaddleXPos,gameSession.epPlayer2);
            }
            else if (threadParam.packetData[3] == 0x2)
            {
                gameSession.fPlayer2PaddleXPos = (-1.0f) * (float)BitConverter.ToSingle(threadParam.packetData, 4);
                lock (m_runningSessionsLock)
                {
                    m_runningSessions[threadParam.gameSessionIndex] = gameSession;
                }
                SendSetPaddlePosCommand(gameSession.fPlayer2PaddleXPos, gameSession.epPlayer1);
            }
        }

        public static void SendSetPaddlePosCommand(float fPaddlePos, IPEndPoint reciever)
        {
            byte[] bytePacket = new byte[8];
            byte[] byteArgLen = BitConverter.GetBytes((Int16)8);
            byte[] bytePaddlePos = BitConverter.GetBytes(fPaddlePos);
            bytePacket[0] = byteArgLen[0];
            bytePacket[1] = byteArgLen[1];
            bytePacket[2] = (byte)netPongUdpOpCodes.NP_UDP_CMD_CHANGE_PADDLE_POS;
            bytePacket[3] = 0x00;

            for (int i = 0; i < bytePaddlePos.Length; i++)
                bytePacket[i + 4] = bytePaddlePos[i];

            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient.BeginSend(bytePacket, bytePacket.Length, reciever, new AsyncCallback(SendSetPaddlePosCallback), null);
                }
                catch (Exception)
                {
                }
            }
        }

        private static void SendSetPaddlePosCallback(IAsyncResult ar)
        {
            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient.EndSend(ar);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
