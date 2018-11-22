using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace Wrapper.View
{
    public class InfoOutputView : IInfoOutputView
    {
        public const int DEFAULT_POSITION_X = 70;
        public const int DEFAULT_POSITION_Y = 0;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;
        private string _info;

        public bool viewEnabled { get; set; }

        public InfoOutputView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
           
        }

        public InfoOutputView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }

        public void SetInfoContent(string info)
        {
            _info = info;
            //set
        }

        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _info, ConsoleColor.Blue);
        }
    }
}
