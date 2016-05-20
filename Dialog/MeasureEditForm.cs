using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace TestCms1.Dialog
{
    public partial class MeasureEditForm : DevExpress.XtraEditors.XtraForm
    {
        private MeasureConfig ConfigItems = new MeasureConfig();
        public MeasureEditForm()
        {
            InitializeComponent();

            radioGroup1.DataBindings.Add(new Binding("SelectedIndex", bindingSource1, "MeasureTypeIndex"));
            tb_HighFreq.DataBindings.Add(new Binding("Text", bindingSource1, "High"));
            tb_lowFreq.DataBindings.Add(new Binding("Text", bindingSource1, "Low"));
            tb_Impulse1.DataBindings.Add(new Binding("Text", bindingSource1, "Impulse1RegionTime"));
            tb_Impulse2.DataBindings.Add(new Binding("Text", bindingSource1, "Impulse2RegionTime"));
            tb_Move.DataBindings.Add(new Binding("Text", bindingSource1, "MoveRegionTime"));
            cb_Channel.DataBindings.Add(new Binding("SelectedIndex", bindingSource1, "ChannelIdx"));
            bindingSource1.DataSource = ConfigItems;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            IMeasureCalculator CurrentMeasure = null;
            switch ((MeasureType)ConfigItems.MeasureTypeIndex)
            {
                case MeasureType.RMS: CurrentMeasure = new RMSMeasure(ConfigItems.Low, ConfigItems.High) { ChannelIdx = ConfigItems.ChannelIdx }; break;
                case MeasureType.PeakToPeak: CurrentMeasure = new PeakToPeakMeasure() { ChannelIdx = ConfigItems.ChannelIdx }; break;
                case MeasureType.Peak: CurrentMeasure = new PeakMeasure() { ChannelIdx = ConfigItems.ChannelIdx }; break;
                case MeasureType.Lift_Shock: CurrentMeasure = new Lift_ShockMeasure(ConfigItems.Impulse1RegionTime, ConfigItems.Impulse2RegionTime, ConfigItems.MoveRegionTime) { ChannelIdx = ConfigItems.ChannelIdx }; break;
                case MeasureType.Lift_Move: CurrentMeasure = new Lift_MoveMeasure(ConfigItems.Impulse1RegionTime, ConfigItems.Impulse2RegionTime, ConfigItems.MoveRegionTime) { ChannelIdx = ConfigItems.ChannelIdx }; break;
            }
            if (CurrentMeasure != null)
            {
                (Owner as WaveMonitor).MeasureList.Add(CurrentMeasure);
                Close();
            }
        }

        private class MeasureConfig
        {
            public MeasureConfig()
            {
                MeasureTypeIndex = 0;
                High = 1000;
                Low = 900;

                Impulse1RegionTime = 4000;
                Impulse2RegionTime = 4000;
                MoveRegionTime = 6000;
            }
            public int Impulse1RegionTime { get; set; }
            public int Impulse2RegionTime { get; set; }
            public int MoveRegionTime { get; set; }
            public int ChannelIdx { get; set; }
            public int MeasureTypeIndex { get; set; }
            public int High { get; set; }
            public int Low { get; set; }
        }
    }
}