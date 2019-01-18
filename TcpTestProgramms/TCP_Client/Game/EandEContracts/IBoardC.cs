using System.Collections.Generic;

namespace TCP_Client.Game.EandEContracts
{
    public interface IBoard
    {
        int Size { get; }
        int MaxWidth { get; }
        List<IPawn> Pawns { get; set; }
        List<IEntity> Entities { get; set; }

        string CreateOutput();
    }
}
