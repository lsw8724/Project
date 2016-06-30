using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using TestCMSCommon.DataBase;
using TestCms1.Properties;

namespace TestCms1.Dialog
{
    public partial class MeasureEditForm : DevExpress.XtraEditors.XtraForm
    {
        public int MeasureId { get; set; }
        public int MeasureTypeIndex { get; set; }
        public int High { get; set; }
        public int Low { get; set; }
        public int ChannelIdx { get; set; }
        public int Impulse1RegionTime { get; set; }
        public int Impulse2RegionTime { get; set; }
        public int MoveRegionTime { get; set; }

        public MeasureEditForm(IWavesMeasure lastMeasure)
        {
            MeasureId = lastMeasure != null? lastMeasure.GetMeasureId(): 0;
            InitializeComponent();
            MeasureTypeIndex = 0;
            High = 62;
            Low = 58;
            Impulse1RegionTime = 4000;
            Impulse2RegionTime = 4000;
            MoveRegionTime = 6000;
            radioGroup1.DataBindings.Add(new Binding("SelectedIndex", bindingSource1, "MeasureTypeIndex"));
            tb_HighFreq.DataBindings.Add(new Binding("Text", bindingSource1, "High"));
            tb_lowFreq.DataBindings.Add(new Binding("Text", bindingSource1, "Low"));
            tb_Impulse1.DataBindings.Add(new Binding("Text", bindingSource1, "Impulse1RegionTime"));
            tb_Impulse2.DataBindings.Add(new Binding("Text", bindingSource1, "Impulse2RegionTime"));
            tb_Move.DataBindings.Add(new Binding("Text", bindingSource1, "MoveRegionTime"));
            cb_Channel.DataBindings.Add(new Binding("SelectedIndex", bindingSource1, "ChannelIdx"));
            bindingSource1.DataSource = this;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            var monitor = Owner as WaveMonitor;
            switch (radioGroup1.SelectedIndex)
            {
                case 0:
                    monitor.MeasureList.Add(new RMSMeasure(++MeasureId, ChannelIdx, Low, High));
                    if (Settings.Default.EnableDBMode) SQLRepository.Measurement.InsertData(MeasureId, (int)MeasureType.MeasureType_RMS , ChannelIdx, Low, High);
                    break;
                case 1:
                    monitor.MeasureList.Add(new PeakToPeakMeasure(++MeasureId, ChannelIdx));
                    if (Settings.Default.EnableDBMode) SQLRepository.Measurement.InsertData(MeasureId, (int)MeasureType.MeasureType_P2P, ChannelIdx);
                    break;
                case 2:
                    monitor.MeasureList.Add(new PeakMeasure(++MeasureId, ChannelIdx));
                    if (Settings.Default.EnableDBMode) SQLRepository.Measurement.InsertData(MeasureId, (int)MeasureType.MeasureType_PK, ChannelIdx);
                    break;
                case 3:
                    monitor.MeasureList.Add(new Lift_ShockMeasure(++MeasureId, ChannelIdx, Impulse1RegionTime, Impulse2RegionTime, MoveRegionTime));
                    if (Settings.Default.EnableDBMode) SQLRepository.Measurement.InsertData(MeasureId, (int)MeasureType.MeasureType_LiftShock, ChannelIdx, Low, High, Impulse2RegionTime, MoveRegionTime);
                    break;
                case 4:
                    monitor.MeasureList.Add(new Lift_MoveMeasure(++MeasureId, ChannelIdx, Impulse1RegionTime, Impulse2RegionTime, MoveRegionTime));
                    if (Settings.Default.EnableDBMode) SQLRepository.Measurement.InsertData(MeasureId, (int)MeasureType.MeasureType_LiftMove, ChannelIdx, Low, High, Impulse2RegionTime, MoveRegionTime);
                    break;
            }
            Close();    
        }
    }
}