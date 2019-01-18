
namespace TCP_Client.Game.EandEContracts
{

    public interface IPawn
    {
        int Location { get; set; }
        int Color { get; set; }
        int PlayerID { get; set; }
        long Id { get; set; }
        EntityType Type { get; }
        void MovePawn(int fieldsToMove);
    }
}
