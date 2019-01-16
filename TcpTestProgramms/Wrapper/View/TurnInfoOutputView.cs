
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace Wrapper.View
{
    public class TurnInfoOutputView : IUpdateOutputView
    {
        public const int DEFAULT_POSITION_X = 27;
        public const int DEFAULT_POSITION_Y = 20;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;

        private string _content;
        public bool ViewEnabled { get; set; }

        public TurnInfoOutputView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
        }
        public TurnInfoOutputView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }

        public void SetUpdateContent(string content)
        {
            _content = content;
        }
        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _content, ConsoleColor.Gray);
        }
    }
}
