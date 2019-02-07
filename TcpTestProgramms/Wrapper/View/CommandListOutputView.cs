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
    public class CommandListOutputView : IUpdateOutputView
    {
        public const int DEFAULT_POSITION_X = 60;
        public const int DEFAULT_POSITION_Y = 0;
        private string _content = string.Empty;
        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;

        public bool ViewEnabled { get; set; } = false;

        public CommandListOutputView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
        }

        public CommandListOutputView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }

        public void SetUpdateContent(string content)
        {
			if (!(content == null || content.Length == 0))
			{
				ViewEnabled = true;
				_content = content;
			}
        }

        public void Show()
        {
            
            _outputWrapper.WriteOutput(_posX, _posY, _content, ConsoleColor.Yellow);
        }
    }
}
