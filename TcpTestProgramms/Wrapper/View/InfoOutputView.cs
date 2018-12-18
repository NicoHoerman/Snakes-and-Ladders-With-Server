using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.Implementation;

namespace Wrapper.View
{
    public class InfoOutputView : IUpdateOutputView
    {
        public const int DEFAULT_POSITION_X = 0;
        public const int DEFAULT_POSITION_Y = 7;

        private readonly IOutputWrapper _outputWrapper;
        private int _posX;
        private int _posY;
        private string _content;
        private List<string> infoList;

        public bool viewEnabled { get; set; }

        public InfoOutputView(IOutputWrapper outputWrapper, int posX, int posY)
        {
            infoList = new List<string>();
            _outputWrapper = outputWrapper;
            _posX = posX;
            _posY = posY;
           
        }

        public InfoOutputView()
            : this(new OutputWrapper(), DEFAULT_POSITION_X, DEFAULT_POSITION_Y)
        { }

        public void SetUpdateContent(string content)
        {
            /*int x = 0;

            if(content != string.Empty)
                do
                {
                    infoList.Add(content);

                    if(infoList[x] != string.Empty)
                    {
                        _content = infoList[x];
                        x++;                   
                    }
                
                }
                while (infoList.Count <= 4);

            infoList.RemoveAt(0);*/
            _content = content;

            
        }

        public void Show()
        {
            _outputWrapper.WriteOutput(_posX, _posY, _content, ConsoleColor.Magenta);
            //if(infoList.Count <= 4)
                //_posY++;
        }
    }
}
