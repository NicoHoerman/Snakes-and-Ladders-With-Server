﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO.Ports;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const int PORT_NO = 8080;
            IPAddress SERVER_IP = IPAddress.Parse("127.0.0.1");

            var Server = new UdpClient(PORT_NO);
            var ResponseData = Encoding.ASCII.GetBytes("nichts bei dir?");

            while (true)
            {
                var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                var ClientRequestData = Server.Receive(ref ClientEp);
                var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

                Console.WriteLine("Recived:  {0} from:  {1}, sending response", ClientRequest, ClientEp.Address.ToString());
                Server.Send(ResponseData, ResponseData.Length, ClientEp);
                Console.ReadKey();
            }



        }
    }
}
