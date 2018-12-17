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
        

        public bool isFinished;
        public string _finishinfo = string.Empty;
        private string _finishskull1 = string.Empty;
        private string _finishskull2 = string.Empty;
        public int _winner;

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

        public GameFinishedState(IGame game, ISourceWrapper sourceWrapper, DataProvider dataProvider, int winner)
        {
            _game = game;
            _sourceWrapper = sourceWrapper;
            _dataProvider = dataProvider;        
            isFinished = true;
            _winner = winner;
        }

        public GameFinishedState(IGame game,int winner)
            : this(game, new SourceWrapper(), new DataProvider(),winner)
        { }

        public void Execute()
        {
            _finishinfo = string.Format(
                _dataProvider.GetText("playerwins"),
                _dataProvider.GetNumberLiteral(_winner));
            _finishskull1 = string.Format(
                _dataProvider.GetText("finishskull1"));
            _finishskull2 = string.Format(
                _dataProvider.GetText("finishskull2"));

            SaveProperties(_finishinfo,_finishskull1,_finishskull2);

            ServerActions.EndscreenSet.Set();
            while (isFinished)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (stopwatch.ElapsedMilliseconds > 5000)
                {
                    isFinished = false;
                    _game.SwitchState(new GameEndingState(_game));
                }
            }
        }

        public void ClearProperties()
        {
            FinishInfo = string.Empty;
            Finishskull1 = string.Empty;
            Finishskull2 = string.Empty;
        }

        public void SaveProperties(string _finshinfo,string _finishskull1,string _finishskull2)
        {
            FinishInfo = _finishinfo;
            Finishskull1 = _finishskull1;
            Finishskull2 = _finishskull2;
        }

        public void SetInput(string input)
        {
            Input = input;
        }

        public void reactivateViews(ICommunication communication)
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
    }
}
