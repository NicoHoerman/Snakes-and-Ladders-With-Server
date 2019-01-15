using Shared.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Communications
{

    public class CommunicationPackage : IPackage
    {
        private static int s_idCounter;
        public int Id { get; }
        public DataPackage data { get; set; }
        public ICommunication communication { get; set; }

        public CommunicationPackage(ICommunication _communication, DataPackage _data)
        {
            Id = s_idCounter;
            s_idCounter++;
            communication = _communication;
            data = _data;
        }
    }
}
