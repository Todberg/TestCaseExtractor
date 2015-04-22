using System;
using System.ComponentModel;
using System.Windows;

namespace TestCaseExtractor
{
    public class AsyncTasks
    {
        private BackgroundWorker _worker;
        private Window _ownerWindow;
        private LoadingWindow _loadingWindow;
        private Action _closingAction;
        
        public AsyncTasks(Window owerWindow)
        {
            this._ownerWindow = owerWindow;
            this._worker = new BackgroundWorker();
        }

        public void Execute(Action startingAction, Action closingAction = null)
        {
            this._closingAction = closingAction;
            this.subscribe();
            this._worker.RunWorkerAsync(startingAction);
            this._loadingWindow = new LoadingWindow();
            this._loadingWindow.Owner = this._ownerWindow;
            this._loadingWindow.ShowDialog();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ((Action)e.Argument)();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this._closingAction != null)
            {
                this._closingAction();
            }
            this.unsubscribe();
            this._loadingWindow.Close();
        }

        private void subscribe()
        {
            this._worker.DoWork += new DoWorkEventHandler(this.worker_DoWork);
            this._worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
        }

        private void unsubscribe()
        {
            this._worker.DoWork -= new DoWorkEventHandler(this.worker_DoWork);
            this._worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
        }
    }
}