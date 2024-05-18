using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Socket2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Server or client? (S or C): ");
            string input = Console.ReadLine();
            if (input == "S")
            {
                Server server = new Server();
            }
            else if (input == "C")
            {
                Client client = new Client();
            }
        }
    }
}