using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;

namespace Wrapper.View
{
    public class GameView : IView
    {
        public const int DEFAULT_POSITION_X = 20;
        public const int DEFAULT_POSITION_Y = 10;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;

        public bool viewEnabled { get ; set ; }

        public void Show()
        {
            throw new NotImplementedException();
        }
    }
}
