using System;
using System.Text;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace Wrapper.View
{
    public class ServerTableView : IUpdateOutputView
    {
        public const int DEFAULT_POSITION_X = 0;
        public const int DEFAULT_POSITION_Y = 5;
		public const string DEFAULT_STRING = "";

        private readonly IOutputWrapper _outputWrapper;       
        private string _serverTable;
        private string _memory;
        private int _posX;
        private int _posY;
        StringBuilder _serverTableContent;
		string _lobbyInfo;

        public bool ViewEnabled { get; set; } = false;
                              
        public ServerTableView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
			if (_serverTableContent == null || _serverTableContent.Length == 0)
				_serverTableContent = new StringBuilder("");
			_lobbyInfo = DEFAULT_STRING;
            _serverTableContent = new StringBuilder();

        }

        public ServerTableView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }


        public void SetUpdateContent(string serverTable)
        {
            _serverTableContent.Clear();
            string tableHeader = string.Format("{2,3} {0,6}  {1,6}\n", "Player", "Server", "Key");
            _serverTableContent.Append(tableHeader);

            _lobbyInfo =  _serverTableContent.Append(serverTable).ToString();
            _memory = _lobbyInfo;
        }

        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _memory, ConsoleColor.Blue);
            _lobbyInfo = string.Empty;
        }
    }
}

