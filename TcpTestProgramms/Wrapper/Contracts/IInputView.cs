using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper.Contracts
{
    public interface IInputView : IView
    {
        int _xCursorPosition { get; set; }
        void SetInputLine(string inputLine, int xCursorPosition);
    }
}
