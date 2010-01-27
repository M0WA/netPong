using System;

namespace netPongServer
{
    class netPongServerProgram
    {

        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("|========================");
            Console.WriteLine("| netPong Server v1.0");
            Console.WriteLine("|========================");

            netPongUdpGameServer.StartServer("127.0.0.1", 2411);
            netPongServer.StartServer("127.0.0.1", 2410);
        }
    }
}
