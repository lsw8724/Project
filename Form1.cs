 using DaqProtocol;
using TestCms1.Dialog;
using DevExpress.XtraEditors;
using Steema.TeeChart.Styles;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using TestCms1.Properties;
using TestCMSCommon.ADODotNET;

namespace TestCms1
{
    public partial class WaveMonitor : Form
    {
        private const int TrendXScale = 30;
        private SimpleButton[] AddButtons;
        private SimpleButton[] DelButtons;
        public BindingList<IHeritableWavesReceiver> ReceiverList = new BindingList<IHeritableWavesReceiver>();
        public BindingList<IWavesMeasure> MeasureList = new BindingList<IWavesMeasure>();
        public BindingList<IWavesRecoder> RecoderList = new BindingList<IWavesRecoder>();
        private IHeritableWavesReceiver SelectedReceiver;
        public Queue<WaveData[]> WaveQueue = new Queue<WaveData[]>();
        public Queue<TrendData[]> MeasureDataQueue = new Queue<TrendData[]>();
        private Integrator NormalIntegrator = new Integrator();
       
        public bool EnableIntegrator = false;

        public WaveMonitor()
        {
            InitializeComponent();
            SQLRepository.Init();
            if (Settings.Default.EnableDBMode)
            {
                foreach (var row in SQLRepository.MeasurementCache.Values)
                {
                    switch ((MeasureType)row.MeasureType)
                    {
                        case MeasureType.MeasureType_RMS: MeasureList.Add(new RMSMeasure(row.Idx, row.ChannelId, row.LowFreq, row.HighFreq)); break;
                        case MeasureType.MeasureType_P2P: MeasureList.Add(new PeakToPeakMeasure(row.Idx, row.ChannelId)); break;
                        case MeasureType.MeasureType_PK: MeasureList.Add(new PeakMeasure(row.Idx, row.ChannelId)); break;
                        case MeasureType.MeasureType_LiftShock: MeasureList.Add(new Lift_ShockMeasure(row.Idx, row.ChannelId, row.Interval1, row.Interval2, row.Interval3)); break;
                        case MeasureType.MeasureType_LiftMove: MeasureList.Add(new Lift_MoveMeasure(row.Idx, row.ChannelId, row.Interval1, row.Interval2, row.Interval3)); break;
                    }
                }

                foreach (var row in SQLRepository.ReceiverCache.Values)
                {
                    switch ((ReceiverType)row.ReceiverType)
                    {
                        case ReceiverType.ReceiverType_Vdpm: ReceiverList.Add(new VDPMReceiver(row.Ip)); break;
                        case ReceiverType.ReceiverType_File: ReceiverList.Add(new FileReceiver(row.FilePath, SerializerUtil.ToSerializer(row.SerializerType))); break;
                        case ReceiverType.ReceiverType_Network: ReceiverList.Add(new NetworkReceiver(row.Ip,row.Port,SerializerUtil.ToSerializer(row.SerializerType))); break;
                        case ReceiverType.ReceiverType_Simulate: ReceiverList.Add(new SimulateReceiver()); break;
                    }
                }
            }
            if (File.Exists("Config.dat"))
            {
                using (FileStream fStream = new FileStream("Config.dat", FileMode.Open))
                {
                    while (fStream.Position != fStream.Length)
                    {
                        ConfigPacket packet = new ConfigPacket();
                        packet.Read(fStream);
                        UnPacking(packet);
                    }
                }
            }

            FFTChart.Axes.Bottom.Maximum = Settings.Default.AsyncFMax;
            TrendChart.Axes.Bottom.Labels.DateTimeFormat = "yyyy.M.d\nHH:mm:ss";

            lb_Receiver.DataSource = ReceiverList;
            lb_Measure.DataSource = MeasureList;
            lb_Recoder.DataSource = RecoderList;

            AddButtons = new SimpleButton[] { btn_AddMeasure, btn_AddRceive, btn_AddRecode };
            btn_AddMeasure.Tag = new MeasureEditForm(MeasureList.LastOrDefault());
            btn_AddRceive.Tag = new ReceiverEditForm();
            btn_AddRecode.Tag = new RecoderEditForm();
            foreach (var btn in AddButtons)
                btn.Click += AddBtn_Click;

            DelButtons = new SimpleButton[] { btn_DelMeasure, btn_DelReceive, btn_DelRecode };
            btn_DelReceive.Tag = lb_Receiver;
            btn_DelMeasure.Tag = lb_Measure;
            btn_DelRecode.Tag = lb_Recoder;
            foreach (var btn in DelButtons)
                btn.Click += DelBtn_Click;
        }

        private void DelBtn_Click(object sender, EventArgs e)
        {
            lb_Receiver.Refresh();
            lb_Receiver.Update();
            var btn = sender as SimpleButton;
            var listBox = btn.Tag as ListBox;
            if (listBox.Items.Count != 0)
            {
                if (Settings.Default.EnableDBMode)
                    switch (listBox.Name)
                    {
                        case "lb_Measure": SQLRepository.Measurement.DeleteData((listBox.SelectedItem as ISelectableIdx).GetIdx());break;
                        case "lb_Receiver": SQLRepository.Receiver.DeleteData((listBox.SelectedItem as ISelectableIdx).GetIdx()); break;
                        //case "lb_Recoder": SQLRepository.Receiver.DeleteData((listBox.SelectedItem as ISelectableIdx).GetIdx()); break;
                    }
                   
                (listBox.DataSource as IBindingList).RemoveAt(listBox.SelectedIndex);
            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            var btn = sender as SimpleButton;
            var form = (btn.Tag as Form);
            form.Owner = this;
            form.ShowDialog();
        }

        public void waveReceiver_WavesReceived(WaveData[] waves)
        {
            /*Time*/
            var resolution = Settings.Default.AsyncLine / (double)Settings.Default.AsyncFMax;
            foreach (var serise in TimeChart.Series) (serise as FastLine).Clear();
          
            for (int ch = 0; ch < waves.Length; ch++)
            {
                var displayData = EnableIntegrator ? NormalIntegrator.Integration(waves[ch].AsyncData) : waves[ch].AsyncData;
                TimeChart.Series[ch].BeginUpdate();
                for (int i = 0; i < waves[ch].AsyncDataCount; i++)
                    TimeChart.Series[ch].Add(i * resolution / (double)waves[ch].AsyncDataCount, displayData[i]);
                TimeChart.Series[ch].EndUpdate();
            }

            /*FFT*/
            foreach (var serise in FFTChart.Series) (serise as FastLine).Clear();
            for (int ch = 0; ch < waves.Length; ch++)
            {
                var displayData = EnableIntegrator ? NormalIntegrator.Integration(waves[ch].AsyncData) : waves[ch].AsyncData;
                var fft = FFTCalculator.CreateSpectrumData(displayData, waves[ch].DateTime);
                FFTChart.Series[ch].BeginUpdate();
                for (int i = 0; i < fft.XValues.Length; i++)
                    FFTChart.Series[ch].Add(fft.XValues[i], fft.YValues[i]);
                FFTChart.Series[ch].EndUpdate();
            }
            TrendData[] datas = new TrendData[MeasureList.Count];
            /*Trend*/
            if (MeasureList.Count > 0)
            {
                for (int i = 0; i < MeasureList.Count; i++)
                {
                    var chid = MeasureList[i].GetChannelIdx();
                    var fftData = FFTCalculator.CreateSpectrumData(waves[chid].AsyncData, waves[chid].DateTime);
                    var series = TrendChart.Series[i];
                    if (series.Count > TrendXScale) series.Delete(0);
                    var measureData = MeasureList[i].CalcMeasureScalar(waves[chid], fftData);
                    series.Add(waves[chid].DateTime, MeasureList[i].CalcMeasureScalar(waves[chid], fftData));
                    SQLRepository.TrendData.InsertData(new TrendData(waves[chid].DateTime, MeasureList[i].GetIdx(), measureData));
                }
            }

            foreach (var recoder in RecoderList)
            {
                var queue = recoder.GetWavesQueue();
                lock (((ICollection)queue).SyncRoot)
                {
                    if (queue.Count > 100) WaveQueue.Dequeue();
                    queue.Enqueue(waves);
                }
            }
        }

        private ConfigPacket Packing(ISendableConfig config)
        {
            var bytes = config.ToByte();
            return new ConfigPacket(){
                Payload = bytes,
                Header = new ConfigPacketHeader(){PacketTypeByte = config.GetPacketType(), SubType = config.GetConfigType(), PayloadSize = bytes.Length }
            };
        }

        private void UnPacking(ConfigPacket packet)
        {
            ConfigReader reader = new ConfigReader();
            if (packet.Payload != null)
            {
                switch (packet.Header.PacketTypeByte)
                {
                    case PacketType.PacketType_ReceiverConfig: ReceiverList.Add(reader.Read(packet) as IHeritableWavesReceiver); break;
                    case PacketType.PacketType_RecoderConfig: RecoderList.Add(reader.Read(packet) as IWavesRecoder); break;
                    case PacketType.PacketType_MeasureConfig: if(!Settings.Default.EnableDBMode) MeasureList.Add(reader.Read(packet) as IWavesMeasure); break;
                }
                foreach (var receiver in ReceiverList)
                    receiver.WavesReceived += waveReceiver_WavesReceived;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            List<ISendableConfig> configList = new List<ISendableConfig>();
            configList.AddRange(ReceiverList);
            if(!Settings.Default.EnableDBMode) configList.AddRange(MeasureList);
            configList.AddRange(RecoderList);
            try
            {
                using (FileStream fStream = new FileStream("Config.dat", FileMode.Create))
                {
                    foreach (var conf in configList)
                    {
                        ConfigPacket packet = Packing(conf);
                        packet.Write(fStream);
                    }      
                }          
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (SelectedReceiver != null)
            {
                SelectedReceiver.Stop();
                SelectedReceiver = null;
            }

            if (RecoderList.Count > 0)
            {
                foreach (var recoder in RecoderList)
                    recoder.Stop();
            }
            SQLRepository.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            foreach (var serise in TimeChart.Series) (serise as FastLine).Clear();
            foreach (var serise in FFTChart.Series) (serise as FastLine).Clear();
            TrendChart.Series.Clear();

            btnStart.Enabled = false;
            btnStop.Enabled = true;

            SelectedReceiver = lb_Receiver.SelectedItem as IHeritableWavesReceiver;

            foreach (var m in MeasureList)
            {
                FastLine line = new FastLine() { Title = "Ch" + (m.GetChannelIdx() + 1) + " " + m.ToString().Replace("TestCms1.", "").Replace("Measure", "") };
                TrendChart.Series.Add(line);
            }

            if (SelectedReceiver != null) SelectedReceiver.Start();
            foreach (var recoder in RecoderList)
                recoder.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (SelectedReceiver != null)
            {
                SelectedReceiver.Stop();

                btnStop.Enabled = false;
                btnStart.Enabled = true;
            }
            foreach (var recoder in RecoderList)
                recoder.Stop();
        }

        private void Raw_Click(object sender, EventArgs e)
        {
            EnableIntegrator = false;
        }

        private void Integrate_Click(object sender, EventArgs e)
        {
            EnableIntegrator = true;
        }
    }
}
