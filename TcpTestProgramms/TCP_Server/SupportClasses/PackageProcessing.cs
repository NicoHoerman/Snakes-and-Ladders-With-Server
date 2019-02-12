using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCP_Server.Actions;

namespace TCP_Server.Support
{
    public class PackageProcessing
    {
        private readonly PackageQueue _queue;
        private bool _isRunning;
        private BackgroundWorker _worker;
        private ServerActions _actionHandler;

        public PackageProcessing(PackageQueue queue, ServerActions serverActions)
        {
            _queue = queue;
            _actionHandler = serverActions;

            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _isRunning = true;
            while (_isRunning)
            {
				Shared.Contract.IPackage package = _queue.WaitForNextPackage();
                //Console.WriteLine($"Processing package number {package.Id}");
                _actionHandler.ExecuteDataActionFor(package.Communication,package.Data);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}
