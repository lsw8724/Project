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
        private ReceiveConfig ConfigItems = new ReceiveConfig();
        public ReceiverEditForm()
        {
            InitializeComponent();
            radioGroup1.DataBindings.Add(new Binding("SelectedIndex", bindingSource1, "ReceiverTypeIndex"));
            tb_ModuleIp.DataBindings.Add(new Binding("Text", bindingSource1, "ModuleIp"));
            bte_OpenFile.DataBindings.Add(new Binding("Text", bindingSource1, "FilePath"));
            tb_ServerIp.DataBindings.Add(new Binding("Text", bindingSource1, "ServerIp"));
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
            var monitor = Owner as WaveMonitor;
            var Serializers = cb_Protocol.SelectedValue as IWaveSerializer;
            IReceiver CurrentReceiver = null;
            switch ((ReceiverType)ConfigItems.ReceiverTypeIndex)
            {
                case ReceiverType.Daq5509:               
                    CurrentReceiver = new VDPMReceiver(ConfigItems.ModuleIp); 
                    break;
                case ReceiverType.File: CurrentReceiver = new FileReceiver(ConfigItems.FilePath, Serializers);
                    break;
                case ReceiverType.Network: CurrentReceiver = new NetworkReceiver(ConfigItems.ServerIp, ConfigItems.Port, Serializers); 
                    break;
            }
            if (CurrentReceiver != null)
            {
                CurrentReceiver.WavesReceived += monitor.waveReceiver_WavesReceived;
                monitor.ReceiverList.Add(CurrentReceiver);
                if (monitor.xtraTabControl1.SelectedTabPageIndex == 2)
                    ConfigUtil.SendConfig(monitor.Config.ServerIp, monitor.Config.SendPort, CurrentReceiver, monitor.rtb_Client);
                Close();
            }
        }

        private class ReceiveConfig
        {
            public ReceiveConfig()
            {
                ReceiverTypeIndex = 0;
                ModuleIp = "192.168.0.11";
                FilePath = Application.StartupPath + "\\Test_LSW.dat";
                ServerIp = "127.0.0.1";
                Port = 8999;

                Serializers = new List<IWaveSerializer>();
                Serializers.Add(new WaveDataSerializer_LSW());
                Serializers.Add(new WaveDataSerializer_KHW());
                Serializers.Add(new WaveDataSerializer_SHK());
            }
            public int ReceiverTypeIndex { get; set; }
            public string ModuleIp { get; set; }
            public List<IWaveSerializer> Serializers { get; set; }
            public string FilePath { get; set; }
            public string ServerIp { get; set; }
            public int Port { get; set; }
        }
    }
}