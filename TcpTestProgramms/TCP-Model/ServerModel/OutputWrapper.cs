using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCP_Model.ServerModel
{
    public class OutputWrapper : IOutputWrapper
    {

        private string _Memory = string.Empty;


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



        public void Updateview(string input, string afterConectMsg,string servertable)
        {
            Clear();
            FirstLine();

            if (input == string.Empty)
                return;

            if (input.All(char.IsDigit))
                input = "int";


            switch (input)
            {
                case "/search":
                    UpdatePreLobby(requiredstring);
                    break;
                case "int":
                    OnServerSelection(requiredstring);
                    break;
                default:
                    ErrorMsg(input);
                    break;
            }
            
        }

        private void FirstLine()
        {
            string header = "Type an Command: ";
            WriteOutput(0, 0, header, ConsoleColor.Gray);
            Console.SetCursorPosition(17, 0);
        }

        private void UpdatePreLobby(string serverTable)
        {
            string tableHeader = string.Format("{2,3} {0,6}  {1,6}\n", "Player", "Server","Key");
            _Memory = serverTable;


            WriteOutput(0,3, tableHeader,ConsoleColor.Blue);
            WriteOutput(0, 4, serverTable, ConsoleColor.Blue);
        }

        private void ErrorMsg(string input)
        {
            string lastinput = "Last Input: " + input;
            string error = "Error: " + "This command does not exist or isn't enabled at this time";

            WriteOutput(0, 15, lastinput, ConsoleColor.DarkRed);
            WriteOutput(0, 16, error, ConsoleColor.Red);

        }

        private void OnServerSelection(string msg)
        {
            string message = msg;
            string tableHeader = string.Format("{2,3} {0,6}  {1,6}\n", "Player", "Server", "Key");

            WriteOutput(0, 3, message, ConsoleColor.DarkBlue);

            WriteOutput(0, 4, tableHeader, ConsoleColor.Blue);
            WriteOutput(0, 5, _Memory, ConsoleColor.Blue);
        }
    }
}
