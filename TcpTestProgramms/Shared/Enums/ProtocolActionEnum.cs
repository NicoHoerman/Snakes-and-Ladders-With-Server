
namespace Shared.Enums
{
    public enum ProtocolActionEnum
    {
        GetHelp = 101,
        RollDice = 102,
        StartGame = 103,
        Rule = 104,
        CloseGame = 105,
        OnConnection =106,
        ValidationAnswer = 107,

        Broadcast = 201,
        DeclineInfo = 202,
        AcceptInfo = 203,
        UpdateView = 204,
        HelpText = 205,
        Restart = 206,
        ValidationRequest = 207,
        ValidationAccepted = 208,
        LobbyCheckFailed = 209,
        LobbyCheckSuccessful = 210
    }
}
