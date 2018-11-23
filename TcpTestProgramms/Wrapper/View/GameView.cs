using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.Implementation;


namespace Wrapper.View
{
    public class GameView : IUpdateOutputView
    {
        public const int DEFAULT_POSITION_X = 0;
        public const int DEFAULT_POSITION_Y = 5;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;
        private string _gameInfo;

        public bool viewEnabled { get ; set ; }

        public GameView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
        }

        public GameView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }
        public void SetUpdateContent(string gameUpdate)
        {
            _gameInfo = gameUpdate;
        }


        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _gameInfo, ConsoleColor.Green);
        }
    }
}
