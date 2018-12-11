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
        public const string DEFAULT_CONTENT = "";

        private readonly IOutputWrapper _outputWrapper;       
        private string _serverTableContent;
        private string _memory;
        private int _posX;
        private int _posY;
        StringBuilder serverTableContent;

        public bool viewEnabled { get; set; } = false;
                              
        public ServerTableView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
            if(_serverTableContent == null || _serverTableContent.Length == 0)
            _serverTableContent = DEFAULT_CONTENT;
            serverTableContent = new StringBuilder();

        }

        public ServerTableView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }


        public void SetUpdateContent(string serverTable)
        {
            serverTableContent.Clear();
            string tableHeader = string.Format("{2,3} {0,6}  {1,6}\n", "Player", "Server", "Key");
            serverTableContent.Append(tableHeader);

            _serverTableContent =  serverTableContent.Append(serverTable).ToString();
            _memory = _serverTableContent;
        }

        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _memory, ConsoleColor.Blue);
            _serverTableContent = string.Empty;
        }
       
    }


}

