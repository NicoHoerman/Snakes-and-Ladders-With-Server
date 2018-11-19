using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wrapper.Contracts;

namespace Wrapper.Implementation
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
            Console.ForegroundColor = ConsoleColor.White;
        }



        public void Updateview(string input, string afterConectMsg,string servertable,string errorMsg
            ,string updatedView)
        {
            Clear();

            //if (errorMsg.Length != 0)
            //    ErrorMsg(input,errorMsg);
            if (servertable.Length !=0)
                UpdatePreLobby(servertable);
            if (afterConectMsg.Length != 0)
                OnServerSelection(afterConectMsg);
            if (updatedView.Length != 0)
                ServerUpdate(updatedView);

            FirstLine();

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


            WriteOutput(0,4, tableHeader,ConsoleColor.Blue);
            WriteOutput(0, 5, serverTable, ConsoleColor.Blue);
        }

        private void OnServerSelection(string msg)
        {
            string message = msg;
            string tableHeader = string.Format("{2,3} {0,6}  {1,6}\n", "Player", "Server", "Key");

            WriteOutput(0, 1, message, ConsoleColor.DarkBlue);

            WriteOutput(0, 4, tableHeader, ConsoleColor.Blue);
            WriteOutput(0, 5, _Memory, ConsoleColor.Blue);
        }

        private void ErrorMsg(string input, string errorMsg)
        {
            string lastinput = "Last Input: " + input;

            WriteOutput(0, 15, lastinput, ConsoleColor.DarkRed);
            WriteOutput(0, 16, errorMsg, ConsoleColor.Red);

        }

        private void ServerUpdate(string updateview )
        {
            Clear();
            WriteOutput(0, 3, updateview,ConsoleColor.White);
        }
    }
}
