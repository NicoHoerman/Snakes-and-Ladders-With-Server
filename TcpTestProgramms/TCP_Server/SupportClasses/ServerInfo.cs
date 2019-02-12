using Shared.Contract;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TCP_Server.Test;

namespace TCP_Server.Support
{
    public class ServerInfo
    {
        public List<ICommunication> _communications { get; set; }
        public List<ICommunication> _communicationsToRemove { get; set; }
        public List<Lobby> _lobbylist { get; set; }
		public IPAddress ServerIPAdress { get; set; }
        public ServerInfo()
        {
            _communications = new List<ICommunication>();
            _communicationsToRemove = new List<ICommunication>();
            _lobbylist = new List<Lobby>();
			ServerIPAdress = GetLocalIPAddress();
        }

        public void PrintPlayerIP()
        {
            for (int i = 0; i < _communications.Count; i++)
            {
                Console.WriteLine(((IPEndPoint)_communications[i]._client.Client.RemoteEndPoint).Address.ToString());
            }
        }

        public IPAddress GetPlayerIP(int communicationNr)
        {
            return ((IPEndPoint)_communications[communicationNr]._client.Client.RemoteEndPoint).Address;
        }

		private static IPAddress GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip;
				}
			}
			throw new Exception("No network adapters with an IPv4 address in the system!");
		}
	}
}
