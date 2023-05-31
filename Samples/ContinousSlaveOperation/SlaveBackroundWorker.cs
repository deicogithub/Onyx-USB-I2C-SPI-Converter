using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using OnyxAPI_NET;

namespace ContinousSlaveOperation
{
    public class SlaveBackroundWorker
    {
        private readonly BackgroundWorker Worker = new BackgroundWorker();
        private Device _device;
        private AvailableAsyncData _availableData;
        private string _peripheralType;

        public static readonly object lockObject = new object();
        public SlaveBackroundWorker(Device device)
        {
            _device = device;
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += AsyncPoll;
            Worker.RunWorkerCompleted += SlaveDisabled;
        }

        public void StartWorker()
        {
            if (!Worker.IsBusy)
                Worker?.RunWorkerAsync();
        }

        public void StopWorker()
        {
            if (Worker.IsBusy)
                Worker.CancelAsync();
        }

        private void SlaveDisabled(object sender, RunWorkerCompletedEventArgs e)
        {
            Worker.Dispose();
        }

        private void AsyncPoll(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                // Use lock object to prevent race condition
                lock (lockObject)
                {
                    if (Worker.CancellationPending) break;
                    _availableData = _device.AsyncPoll();
                    if (_availableData.AnyData())
                    {
                        OnDataReceived(new DataArrivedEventArguments() { DataLength = _availableData.AvailableSPIsReadBytes });
                    }

                    Thread.Sleep(100);
                }
            }
        }

        public event EventHandler<DataArrivedEventArguments> DataReceivedEventHandler;

        protected virtual void OnDataReceived(DataArrivedEventArguments e)
        {
            var handler = DataReceivedEventHandler;
            if(handler != null)
                handler(this, e);
        }

    }

    public class DataArrivedEventArguments : EventArgs
    {
        public int DataLength { get; set; }
    }
}
