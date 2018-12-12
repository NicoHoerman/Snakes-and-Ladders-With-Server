using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Implementation;
using Wrapper.Contracts;

namespace Wrapper.View
{
    public class FinishSkull1View : IUpdateOutputView
    {
        public const int DEFAULT_POSITION_X = 0;
        public const int DEFAULT_POSITION_Y = 0;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;

        private string _content;
        public bool viewEnabled { get; set; }

        public FinishSkull1View(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
        }
        public FinishSkull1View()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }

        public void SetUpdateContent(string content)
        {
            _content = content;
        }
        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _content, ConsoleColor.DarkYellow);
        }
    }
}
