using System;
using System.Collections;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.DirectX;

namespace netPongClientNG
{
    class netPongClientUdpGameSession
    {
        public enum netPongUdpOpCodes
        {
            NP_UDP_CMD_READY = 0x01,
            NP_UDP_CMD_CHANGE_PADDLE_POS = 0x02,
            NP_UDP_CMD_SET_BALL_POS = 0x03
        };

        private Int32 m_nSessionID = -1;
        private byte[] m_byteSessionID;
        private byte m_nPlayerID = 0;
        private IPEndPoint m_nServerEP = null;
        private UdpClient m_udpClient = null;
        private object m_udpClientLock = new object();

        public netPongClientUdpGameSession(Int32 nSessionID, byte nPlayerID, IPEndPoint server)
        {
            m_nSessionID = nSessionID;
            m_byteSessionID = BitConverter.GetBytes((Int32)m_nSessionID);
            m_nPlayerID = nPlayerID;
            m_nServerEP = server;

            CreateUDPClient();
            SendReadyCommand();
        }

        private void CreateUDPClient()
        {
            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient = new UdpClient(m_nServerEP.Address.ToString(), m_nServerEP.Port);
                    m_udpClient.BeginReceive(new AsyncCallback(ReadCallback), null);
                }
                catch (Exception)
                {
                }
            }
        }

        private void SendReadyCommand()
        {
            byte[] bytePacket = new byte[8];
            byte[] byteArgLen = BitConverter.GetBytes((Int16)8);
            byte[] byteSessionID = BitConverter.GetBytes((Int32)m_nSessionID);
            bytePacket[0] = byteArgLen[0];
            bytePacket[1] = byteArgLen[1];
            bytePacket[2] = (byte)netPongUdpOpCodes.NP_UDP_CMD_READY;
            bytePacket[3] = m_nPlayerID;

            for (int i = 0; i < byteSessionID.Length; i++)
                bytePacket[i + 4] = byteSessionID[i];

            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient.BeginSend(bytePacket, bytePacket.Length, new AsyncCallback(SendReadyCallback), null);
                }
                catch (Exception)
                {
                }
            }
        }

        private void SendReadyCallback(IAsyncResult ar)
        {
            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient.EndSend(ar);
                    netPongClientGlobals.hMessageForm.strLogString += ("READY Command sent\r\n");
                }
                catch (Exception)
                {
                }
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            byte[] data;
            lock (m_udpClientLock)
            {
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    data = m_udpClient.EndReceive(ar, ref sender);
                    HandlePacket(data, sender);
                }
                catch (Exception)
                {
                }

                m_udpClient.BeginReceive(new AsyncCallback(ReadCallback), null);
            }
        }

        private void HandlePacket(byte[] bytePacket, IPEndPoint sender)
        {
            if (bytePacket.Length < 3)
                return;

            int nOpCode = (int)bytePacket[2];
            switch (nOpCode)
            {
                case (int)netPongUdpOpCodes.NP_UDP_CMD_SET_BALL_POS:
                    ParseSetBallPosition(bytePacket);
                    break;
                case (int)netPongUdpOpCodes.NP_UDP_CMD_CHANGE_PADDLE_POS:
                    ParseChangePaddlePosition(bytePacket);
                    break;
                default:
                    break;
            }
        }

        private void ParseSetBallPosition(byte[] packet)
        {
            if (packet.Length != 11)
                return;

            float fBallXPos = BitConverter.ToSingle(packet, 3);
            float fBallZPos = BitConverter.ToSingle(packet, 7);

            if (m_nPlayerID == 2)
            {
                fBallXPos *= -1;
                fBallZPos *= -1;
            }

            if (netPongClientGlobals.hGameForm != null)
                netPongClientGlobals.hGameForm.MoveBall(new Vector3(fBallXPos, 0.0f, fBallZPos));
        }

        public void SendSetPaddlePosCommand(float fPaddlePos)
        {
            byte[] bytePacket = new byte[8];
            byte[] byteArgLen = BitConverter.GetBytes((Int16)8);
            byte[] bytePaddlePos = BitConverter.GetBytes(fPaddlePos);
            bytePacket[0] = byteArgLen[0];
            bytePacket[1] = byteArgLen[1];
            bytePacket[2] = (byte)netPongUdpOpCodes.NP_UDP_CMD_CHANGE_PADDLE_POS;
            bytePacket[3] = m_nPlayerID;

            for (int i = 0; i < bytePaddlePos.Length; i++)
                bytePacket[i + 4] = bytePaddlePos[i];

            lock (m_udpClientLock)
            {
                try
                {
                    m_udpClient.BeginSend(bytePacket, bytePacket.Length, new AsyncCallback(SendSetPaddlePosCallback), null);
                }
                catch (Exception)
                {
                }
            }
        }

        private void SendSetPaddlePosCallback(IAsyncResult ar)
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

        private void ParseChangePaddlePosition(byte[] bytePacket)
        {
            netPongClientGlobals.hGameForm.SetOpponentPaddlePos((float)BitConverter.ToSingle(bytePacket, 4));
        }
    }
}
