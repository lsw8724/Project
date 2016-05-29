namespace TestCms1.Dialog
{
    partial class ReceiverEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.tb_ModuleIp = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.bte_OpenFile = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.tb_ServerIp = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.tb_DataPort = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.cb_Protocol = new System.Windows.Forms.ComboBox();
            this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_Ok = new DevExpress.XtraEditors.SimpleButton();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tb_ModuleIp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bte_OpenFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_ServerIp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_DataPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(144, 40);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(54, 14);
            this.labelControl1.TabIndex = 22;
            this.labelControl1.Text = "Module IP";
            // 
            // tb_ModuleIp
            // 
            this.tb_ModuleIp.EditValue = "192.168.0.11";
            this.tb_ModuleIp.Location = new System.Drawing.Point(213, 37);
            this.tb_ModuleIp.Name = "tb_ModuleIp";
            this.tb_ModuleIp.Size = new System.Drawing.Size(121, 20);
            this.tb_ModuleIp.TabIndex = 8;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(144, 115);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(42, 14);
            this.labelControl3.TabIndex = 24;
            this.labelControl3.Text = "FilePath";
            // 
            // bte_OpenFile
            // 
            this.bte_OpenFile.Location = new System.Drawing.Point(213, 112);
            this.bte_OpenFile.Name = "bte_OpenFile";
            this.bte_OpenFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bte_OpenFile.Size = new System.Drawing.Size(129, 20);
            this.bte_OpenFile.TabIndex = 23;
            this.bte_OpenFile.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_OpenFile_ButtonClick);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(144, 202);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(50, 14);
            this.labelControl2.TabIndex = 26;
            this.labelControl2.Text = "Server IP";
            // 
            // tb_ServerIp
            // 
            this.tb_ServerIp.EditValue = "127.0.0.1";
            this.tb_ServerIp.Location = new System.Drawing.Point(213, 199);
            this.tb_ServerIp.Name = "tb_ServerIp";
            this.tb_ServerIp.Size = new System.Drawing.Size(121, 20);
            this.tb_ServerIp.TabIndex = 25;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(144, 229);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(23, 14);
            this.labelControl4.TabIndex = 28;
            this.labelControl4.Text = "Port";
            // 
            // tb_DataPort
            // 
            this.tb_DataPort.Location = new System.Drawing.Point(213, 226);
            this.tb_DataPort.Name = "tb_DataPort";
            this.tb_DataPort.Size = new System.Drawing.Size(77, 20);
            this.tb_DataPort.TabIndex = 27;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(144, 141);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(45, 14);
            this.labelControl5.TabIndex = 31;
            this.labelControl5.Text = "Protocol";
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.cb_Protocol);
            this.groupControl2.Controls.Add(this.radioGroup1);
            this.groupControl2.Controls.Add(this.labelControl5);
            this.groupControl2.Controls.Add(this.bte_OpenFile);
            this.groupControl2.Controls.Add(this.labelControl3);
            this.groupControl2.Controls.Add(this.tb_ModuleIp);
            this.groupControl2.Controls.Add(this.labelControl1);
            this.groupControl2.Controls.Add(this.labelControl4);
            this.groupControl2.Controls.Add(this.tb_DataPort);
            this.groupControl2.Controls.Add(this.labelControl2);
            this.groupControl2.Controls.Add(this.tb_ServerIp);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl2.Location = new System.Drawing.Point(0, 0);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.ShowCaption = false;
            this.groupControl2.Size = new System.Drawing.Size(356, 257);
            this.groupControl2.TabIndex = 30;
            this.groupControl2.Text = "groupControl2";
            // 
            // cb_Protocol
            // 
            this.cb_Protocol.FormattingEnabled = true;
            this.cb_Protocol.Location = new System.Drawing.Point(213, 138);
            this.cb_Protocol.Name = "cb_Protocol";
            this.cb_Protocol.Size = new System.Drawing.Size(121, 22);
            this.cb_Protocol.TabIndex = 35;
            // 
            // radioGroup1
            // 
            this.radioGroup1.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioGroup1.Location = new System.Drawing.Point(2, 2);
            this.radioGroup1.Name = "radioGroup1";
            this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Daq Receiver"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "File Receiver"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Network Receiver")});
            this.radioGroup1.Size = new System.Drawing.Size(136, 253);
            this.radioGroup1.TabIndex = 34;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_Ok
            // 
            this.btn_Ok.Location = new System.Drawing.Point(129, 263);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(99, 30);
            this.btn_Ok.TabIndex = 36;
            this.btn_Ok.Text = "확인";
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // ReceiverEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 305);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.groupControl2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReceiverEditForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ReceiverEditForm";
            ((System.ComponentModel.ISupportInitialize)(this.tb_ModuleIp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bte_OpenFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_ServerIp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_DataPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.groupControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit tb_ModuleIp;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.ButtonEdit bte_OpenFile;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit tb_ServerIp;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit tb_DataPort;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private DevExpress.XtraEditors.SimpleButton btn_Ok;
        private System.Windows.Forms.BindingSource bindingSource1;
        private DevExpress.XtraEditors.RadioGroup radioGroup1;
        private System.Windows.Forms.ComboBox cb_Protocol;
    }
}