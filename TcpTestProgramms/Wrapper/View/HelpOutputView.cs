using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.Implementation;
using System.Threading;

namespace Wrapper.View
{
    public class HelpOutputView : IHelpOutputView
    {
        public const int DEFAULT_POSITION_X = 70;
        public const int DEFAULT_POSITION_Y = 20;
        private string _help = string.Empty;
        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;

        public bool viewEnabled { get ; set ; }

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
            //set
        }

        public void Show()
        {
            
            _outputWrapper.WriteOutput(_posX, _posY, _help, ConsoleColor.Yellow);
        }
    }
}
