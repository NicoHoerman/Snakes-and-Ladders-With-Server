using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace Wrapper.View
{
    public class MasterCommandsView : IView
    {
        public const int DEFAULT_POSITION_X = 20;
        public const int DEFAULT_POSITION_Y = 10;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;

        public void Show()
        {
            throw new NotImplementedException();
        }
    }
}
