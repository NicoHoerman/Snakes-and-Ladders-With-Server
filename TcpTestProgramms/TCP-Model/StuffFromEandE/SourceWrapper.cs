using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCP_Model.EandEContracts;

namespace TCP_Model.StuffFromEandE
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
