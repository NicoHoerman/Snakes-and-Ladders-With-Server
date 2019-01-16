using EandE_ServerModel.EandE.EandEContracts;
using EandE_ServerModel.EandE.StuffFromEandE;
using System;
using System.Diagnostics;
using TCP_Server.Actions;
using Shared.Contracts;
using Shared.Contract;
using TCP_Server.PROTOCOLS;
using TCP_Server.Enum;
using Shared.Communications;
using Shared.Enums;
using Newtonsoft.Json;

namespace EandE_ServerModel.EandE.States
{
    public class GameFinishedState : IState
    {
        private readonly IGame _game;
        private readonly ISourceWrapper _sourceWrapper;
        private readonly DataProvider _dataProvider;

        public bool _isFinished;

        #region Properties
        public string FinishInfo { get; set; } = string.Empty;
        public string Finishskull1 { get; set; } = string.Empty;
        public string Finishskull2 { get; set; } = string.Empty;
       
        public string MainMenuOuput { get; set; } = string.Empty;
        public string Lastinput { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string GameInfoOuptput { get; set; } = string.Empty;
        public string BoardOutput { get; set; } = string.Empty;
        public string TurnInfoOutput { get; set; } = string.Empty;
        public string AfterTurnOutput { get; set; } = string.Empty;
        public string HelpOutput { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        public int CurrentPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        public GameFinishedState(IGame game, ISourceWrapper sourceWrapper, DataProvider dataProvider)
        {
            _game = game;
            _sourceWrapper = sourceWrapper;
            _dataProvider = dataProvider;        
            _isFinished = true;
        }

        public GameFinishedState(IGame game)
            : this(game, new SourceWrapper(), new DataProvider())
        { }

        public void Execute()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (_isFinished)
            {
                if (stopwatch.ElapsedMilliseconds > 1000 * 10)
                {
                    _isFinished = false;
                    _game.SwitchState(new GameEndingState(_game));
                }
            }
        }
        public void ReactivateViews(ICommunication communication)
        {
            var reactivationPackage = new DataPackage
            {
                Header = ProtocolActionEnum.Restart,
                Payload = JsonConvert.SerializeObject(new PROT_RESTART
                {

                })
            };

            reactivationPackage.Size = reactivationPackage.ToByteArray().Length;

            communication.Send(reactivationPackage);
        }

		public void ExecuteStateAction(string input)
		{
			throw new NotImplementedException();
		}
	}
}
