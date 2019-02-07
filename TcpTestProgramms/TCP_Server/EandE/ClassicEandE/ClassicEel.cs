using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.ClassicEandE
{
    public class ClassicEel : IEntity
    {
        public int Top_location { get ;set; }
        public int Bottom_location { get; set; }
        public EntityType Type => EntityType.Eel;

        public long Id { get; set; }

        public void SetPawn(IPawn pawn)
        {
            pawn.Location = Bottom_location;
        }

        public bool OnSamePositionAs(IPawn pawn)
        {
            return Top_location == pawn.Location ? true : false;
        }
    }
}
