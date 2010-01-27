using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.DirectX;

namespace netPongServer
{
    public struct netPongUdpGameSession
    {
        public int m_nSessionID;
        private string m_strPlayer1;
        private string m_strPlayer2;
        private IPEndPoint m_epPlayer1;
        private IPEndPoint m_epPlayer2;
        private DateTime m_initTime;
        private bool m_bIsInGame;
        private Thread m_hBallCalcThread;
        private Vector3 m_ballPos;
        private Vector3 m_ballDirSpeed;
        private DateTime m_lastBallPosCalculation;
        private float m_fPlayer1PaddleXPos;
        private float m_fPlayer2PaddleXPos;

        public netPongUdpGameSession(int nSessionID, string strPlayer1, string strPlayer2, IPEndPoint epPlayer1, IPEndPoint epPlayer2)
        {
            m_nSessionID = nSessionID;
            m_strPlayer1 = strPlayer1;
            m_strPlayer2 = strPlayer2;
            m_epPlayer1 = epPlayer1;
            m_epPlayer2 = epPlayer2;
            m_initTime = DateTime.Now;
            m_bIsInGame = false;
            m_hBallCalcThread = null;
            m_ballPos = new Vector3(0, 0, 0.0f);
            m_ballDirSpeed = new Vector3(0.0f, 0.0f, 0.05f);
            m_lastBallPosCalculation = DateTime.Now;
            m_fPlayer1PaddleXPos = 0.0f;
            m_fPlayer2PaddleXPos = 0.0f;
        }

        public int nSessionID
        {
            get { return m_nSessionID;  }
            set { m_nSessionID = value; }
        }

        public string strPlayer1
        {
            get { return m_strPlayer1; }
            set { m_strPlayer1 = value; }
        }

        public string strPlayer2
        {
            get { return m_strPlayer2; }
            set { m_strPlayer2 = value; }
        }

        public IPEndPoint epPlayer1
        {
            get { return m_epPlayer1; }
            set { m_epPlayer1 = value; }
        }

        public IPEndPoint epPlayer2
        {
            get { return m_epPlayer2; }
            set { m_epPlayer2 = value; }
        }

        public DateTime initTime
        {
            get { return m_initTime; }
            set { m_initTime = value; }
        }

        public bool bIsInGame
        {
            get { return m_bIsInGame; }
            set { m_bIsInGame = value; }
        }

        public Thread hBallCalcThread
        {
            get { return m_hBallCalcThread; }
            set { m_hBallCalcThread = value; }
        }

        public Vector3 ballPos
        {
            get { return m_ballPos; }
            set { m_ballPos = value; }
        }

        public Vector3 ballDirSpeed
        {
            get { return m_ballDirSpeed; }
            set { m_ballDirSpeed = value; }
        }

        public DateTime lastBallPosCalculation
        {
            get { return m_lastBallPosCalculation; }
            set { m_lastBallPosCalculation = value; }
        }

        public float fPlayer1PaddleXPos
        {
            get { return m_fPlayer1PaddleXPos; }
            set { m_fPlayer1PaddleXPos = value; }
        }

        public float fPlayer2PaddleXPos
        {
            get { return m_fPlayer2PaddleXPos; }
            set { m_fPlayer2PaddleXPos = value; }
        }
    }
}
