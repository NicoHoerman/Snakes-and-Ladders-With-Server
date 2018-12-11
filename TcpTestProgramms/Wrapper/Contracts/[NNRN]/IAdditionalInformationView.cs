using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;

namespace Wrapper.View
{
    public interface IAdditionalInformationView : IView
    {
        void SetUpdateContent(string content);
    }
}
