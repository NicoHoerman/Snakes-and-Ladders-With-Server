
namespace EandE_ServerModel.EandE.EandEContracts
{
    public enum EntityType
    {
        Eel,
        Escalator,
        Pawn,
    }
    
    public interface IEntity
    {
        int top_location { get; set; }
        int bottom_location{ get; set; }
        EntityType type { get; }
        long Id { get; }

        void SetPawn(IPawn pawn);
        bool OnSamePositionAs(IPawn pawn);
    }
}
