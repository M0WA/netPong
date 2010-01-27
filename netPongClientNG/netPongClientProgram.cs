using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace netPongClientNG
{
    struct netPongClientGlobals
    {
        static public netPongClientGameForm hGameForm = null;
        static public netPongClientMessageForm hMessageForm = null;
        static public netPongClientTcpSocket hTCPControlSock = null;
        static public netPongClientUdpGameSession hUDPGameSession = null;
        static public netPongClientLoginForm hLoginForm = null;
        static public netPongClientChatForm hChatForm = null;
        static public netPongClientKeySettings hKeySettings = null;

        static private Hashtable m_hKeyBindings = new Hashtable();
        static private object m_hKeyBindLock = new object();
        static public Hashtable hKeyBindings
        {
            get
            {
                Hashtable returnTable;
                lock (m_hKeyBindLock)
                {
                    returnTable = m_hKeyBindings;
                }
                return returnTable;
            }

            set
            {
                lock (m_hKeyBindLock)
                {
                    m_hKeyBindings = value;
                }
            }
        }
    }
    
    static class netPongClientProgram
    {
        static private bool m_bEndProgram = false;
        static private object m_objLockEndProgram = new object();

        static public bool bEndProgram
        {
            get
            {
                bool bReturn = false;
                lock (m_objLockEndProgram)
                {
                    bReturn = m_bEndProgram;
                }
                return bReturn;
            }

            set
            {
                lock (m_objLockEndProgram)
                {
                    m_bEndProgram = value;
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            netPongClientGlobals.hMessageForm = new netPongClientMessageForm();
            netPongClientGlobals.hMessageForm.Show();

            netPongClientGlobals.hChatForm = new netPongClientChatForm();
            netPongClientGlobals.hChatForm.Hide();

            netPongClientGlobals.hKeySettings = new netPongClientKeySettings();
            netPongClientGlobals.hKeySettings.Hide();
            netPongClientGlobals.hKeySettings.LoadKeySettings();
            netPongClientGlobals.hKeySettings.InitKeyList();

            netPongClientGlobals.hLoginForm = new netPongClientLoginForm();
            netPongClientGlobals.hLoginForm.Show();

            netPongClientGlobals.hGameForm = new netPongClientGameForm();
            netPongClientGlobals.hGameForm.InitGame();
            netPongClientGlobals.hGameForm.Show();

            while (!bEndProgram)
            {
                Application.DoEvents();
                if (netPongClientGlobals.hGameForm.Focused)
                    netPongClientGlobals.hGameForm.Work();
                else
                {
                    netPongClientGlobals.hGameForm.Work();
                    Thread.Sleep(10);
                }
            }

            netPongClientGlobals.hGameForm.Shutdown();
        }
    }
}