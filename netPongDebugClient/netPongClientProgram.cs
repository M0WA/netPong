using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security.Permissions;

namespace netPongDebugClient
{
    static class netPongClientProgram
    {
        public static netPongClient m_cNetClient = null;
        public static netPongClientSettingsForm m_cSettingsForm = null;
        public static netPongGameForm m_hGameForm = null;
        public static netClientTcpSocket m_hTcpSocket = null;
        public static netPongClientChatForm m_chatWindow = null;
        public static netPongUdpGameClientSession m_udpGameSession = null;
        private static bool m_bIsRunning = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            m_cSettingsForm = new netPongClientSettingsForm();
            m_cSettingsForm.Show();

            m_bIsRunning = true;

            while (m_bIsRunning)
            {
                Application.DoEvents();
                if ((m_hGameForm != null) && m_hGameForm.Created)
                {
                    m_hGameForm.Work();                    
                }
                else
                {
                    System.Threading.Thread.Sleep(38);
                }
            }
        }

        public static void ExitNetPongClient()
        {
            m_bIsRunning = false;

            /*
            if (m_hGameForm != null)
                m_hGameForm.Close();
             */

            if (m_chatWindow != null)
                m_chatWindow.Close();

        }
    }
}