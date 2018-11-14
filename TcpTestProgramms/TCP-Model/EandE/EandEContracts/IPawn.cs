
namespace EandE_ServerModel.EandE.EandEContracts
{

    public interface IPawn
    {
        int location { get; set; }
        int color { get; set; }
        int playerID { get; set; }
        long Id { get; set; }
        EntityType type { get; }
        void MovePawn(int fieldsToMove);
    }
}
