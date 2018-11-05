using System;
using System.Linq;
using System.Text;

namespace TCP_Model
{
    public class DataPackage
    {
        public Int32 Size { get; set; }
        public ProtocolAction Header { get; set; }
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
