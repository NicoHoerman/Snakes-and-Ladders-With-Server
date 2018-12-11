using System;

namespace EandE_ServerModel.EandE.EandEContracts
{
    public interface ISourceWrapper
    {
        void WriteOutput(int x, int y, string output, ConsoleColor color = ConsoleColor.White);
        string ReadInput();
        void Clear();
        ConsoleKeyInfo ReadKey();
    }
}
