using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Windows.Forms;

namespace netPongDebugClient
{
    class netPongClient
    {        
        public netPongClient(string strUsername, string strServerIP, int nServerPort)
        {
            netPongClientProgram.m_hTcpSocket = new netClientTcpSocket(strUsername, strServerIP, nServerPort);
        }

        public void StartGameSession()
        {
            Thread thread = new Thread(new ThreadStart(StartSessionThread));
            //thread.IsBackground = true;
            thread.Start();
        }

        private static void StartSessionThread()
        {
            if (netPongClientProgram.m_hGameForm == null)
                netPongClientProgram.m_hGameForm = new netPongGameForm();

            if (netPongClientProgram.m_hGameForm.InvokeRequired)
            {

                netPongClientProgram.m_hGameForm.Invoke(new MethodInvoker(netPongClientProgram.m_hGameForm.InitGame));
                netPongClientProgram.m_hGameForm.Invoke(new MethodInvoker(netPongClientProgram.m_hGameForm.Show));
            }
            else
            {
                netPongClientProgram.m_hGameForm.InitGame();
                netPongClientProgram.m_hGameForm.Show();
            }
        }
    }    
}
