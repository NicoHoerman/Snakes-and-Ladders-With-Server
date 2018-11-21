using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace Wrapper.View
{
    public class HelpOutputView : IView
    {
        public const int DEFAULT_POSITION_X = 35;
        public const int DEFAULT_POSITION_Y = 3;
        private string _help = string.Empty;
        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;

        public HelpOutputView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
        }

        public HelpOutputView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }

        public void SetHelp(string help)
        {
            _help = help;
        }

        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _help, ConsoleColor.Yellow);
        }
    }
}
