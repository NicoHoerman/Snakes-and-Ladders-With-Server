using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCP_Model.ServerModel
{
    public class OutputWrapper : IOutputWrapper
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


        public void UpdatePreLobby(StringBuilder serverTable)
        {
            string tableHeader = string.Format("{0,6}  {1,6}\n", "Player", "Server");
            
            WriteOutput(0,3, tableHeader,ConsoleColor.Blue);
            WriteOutput(0, 4, serverTable.ToString(), ConsoleColor.Blue);
        }

        public void ShowSomething()
        {
            Clear();

            string header = "Type /search for Servers";
            WriteOutput(0, 0, header, ConsoleColor.DarkBlue);

        }
    }
}
