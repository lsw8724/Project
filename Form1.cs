using DaqProtocol;
using TestCms1.Dialog;
using DevExpress.XtraEditors;
using Steema.TeeChart.Styles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using Newtonsoft.Json;

namespace TestCms1
{
    public partial class WaveMonitor : Form
    {
        private const int TrendXScale = 30;
        private SimpleButton[] AddButtons;
        private SimpleButton[] DelButtons;
        public BindingList<IWavesReceiver> ReceiverList = new BindingList<IWavesReceiver>();
        public BindingList<IWavesMeasure> MeasureList = new BindingList<IWavesMeasure>();
        public BindingList<IWavesRecoder> RecoderList = new BindingList<IWavesRecoder>();
        private IWavesReceiver SelectedReceiver;
        public Queue<WaveData[]> WaveQueue = new Queue<WaveData[]>();
        private string ReceiverConfigPath = Application.StartupPath + "\\ReceiverConfigs.JSON";
        private string MeasureConfigPath = Application.StartupPath + "\\MeasureConfigs.JSON";
        private string RecoderConfigPath = Application.StartupPath + "\\RecoderConfigs.JSON";
        

        public WaveMonitor()
        {
            InitializeComponent();

            ConfigUtil.LoadConfig(ReceiverConfigPath, ReceiverList);
            ConfigUtil.LoadConfig(MeasureConfigPath, MeasureList);
            ConfigUtil.LoadConfig(RecoderConfigPath, RecoderList);

            FFTChart.Axes.Bottom.Maximum = ConstantMember.AsyncFMax;
            TrendChart.Axes.Bottom.Labels.DateTimeFormat = "yyyy.M.d\nHH:mm:ss";
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog2.InitialDirectory = Application.StartupPath;

            lb_Receiver.DataSource = ReceiverList;
            lb_Measure.DataSource = MeasureList;
            lb_Recoder.DataSource = RecoderList;

            lb_Receiver.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            AddButtons = new SimpleButton[] { btn_AddMeasure, btn_AddRceive, btn_AddRecode };
            btn_AddMeasure.Tag = new MeasureEditForm();
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
            var btn = sender as SimpleButton;
            var listBox = btn.Tag as ListBox;
            if (listBox.Items.Count != 0)
                (listBox.DataSource as IBindingList).RemoveAt(listBox.SelectedIndex);
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
            foreach(var recoder in RecoderList)
            {
                var queue = recoder.GetWavesQueue();
                lock (((ICollection)queue).SyncRoot)
                {
                    if (queue.Count > 10) WaveQueue.Clear();
                    queue.Enqueue(waves);
                }
            }

            /*Time*/
            foreach (var serise in TimeChart.Series) (serise as FastLine).Clear();
            for (int ch = 0; ch < waves.Length; ch++)
            {
                for (int i = 0; i < waves[ch].AsyncDataCount; i++)
                    TimeChart.Series[ch].Add(i * (ConstantMember.AsyncLine / (double)ConstantMember.AsyncFMax) / (double)waves[ch].AsyncDataCount, waves[ch].AsyncData[i]);
            }

            /*FFT*/
            foreach (var serise in FFTChart.Series) (serise as FastLine).Clear();
            for (int ch = 0; ch < waves.Length; ch++)
            {
                var fft = FFTCalculator.CreateSpectrumData(waves[ch].AsyncData, waves[ch].DateTime, ConstantMember.AsyncLine, ConstantMember.AsyncFMax);
                for (int i = 0; i < fft.XValues.Length; i++)
                    FFTChart.Series[ch].Add(fft.XValues[i], fft.YValues[i]);
            }

            /*Trend*/
            if (MeasureList.Count > 0)
            {
                for (int i = 0; i < MeasureList.Count; i++)
                {
                    var chid = MeasureList[i].GetChannelIdx();
                    var fftData = FFTCalculator.CreateSpectrumData(waves[chid].AsyncData, waves[chid].DateTime, ConstantMember.AsyncLine, ConstantMember.AsyncFMax);
                    var series = TrendChart.Series[i];
                    if (series.Count > TrendXScale) series.Delete(0);
                    series.Add(waves[chid].DateTime, MeasureList[i].CalcMeasureScalar(waves[chid], fftData));
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConfigUtil.SaveConfig(ReceiverConfigPath, ReceiverList);
            ConfigUtil.SaveConfig(MeasureConfigPath, MeasureList);
            ConfigUtil.SaveConfig(RecoderConfigPath, RecoderList);

            if (SelectedReceiver != null)
            {
                SelectedReceiver.Stop();
                SelectedReceiver = null;
            }

            if (RecoderList.Count > 0)
            {
                foreach (var recoder in RecoderList)
                    recoder.Stop();
                RecoderList.Clear();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            foreach (var serise in TimeChart.Series) (serise as FastLine).Clear();
            foreach (var serise in FFTChart.Series) (serise as FastLine).Clear();
            TrendChart.Series.Clear();

            btnStart.Enabled = false;
            btnStop.Enabled = true;

            SelectedReceiver = lb_Receiver.SelectedItem as IWavesReceiver;

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
    }
}
