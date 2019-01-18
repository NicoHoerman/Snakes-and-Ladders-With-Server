
namespace TCP_Client.Game.EandEContracts
{
    public interface IGame
    {
        IRules Rules { get; }
        IBoard Board { get; set; }
    }
}
