using System;
using System.Linq;
using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.GameAndLogic
{

    public class Logic
    {
        
        public IPawn CurrentPawn;
        private bool GameFinished;
        public int numberOfPlayers;
        public int CurrentPlayerID { get; set; } = 1;
        
        private readonly IGame _game;
        public Logic(IGame game)
        {
            _game = game;
            
        }

        //Gets the Pawn with the matching playerID from the Pawns List
        public IPawn GetPawn()
        {
            try
            {
                return CurrentPawn =  _game.Board.Pawns.Find(x => x.playerID.Equals(CurrentPlayerID));
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"Nothing Found with PlayerID {CurrentPlayerID} ",e);
            }

        }


        public TurnState CheckIfGameFinished(IPawn pawn)
        {
            try
            {
                pawn = CurrentPawn;

                if (pawn.location == _game.Board.size)
                    GameFinished = true;
                else
                    GameFinished = false;

                if (GameFinished == true)
                    return TurnState.GameFinished;
                else
                    return TurnState.TurnFinished;
                        

                //GameFinished = CurrentPawn.location == _game.Board.size ? true : false;
                //return GameFinished == true ? TurnState.GameFinished : TurnState.TurnFinished;
            }
            catch
            {
                throw new Exception();
            }

        }

        
        public void NextPlayer()
        {
            var orderedPlayers = _game.Board.Pawns.OrderBy(x => x.playerID).ToList();

            if (numberOfPlayers == 0)
                numberOfPlayers = orderedPlayers[orderedPlayers.Count - 1].playerID;

            var nextPlayer = orderedPlayers.Where(x => x.playerID == CurrentPlayerID + 1).FirstOrDefault();
            if (nextPlayer == null)
                CurrentPlayerID = orderedPlayers.First().playerID;
            else
                CurrentPlayerID = nextPlayer.playerID;
        }


        public TurnState MakeTurn()
        {

            try
            {
                //Get current Pawn 
                GetPawn();
                //Roll Dice
                _game.Rules.RollDice();

                //Check If Player Exceeds Board and Moves Pawn
                if (CurrentPawn.location + _game.Rules.DiceResult > _game.Board.size)
                {
                    NextPlayer();
                    return TurnState.PlayerExceedsBoard;
                }
                else
                    CurrentPawn.MovePawn(_game.Rules.DiceResult);
                
                //Entities check if the pawn is on them
                _game.Board.Entities.ForEach(entity =>
                {
                    if (entity.OnSamePositionAs(CurrentPawn))
                    {
                        entity.SetPawn(CurrentPawn);
                    }
                });

                
                NextPlayer();

                var CurrentState = CheckIfGameFinished(CurrentPawn);

                return CurrentState;

            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"Could not Return a TurnState",e);
            }

        }

    }
}
