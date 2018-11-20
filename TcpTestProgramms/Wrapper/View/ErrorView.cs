using System;
using Wrapper.Implementation;
using Wrapper.Contracts;

namespace Wrapper.View
{
    public class ErrorView : IErrorView
    {
        public const int DEFAULT_POSITION_X = 20;
        public const int DEFAULT_POSITION_Y = 10;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;
        private string _lastInput = string.Empty;
        private string _errorMessage = string.Empty;

        public ErrorView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
        }

        public ErrorView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }


        public void SetContent(string lastInput, string errorMessage)
        {
            _lastInput = lastInput;
            _errorMessage = errorMessage;
        }


        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _lastInput, ConsoleColor.DarkRed);
            _outputWrapper.WriteOutput(_posX, _posY + 1, _errorMessage, ConsoleColor.Red);
        }
    }


}

