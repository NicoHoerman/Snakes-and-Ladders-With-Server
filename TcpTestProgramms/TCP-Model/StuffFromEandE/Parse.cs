using TCP_Model.EandEContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace TCP_Model.StuffFromEandE
{
    public class Parse
    {
        private Dictionary<string, Action> _commandList;
        private Action<string> _errorAction = s => { };

        public Parse()
        {
            _commandList = new Dictionary<string, Action>();
        }



        public void AddCommand(string token, Action command) => _commandList.Add(token, command);

        public void SetErrorAction(Action<string> action) => _errorAction = action;

        public void Execute(string token)
        {
            if (_commandList.TryGetValue(token.ToLower(), out var function) == false)
                _errorAction(token);
            else
                function();
        }
    }
}
