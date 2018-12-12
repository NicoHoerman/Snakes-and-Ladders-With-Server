using EandE_ServerModel.EandE.EandEContracts;
using System;
using System.Linq;

namespace EandE_ServerModel.EandE.StuffFromEandE
{
    public class SourceWrapper : ISourceWrapper
    {
        public void Clear()
        {
            Console.Clear();
        }

        public string ReadInput()
        {
            return Console.ReadLine();
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        public void WriteOutput(int x, int y, string output, ConsoleColor color)
        {
            var offsetY = 0;
            output.Split('\n').ToList().ForEach(line =>
            {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(x, y + offsetY);
                Console.WriteLine(line);
                offsetY++;
            });
        }

    }
}
