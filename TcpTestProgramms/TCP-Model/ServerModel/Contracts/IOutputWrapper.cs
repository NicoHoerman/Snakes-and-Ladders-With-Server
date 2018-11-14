using System;

namespace TCP_Model.ServerModel
{
    public interface IOutputWrapper
    {
        void WriteOutput(int x, int y, string output, ConsoleColor color = ConsoleColor.White);
        string ReadInput();
        void Clear();
        ConsoleKeyInfo ReadKey();
    }
}