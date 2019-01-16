using System;
using System.Linq;
using EandE_ServerModel.EandE.EandEContracts;

namespace EandE_ServerModel.EandE.GameAndLogic
{

    public class Logic
    {
        public IPawn _currentPawn;
        private bool _gameFinished;
        public int _numberOfPlayers;
        public int CurrentPlayerID { get; set; } = 1;
        
        private readonly IGame _game;
        public Logic(IGame game)
        {
            _game = game;
        }

        //Gets the Pawn with the matching playerID from the Pawns List
        public void GetPawn()
        {
            try
            {
                 _currentPawn =  _game.Board.Pawns.Find(x => x.PlayerID.Equals(CurrentPlayerID));
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"Nothing Found with PlayerID {CurrentPlayerID} ",e);
            }
        }

        public TurnState CheckIfGameFinished()
        {
            if (_currentPawn.Location == _game.Board.Size)
                _gameFinished = true;
            else
                _gameFinished = false;

            if (_gameFinished == true)
                return TurnState.GameFinished;
            else
                return TurnState.TurnFinished;
        }

        public void NextPlayer()
        {
            var orderedPlayers = _game.Board.Pawns.OrderBy(x => x.PlayerID).ToList();

            if (_numberOfPlayers == 0)
                _numberOfPlayers = orderedPlayers[orderedPlayers.Count - 1].PlayerID;

            var nextPlayer = orderedPlayers.Where(x => x.PlayerID == CurrentPlayerID + 1).FirstOrDefault();
            if (nextPlayer == null)
                CurrentPlayerID = orderedPlayers.First().PlayerID;
            else
                CurrentPlayerID = nextPlayer.PlayerID;
        }

        public TurnState MakeTurn()
        {
            try
            {
				//
                //Get current Pawn 
                GetPawn();
				//Server
                //Roll Dice
                _game.Rules.RollDice();

				//Server
                //Check If Player Exceeds Board and Moves Pawn
                if (_currentPawn.Location + _game.Rules.DiceResult > _game.Board.Size)
                {
                    NextPlayer();
                    return TurnState.PlayerExceedsBoard;
                }
                else
					//Client
                    _currentPawn.MovePawn(_game.Rules.DiceResult);
                
				//Client
                //Entities check if the pawn is on them
                _game.Board.Entities.ForEach(entity =>
                {
                    if (entity.OnSamePositionAs(_currentPawn))
                    {
                        entity.SetPawn(_currentPawn);
                    }
                });

				//Server
                NextPlayer();
				//Server
                var currentState = CheckIfGameFinished();

                return currentState;
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"Could not Return a TurnState",e);
            }
        }
    }
}
