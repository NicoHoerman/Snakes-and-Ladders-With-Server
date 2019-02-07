using TCP_Client.GameStuff.EandEContracts;

namespace TCP_Client.GameStuff.ClassicEandE
{

	public class ClassicEscalator : IEntity
    {
        public int Top_location { get; set; }
        public int Bottom_location { get; set; }

        public EntityType Type => EntityType.Escalator;

        public long Id { get; set; }

        public ClassicEscalator()
        {

        }

        public void SetPawn(IPawn pawn)
        {
          pawn.Location = Top_location;
        }

        public bool OnSamePositionAs(IPawn pawn)
        {
          return Bottom_location == pawn.Location ? true : false;
        }
    }
}
