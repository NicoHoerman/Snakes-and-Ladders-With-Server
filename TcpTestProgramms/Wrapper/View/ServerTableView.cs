using System;
using Wrapper.Contracts;

namespace Wrapper.View
{
    public class ServerTableView : IView
    {
        public const int DEFAULT_POSITION_X = 20;
        public const int DEFAULT_POSITION_Y = 10;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;

        public ServerTableView()
        {

        }

        public void Show()
        {
            throw new NotImplementedException();
        }
    }


}

