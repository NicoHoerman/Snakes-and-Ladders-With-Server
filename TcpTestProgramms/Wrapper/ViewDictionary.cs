using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.View;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace Wrapper
{
    public class ViewDictionary
    {
        
         
            public Dictionary<ClientView, IView> _views = new Dictionary<ClientView, IView>
            { 
            { ClientView.Error, new ErrorView() },
            { ClientView.ServerTable, new ServerTableView() },
            { ClientView.InfoOutput, new InfoOutputView() },
            { ClientView.CommandList, new CommandListOutputView() },
            { ClientView.Input, new InputView() },
            { ClientView.MenuOutput, new MainMenuOutputView() },
            { ClientView.TurnInfo, new TurnInfoOutputView() },
            { ClientView.GameInfo, new GameInfoOutputView() },
            { ClientView.AfterTurnOutput, new AfterTurnOutputView() },
            { ClientView.Board, new BoardOutputView() },
            { ClientView.LobbyInfoDisplay, new LobbyInfoDisplayView() },
            { ClientView.FinishInfo, new FinishInfoOutputView() },
            { ClientView.FinishSkull1, new FinishSkull1View() },
            { ClientView.FinishSkull2, new FinishSkull2View() }
            };
         
        
    }
}
