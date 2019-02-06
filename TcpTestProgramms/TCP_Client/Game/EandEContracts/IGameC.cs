namespace TCP_Client.GameStuff.EandEContracts
{
    public interface IGame
    {
	    int LastPlayer { get; set; }
		string Turnstate { get; set; }
        IRules Rules { get; }
        IBoard Board { get; set; }
	}
}
