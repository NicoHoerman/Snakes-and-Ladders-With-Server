using System;
using System.ComponentModel;
using TCP_Server.Test;
using TCP_Server.UDP;

namespace TCP_Server
{
    class Program
    {
    
        static void Main(string[] args)
        {
            Core core = new Core();
            core.Start();
            
        }
    }
}
