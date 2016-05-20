using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TestCms1;
using TraceTool;

namespace DaqProtocol
{
    public class Daq5509Receiver : SingleTask, IWavesReceiver
    {
        private string moduleIp;
        public DaqClient daq;

        private RobinChannel[] channels;
        private float[] scaleFactors;
        DaqSamplingRate nSamplingRate;
        private int Fmax { get { return channels[0].AsyncFMax; } }
        private int Line { get { return channels[0].AsyncLine; } }
        private double AcquireSec { get { return (double)(Line / (double)Fmax); } }
        private TimeSpan TimeOffset { get; set; }

        public DaqInputType InputType { get; set; }
        WinWatch watch;
        int PacketCountFor1Sec = 16;
        public event Action<WaveData[]> WavesReceived;

        public Daq5509Receiver(string moduleIp, RobinChannel[] channels, int PacketCountFor1Sec = 16)
        {
            this.moduleIp = moduleIp;
            this.InputType = DaqInputType.AC;
            this.channels = channels;
            //this.TimeTrigger = new SimpleTimeTrigger(TimeSpan.FromSeconds(TSIAgentTrig.Properties.Settings.Default.CmsTimeTriggerPeriod));
            this.PacketCountFor1Sec = PacketCountFor1Sec;

            scaleFactors = new float[channels.Length];
            foreach (var channel in channels)
                scaleFactors[channel.PhysicalIndex] = channel.ScaleFactorByDisplayUnit();//channel.ScaleFactor;//db.GetChannel(ch + 1).ScaleFactor;

            if (!Enum.TryParse<DaqSamplingRate>("_" + (channels[0].AsyncFMax * 2.56), out nSamplingRate))
                nSamplingRate = DaqSamplingRate._8192;
        }

        protected override void OnNewTask(CancellationToken token)
        {
            //ReconnectLoop
            while (!token.IsCancellationRequested)
            {
                try
                {
                    WriteLog("Connecting");

                    ConnectDaq();

                    ReadLoop(token);

                    CloseDaq();

                    WriteLog("Closing");
                }
                catch (Exception ex)
                {
                    WriteLog("Error - " + ex);
                    Thread.Sleep(100);
                }
            }
        }

        private void ConnectDaq()
        {
            CloseDaq();

            for (int i = 0; i < 5; i++)    //Connect 실패가 잦으니 여러번 시도
            {
                try
                {
                    var _daq = new DaqClient(scaleFactors);
                    _daq.PacketCountFor1Sec = PacketCountFor1Sec;
                    _daq.Connect(moduleIp, 7000);
                    _daq.Stop(true);
                    //if (!_daq.Stop(true))
                    //    throw new Exception("Stop Failed");
                    daq = _daq;
                    break;
                }
                catch (Exception) { Thread.Sleep(500); }
            }
            if (daq == null)
                throw new Exception("Connect Failed - IP:" + moduleIp);

            foreach (var channel in channels)
            {
                daq.SetInputType(channel.PhysicalIndex, channel.ICP, InputType);
                //daq.SetInputType(channel.PhysicalIndex, false, DaqInputType.AC);
                daq.SetGain(channel.PhysicalIndex, false, channel.HWGain);
            }
            daq.SetRunVariable();
            daq.SetSampleMode(nSamplingRate);

            daq.Start();
        }
        private void CloseDaq()
        {
            if (daq != null)
            {
                try
                {
                    daq.Stop();
                    daq.Close();
                }
                catch (Exception) { }
                daq = null;
            }
        }

        private void ReadLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var data = daq.FetchDatas((int)(8 * AcquireSec));
                var now = DateTime.Now.Add(TimeOffset);

                var waves = new WaveData[8];
                for (int i = 0; i < channels.Length; i++)
                {
                    var ch = channels[i];
                    var asyncs = data.ChannelsAsyncs[i];
                    var acquireSec = ch.AsyncLine / ch.AsyncFMax;
                    var dataSize = (int)(ch.AsyncFMax * 2.56 * acquireSec);
                    if (asyncs.Length != dataSize)
                    {
                        System.Diagnostics.Trace.WriteLine("5509 Read Error - CH:" + ch.Id + ", AsyncSize:" + asyncs.Length + ", Expect:" + dataSize);
                        goto NEXT_READ;
                    }

                    var wave = new WaveData();
                    wave.ChannelId = ch.Id;
                    wave.DateTime = now;
                    wave.Rpm = data.Rpm1;
                    wave.AsyncData = asyncs;
                    wave.AsyncDataCount = asyncs.Length;
                    waves[i] = wave;
                }

                if (WavesReceived != null)
                    WavesReceived(waves);

            NEXT_READ:
                continue;
            }
        }
    }
}

   