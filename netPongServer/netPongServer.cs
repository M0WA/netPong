using System;
using System.Text;

namespace netPongServer
{
    public static class netPongServer
    {
        public static void StartServer(string strServerIP, int nServerPort)
        {
            netPongServerTcpSocket.StartListening(strServerIP, nServerPort);
        }
    }

     
}


