using System;

namespace Wrapper.Contracts
{
    public interface IOutputWrapper
    {
        void WriteOutput(int x, int y, string output, ConsoleColor color = ConsoleColor.White);
        string ReadInput();
        void Clear();
        ConsoleKeyInfo ReadKey();
    }
}