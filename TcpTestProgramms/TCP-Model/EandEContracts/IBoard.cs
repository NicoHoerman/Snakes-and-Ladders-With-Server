using System;
using System.Collections.Generic;
using System.Text;

namespace TCP_Model.EandEContracts
{
    public interface IBoard
    {
        int size { get; }
        int MaxWidth { get; }
        List<IPawn> Pawns { get; set; }
        List<IEntity> Entities { get; set; }

        string CreateOutput();
    }
}
