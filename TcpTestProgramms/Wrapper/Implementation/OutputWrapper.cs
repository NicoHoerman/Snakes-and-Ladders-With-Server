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
        private bool _ShiftPressed = false;           
        public string userInput = string.Empty;

        public void Clear()
        {
            Console.Clear();
        }

        public string ReadInput()
        {
             /*ConsoleKeyInfo cki = new ConsoleKeyInfo();
             userInput = string.Empty;

             while (cki.Key != ConsoleKey.Enter)
             {
                 cki = Console.ReadKey();

                 if ((cki.Modifiers & ConsoleModifiers.Shift) != 0)
                     _ShiftPressed = true;
                 if (_ShiftPressed == true & cki.Key == ConsoleKey.D7)
                     userInput += "/";
                 else if (cki.Key != ConsoleKey.Enter)
                     userInput += cki.Key.ToString();        

             }

             return userInput;*/
             
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

        public void FirstLine()
        {
            string header = "Type a command: ";
            WriteOutput(0, 0, header, ConsoleColor.Gray);
            Console.SetCursorPosition(30, 0);
        }      

        private void OnServerSelection(string msg)
        {
            
        }       

        private void ServerUpdate(string updateview)
        {
            Clear();
            WriteOutput(0, 3, updateview,ConsoleColor.White);
        }
    }
}
