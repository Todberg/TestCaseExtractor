using System;
using System.ComponentModel;
using System.Windows;

namespace TestCaseExtractor
{
    public class AsyncTasks
    {
        private BackgroundWorker worker;
        private Action endAction;

        private Window ownerWindow;
        private LoadingWindow loadingWindow;
                
        public AsyncTasks(Window owerWindow)
        {
            this.ownerWindow = owerWindow;
            this.worker = new BackgroundWorker();
        }

        public void Execute(Action beginAction, Action endAction = null)
        {
            this.endAction = endAction;
            this.Subscribe();
            this.worker.RunWorkerAsync(beginAction);
            this.loadingWindow = new LoadingWindow();
            this.loadingWindow.Owner = this.ownerWindow;
            this.loadingWindow.ShowDialog();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            ((Action)e.Argument)();
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.endAction != null)
                this.endAction();

            this.Unsubscribe();
            this.loadingWindow.Close();
        }

        private void Subscribe()
        {
            this.worker.DoWork += new DoWorkEventHandler(this.WorkerDoWork);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerRunWorkerCompleted);
        }

        private void Unsubscribe()
        {
            this.worker.DoWork -= new DoWorkEventHandler(this.WorkerDoWork);
            this.worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.WorkerRunWorkerCompleted);
        }
    }
}