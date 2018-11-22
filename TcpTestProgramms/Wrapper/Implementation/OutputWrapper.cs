using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wrapper.Contracts;
using Wrapper.View;
using Wrapper;

namespace Wrapper.Implementation
{
    public class OutputWrapper : IOutputWrapper
    {
        public List<IView> viewList;
        private string _Memory = string.Empty;
        private bool _ShiftPressed = false;           
        public string userInput = string.Empty;

        public void Clear()
        {
            Console.Clear();
        }

        public OutputWrapper()
        {
            viewList = new List<IView>()
            {
                new ErrorView(),
                new HelpOutputView(),
                new InfoOutputView(),
                new ServerTableView(),
                new InputView(),
                //new Game()
                //new MainMenu

            };
        }


        public void UpdateView()
        {
            viewList.ForEach(view =>
            {
                if (view.viewEnabled)
                {
                    view.Show();
                        
                }

            });
            
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
