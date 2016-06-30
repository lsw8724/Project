using DaqProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace TestCms1.Dialog
{
    public partial class ReceiverEditForm : DevExpress.XtraEditors.XtraForm
    {
        public int ReceiverTypeIndex { get; set; }
        public string ModuleIp { get; set; }
        public string FilePath { get; set; }
        public string ServerIp { get; set; }
        public int Port { get; set; }
        IWavesSerializer[] Serializers = new IWavesSerializer[] {new WaveDataSerializer_LSW(),new WaveDataSerializer_KHW(),new WaveDataSerializer_SHK(), new WaveDataSerializer_Cpp()};

        public ReceiverEditForm()
        {
            InitializeComponent();
            ReceiverTypeIndex = 0;
            ModuleIp = "192.168.0.11";
            FilePath = Application.StartupPath + "\\Test_LSW_Cpp.dat";
            ServerIp = "127.0.0.1";
            Port = 8999;
            tb_ModuleIp.DataBindings.Add(new Binding("Text", bindingSource1, "ModuleIp"));
            bte_OpenFile.DataBindings.Add(new Binding("Text", bindingSource1, "FilePath"));
            tb_ServerIp.DataBindings.Add(new Binding("Text", bindingSource1, "ServerIp"));
            tb_DataPort.DataBindings.Add(new Binding("Text", bindingSource1, "Port"));
            bindingSource1.DataSource = this;
            cb_Protocol.DataSource = new BindingSource(Serializers, null);
        }

        private void bte_OpenFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            bte_OpenFile.Text = openFileDialog1.FileName;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            var monitor = Owner as WaveMonitor;
            var Serializers = cb_Protocol.SelectedValue as IWavesSerializer;
            IHeritableWavesReceiver CurrentReceiver = null;
            switch (radioGroup1.SelectedIndex)
            {
                case 0: CurrentReceiver = new VDPMReceiver(ModuleIp); break;
                case 1: CurrentReceiver = new FileReceiver(FilePath, Serializers); break;
                case 2: CurrentReceiver = new NetworkReceiver(ServerIp, Port, Serializers); break;
                case 3: CurrentReceiver = new SimulateReceiver(); break;
            }
            if (CurrentReceiver != null)
            {
                CurrentReceiver.WavesReceived += monitor.waveReceiver_WavesReceived;
                monitor.ReceiverList.Add(CurrentReceiver);
                Close();
            }
        }
    }
}