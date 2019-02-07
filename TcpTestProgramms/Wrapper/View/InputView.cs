using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace Wrapper.View
{
    public class InputView : IInputView
    {
        public const int DEFAULT_POSITION_X = 0;
        public const int DEFAULT_POSITION_Y = 0;
        public const int DEFAULT_CURSOR_POSITION = 30;
        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;
        public int _xCursorPosition { get; set; }
        public bool ViewEnabled { get; set; } = true;

        private string _inputLine;


        public InputView(IOutputWrapper outputWrapper, int posX, int posY, int cursorPosition)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
            if (_inputLine == null || _inputLine == "")
            _inputLine = "Type /search to find servers.";
            _xCursorPosition = cursorPosition;
            
        }

        public InputView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y, DEFAULT_CURSOR_POSITION)
        { }

        public void SetInputLine(string inputLine, int xCursorPosition)
        {
            _inputLine = inputLine;
            _xCursorPosition = xCursorPosition;
            //set
        }
      
        public void Show()
        {           
            _outputWrapper.WriteOutput(_posX, _posY, _inputLine, ConsoleColor.Gray);
        }
    }
}

