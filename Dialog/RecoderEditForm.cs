using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TestCms1.Dialog
{
    public partial class RecoderEditForm : DevExpress.XtraEditors.XtraForm
    {
        private RecodeConfig ConfigItems = new RecodeConfig();
        public RecoderEditForm()
        {
            InitializeComponent();
            radioGroup1.DataBindings.Add(new Binding("SelectedIndex", bindingSource1, "RecoderTypeIndex"));
            bte_OpenFile.DataBindings.Add(new Binding("Text", bindingSource1, "FilePath"));
            tb_DataPort.DataBindings.Add(new Binding("Text", bindingSource1, "Port"));
            bindingSource1.DataSource = ConfigItems;
            cb_Protocol.DataSource = new BindingSource(ConfigItems.Serializers, null);
        }

        private void bte_OpenFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            bte_OpenFile.Text = openFileDialog1.FileName;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            var serializer = cb_Protocol.SelectedValue as IWaveSerializer;
            var monitor = Owner as WaveMonitor;
            IWavesRecoder CurrentRecoder = null;
            switch ((RecoderType)ConfigItems.RecoderTypeIndex)
            {
                case RecoderType.File: CurrentRecoder = new FileRecoder(ConfigItems.FilePath, serializer); break;
                case RecoderType.Network: CurrentRecoder = new NetworkRecoder(ConfigItems.Port, serializer); break;
            }
            if (CurrentRecoder != null)
            {
                monitor.RecoderList.Add(CurrentRecoder);
                Close();
            }
        }
        private class RecodeConfig
        {
            public RecodeConfig()
            {
                RecoderTypeIndex = 0;
                FilePath = Application.StartupPath + "\\Test_LSW.dat";
                Port = 8999;
                Serializers = new List<IWaveSerializer>();
                Serializers.Add(new WaveDataSerializer_LSW());
                Serializers.Add(new WaveDataSerializer_KHW());
                Serializers.Add(new WaveDataSerializer_SHK());
            }

            public int RecoderTypeIndex { get; set; }
            public List<IWaveSerializer> Serializers { get; set; }

            public string FilePath { get; set; }
            public int Port { get; set; }
        }
    }
}