using Shared.Enums;
using System;
using System.Linq;
using System.Text;

namespace Shared.Communications
{
    public class DataPackage
    {
        public Int32 Size { get; set; }
        public ProtocolActionEnum Header { get; set; }
        public string Payload { get; set; }


        //ASCII
        public byte[] ToByteArray()
        {
            var bytes = Encoding.ASCII.GetBytes(Payload);
            Size = bytes.Length + 2 * sizeof(Int32);
            return BitConverter.GetBytes(Size)
                .Concat(BitConverter.GetBytes((Int32)Header)
                .Concat(bytes)).ToArray();
        }

        public byte[] ToByteArrayUTF()
        {
            var bytes = Encoding.UTF8.GetBytes(Payload);
            Size = bytes.Length + 2 * sizeof(Int32);
            return BitConverter.GetBytes(Size)
                .Concat(BitConverter.GetBytes((Int32)Header)
                .Concat(bytes)).ToArray();
        }


    }
}
