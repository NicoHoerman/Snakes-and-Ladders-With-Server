using System;
using System.Collections.Generic;
using System.Text;

namespace EelsAndEscalators.Contracts
{ 
    public interface ISourceWrapper
    {
        void WriteOutput(int x, int y, string output, ConsoleColor color = ConsoleColor.White);
        string ReadInput();
        void Clear();
        ConsoleKeyInfo ReadKey();
    }
}
