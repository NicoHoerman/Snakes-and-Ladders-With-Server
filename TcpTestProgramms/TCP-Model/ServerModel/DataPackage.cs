using EandE_ServerModel.ServerModel.ProtocolActionStuff;
using System;
using System.Linq;
using System.Text;

namespace EandE_ServerModel.ServerModel
{
    public class DataPackage
    {
        public Int32 Size { get; set; }
        public ProtocolActionEnum Header { get; set; }
        public string Payload { get; set; }

        public byte[] ToByteArray()
        {
            var bytes = Encoding.ASCII.GetBytes(Payload);
            Size = bytes.Length + 2 * sizeof(Int32);
            return BitConverter.GetBytes(Size)
                .Concat(BitConverter.GetBytes((Int32)Header)
                .Concat(bytes)).ToArray();
        }
    }
}
