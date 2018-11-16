using Newtonsoft.Json;
using Shared.Communications;
using Shared.Contract;
using Shared.Contracts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using TCP_Server.PROTOCOLS;

namespace TCP_Server.Actions
{
    public class ServerActions
    {
        private Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>> _protocolActions;


        public ServerActions()
        {
            _protocolActions = new Dictionary<ProtocolActionEnum, Action<ICommunication, DataPackage>>
            {
                { ProtocolActionEnum.RollDice,  OnRollDiceAction },
                { ProtocolActionEnum.GetHelp,   OnGetHelpAction },
                { ProtocolActionEnum.StartGame, OnStartGameAction },
                { ProtocolActionEnum.CloseGame, OnCloseGameAction }
            };
        }

        public void ExecuteDataActionFor(ICommunication communication, DataPackage data)
        {
            if (_protocolActions.TryGetValue(data.Header, out var protocolAction) == false)
                throw new InvalidOperationException("Invalid communication");

            protocolAction(communication, data);
        }

        #region Protocol actions
        private void OnRollDiceAction(ICommunication communication, DataPackage data)
        {

            var dataPackage = new DataPackage
            {

                Header = ProtocolActionEnum.UpdateView,
                Payload = JsonConvert.SerializeObject(new PROT_UPDATE
                {
                    _Updated_board = "Test: XD",
                    _Updated_dice_information = "You rolled a 4",
                    _Updated_turn_information = "Its Player 1's turn",
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnGetHelpAction(ICommunication communication, DataPackage data)
        {

            var clientId = CreateProtocol<PROT_HELPTEXT>(data);

            var dataPackage = new DataPackage
            {
                Header = ProtocolActionEnum.HelpText,
                Payload = JsonConvert.SerializeObject(new PROT_HELPTEXT
                {
                    _Text = $"Here is your help Player {clientId._Text}.\n " +
                    $"Commands:\n/rolldice\n/closegame\n"
                })
            };
            dataPackage.Size = dataPackage.ToByteArray().Length;

            communication.Send(dataPackage);
        }

        private void OnCloseGameAction(ICommunication arg1, DataPackage arg2)
        {
            throw new NotImplementedException();
        }

        private void OnStartGameAction(ICommunication arg1, DataPackage arg2)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Static helper functions

        private static T CreateProtocol<T>(DataPackage data) where T : IProtocol
        {
            return JsonConvert.DeserializeObject<T>(data.Payload);
        }

        #endregion
    }
}
