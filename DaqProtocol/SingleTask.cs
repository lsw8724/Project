using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestCms1;

namespace DaqProtocol
{
    public interface ICancelableTask : IDisposable
    {
        void Start();

        void Stop();
    }

    public class WaveData
    {
        public uint Idx;
        public int ChannelId;
        public int SaveType;
        public DateTime DateTime;
        public float Rpm;
        public int SyncDataCount;
        public int AsyncDataCount;
        public float[] SyncData;
        public float[] AsyncData;
    }

    public interface IWavesReceiver : ICancelableTask
    {
        event Action<WaveData[]> WavesReceived;
    }

    public abstract class SingleTask : ICancelableTask
    {
        protected Task task = null;
        protected CancellationTokenSource cancelSource;

        public SingleTask()
        {
            this.cancelSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
        }

        public void WriteLog(string msg)
        {
            Debug.WriteLine(msg);
        }

        public virtual void Start()
        {
            WriteLog("Start");
            if (task != null)
                return;

            task = Task.Factory.StartNew(OnStart, cancelSource.Token);
        }

        public virtual void Stop()
        {
            WriteLog("Stop");
            if (!cancelSource.IsCancellationRequested)
            {
                cancelSource.Cancel();
                cancelSource = new CancellationTokenSource();
            }
            if (task != null)
            {
                if (!task.IsCompleted)
                    task.Wait();
                task = null;
            }
        }

        private void OnStart()
        {
            try
            {
                WriteLog("Task Created");
                OnNewTask(cancelSource.Token);
                WriteLog("Task Exiting");
            }
            catch (Exception ex)
            {
                WriteLog("Error on new task - " + ex);
            }
        }
        
        protected abstract void OnNewTask(CancellationToken token);
    }
}