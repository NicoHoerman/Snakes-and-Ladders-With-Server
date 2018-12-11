using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper.Contracts
{
    public interface IBoardOutputView : IView
    {
        void SetUpdateContent(string content);
    }
}
