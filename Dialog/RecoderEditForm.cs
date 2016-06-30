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
        public int RecoderTypeIndex {get;set;}
        public string FilePath {get;set;}
        public int Port { get; set; }
        public string DbIp { get; set; }
        public string DbAccount { get; set; }
        public string DbPassword { get; set; }
        public string DbName { get; set; }
        IWavesSerializer[] Serializers = new IWavesSerializer[] 
        {
            new WaveDataSerializer_LSW(),
            new WaveDataSerializer_KHW(),
            new WaveDataSerializer_SHK(),
            new WaveDataSerializer_Cpp()
        };

        public RecoderEditForm()
        {
            InitializeComponent();
            RecoderTypeIndex = 0;
            FilePath = Application.StartupPath + "\\Test_LSW.dat";
            Port = 8999;

            DbIp = "127.0.0.1";
            DbAccount = "sa";
            DbPassword = "nada1234";
            DbName = "TestCms";

            radioGroup1.DataBindings.Add(new Binding("SelectedIndex", bindingSource1, "RecoderTypeIndex"));
            bte_OpenFile.DataBindings.Add(new Binding("Text", bindingSource1, "FilePath"));
            tb_DataPort.DataBindings.Add(new Binding("Text", bindingSource1, "Port"));
            bindingSource1.DataSource = this;
            cb_Protocol.DataSource = new BindingSource(Serializers, null);
            tb_dbIp.DataBindings.Add(new Binding("Text", bindingSource1, "DbIp"));
            tb_dbAcc.DataBindings.Add(new Binding("Text", bindingSource1, "DbAccount"));
            tb_dbPw.DataBindings.Add(new Binding("Text", bindingSource1, "DbPassword"));
            tb_dbName.DataBindings.Add(new Binding("Text", bindingSource1, "DbName"));
        }

        private void bte_OpenFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            bte_OpenFile.Text = openFileDialog1.FileName;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            var serializer = cb_Protocol.SelectedValue as IWavesSerializer;
            var monitor = Owner as WaveMonitor;
            switch (radioGroup1.SelectedIndex)
            {
                case 0: monitor.RecoderList.Add(new FileRecoder(FilePath, serializer)); break;
                case 1: monitor.RecoderList.Add(new NetworkRecoder(Port, serializer)); break;
                case 2: monitor.RecoderList.Add(new DBRecoder(DbIp, DbAccount, DbPassword, DbName));break;
            }
            Close();
        }
    }
}