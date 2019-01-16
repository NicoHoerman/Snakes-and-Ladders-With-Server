
namespace EandE_ServerModel.EandE.EandEContracts
{
    public interface IGame
    {
        IRules Rules { get; }
        IBoard Board { get; set; }
    }
}
