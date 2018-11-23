using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wrapper.Contracts;
using Wrapper.View;

namespace Wrapper.Implementation
{
    public class ViewUpdater
    {
        private Dictionary<ClientView, IView> _views;
        private bool isRunning;

        public ViewUpdater(Dictionary<ClientView, IView> views)
        {
            _views = views;
        }

        public void UpdateView()
        {
            _views.Values.ToList().ForEach(view =>
            {
                if (view.viewEnabled)
                {
                    view.Show();
                }
            });
        }

        private bool CheckForOutputchanges()
        {
            return true;
        }
        public void RunUpdater()
        {
            isRunning = true;
            while (isRunning)
            {
                if (CheckForOutputchanges())
                    UpdateView();

                Thread.Sleep(5000);
            }
        }
    }
}
