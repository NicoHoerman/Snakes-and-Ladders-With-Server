namespace TCP_Client.GameStuff.EandEContracts
{
    public enum EntityType
    {
        Eel,
        Escalator,
        Pawn,
    }
    
    public interface IEntity
    {
        int Top_location { get; set; }
        int Bottom_location{ get; set; }
        EntityType Type { get; }
        long Id { get; }

        void SetPawn(IPawn pawn);
        bool OnSamePositionAs(IPawn pawn);
    }
}
