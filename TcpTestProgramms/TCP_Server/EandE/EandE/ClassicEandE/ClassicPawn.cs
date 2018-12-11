using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.ClassicEandE
{

    public class ClassicPawn : IPawn
    {
        public int location { get; set; }
        public int color { get; set; }
        public int playerID { get; set; }
        public long Id { get; set; }
        public EntityType type => EntityType.Pawn;

        public ClassicPawn()
        {
        }

        public void MovePawn(int fieldsToMove)
        {
            location += fieldsToMove;
        }
    }
}
