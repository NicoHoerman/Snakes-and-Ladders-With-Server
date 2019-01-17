using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.StuffFromEandE;
using System;
using System.Diagnostics;

namespace EandE_ServerModel.EandE.States
{
	public class GameFinishedState : IState
    {
        private readonly IGame _game;
        private readonly ISourceWrapper _sourceWrapper;
        public bool _isFinished;

        #region Properties
        public int CurrentPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int LastPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string TurnStateProp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		#endregion

		public GameFinishedState(IGame game, ISourceWrapper sourceWrapper)
        {
            _game = game;
            _sourceWrapper = sourceWrapper;
            _isFinished = true;
        }

        public GameFinishedState(IGame game)
            : this(game, new SourceWrapper())
        { }

        public void Execute()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (_isFinished)
            {
                if (stopwatch.ElapsedMilliseconds > 1000 * 15)
                {
                    _isFinished = false;
                    _game.SwitchState(new GameEndingState(_game));
                }
            }
        }

        public void ExecuteStateAction(string input)
		{
			throw new NotImplementedException();
		}
	}
}
