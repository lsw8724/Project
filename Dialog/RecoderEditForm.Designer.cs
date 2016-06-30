namespace TestCms1.Dialog
{
    partial class RecoderEditForm
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
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.tb_DataPort = new DevExpress.XtraEditors.TextEdit();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.cb_Protocol = new System.Windows.Forms.ComboBox();
            this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.bte_OpenFile = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_Ok = new DevExpress.XtraEditors.SimpleButton();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.tb_dbIp = new DevExpress.XtraEditors.TextEdit();
            this.tb_dbAcc = new DevExpress.XtraEditors.TextEdit();
            this.tb_dbPw = new DevExpress.XtraEditors.TextEdit();
            this.tb_dbName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_DataPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bte_OpenFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_dbIp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_dbAcc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_dbPw.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_dbName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(151, 79);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(23, 14);
            this.labelControl4.TabIndex = 28;
            this.labelControl4.Text = "Port";
            // 
            // tb_DataPort
            // 
            this.tb_DataPort.EditValue = "";
            this.tb_DataPort.Location = new System.Drawing.Point(220, 76);
            this.tb_DataPort.Name = "tb_DataPort";
            this.tb_DataPort.Size = new System.Drawing.Size(77, 20);
            this.tb_DataPort.TabIndex = 27;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.tb_dbName);
            this.groupControl1.Controls.Add(this.tb_dbPw);
            this.groupControl1.Controls.Add(this.tb_dbAcc);
            this.groupControl1.Controls.Add(this.tb_dbIp);
            this.groupControl1.Controls.Add(this.labelControl7);
            this.groupControl1.Controls.Add(this.labelControl6);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.cb_Protocol);
            this.groupControl1.Controls.Add(this.radioGroup1);
            this.groupControl1.Controls.Add(this.labelControl5);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.tb_DataPort);
            this.groupControl1.Controls.Add(this.bte_OpenFile);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.ShowCaption = false;
            this.groupControl1.Size = new System.Drawing.Size(359, 222);
            this.groupControl1.TabIndex = 32;
            this.groupControl1.Text = "groupControl1";
            // 
            // cb_Protocol
            // 
            this.cb_Protocol.FormattingEnabled = true;
            this.cb_Protocol.Location = new System.Drawing.Point(220, 42);
            this.cb_Protocol.Name = "cb_Protocol";
            this.cb_Protocol.Size = new System.Drawing.Size(121, 22);
            this.cb_Protocol.TabIndex = 36;
            // 
            // radioGroup1
            // 
            this.radioGroup1.Dock = System.Windows.Forms.DockStyle.Left;
            this.radioGroup1.Location = new System.Drawing.Point(2, 2);
            this.radioGroup1.Name = "radioGroup1";
            this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "File Recoder"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Network Recoder"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "DB Recoder")});
            this.radioGroup1.Size = new System.Drawing.Size(143, 218);
            this.radioGroup1.TabIndex = 34;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(151, 47);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(45, 14);
            this.labelControl5.TabIndex = 31;
            this.labelControl5.Text = "Protocol";
            // 
            // bte_OpenFile
            // 
            this.bte_OpenFile.Location = new System.Drawing.Point(220, 7);
            this.bte_OpenFile.Name = "bte_OpenFile";
            this.bte_OpenFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bte_OpenFile.Size = new System.Drawing.Size(129, 20);
            this.bte_OpenFile.TabIndex = 23;
            this.bte_OpenFile.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_OpenFile_ButtonClick);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(151, 10);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(42, 14);
            this.labelControl3.TabIndex = 24;
            this.labelControl3.Text = "FilePath";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_Ok
            // 
            this.btn_Ok.Location = new System.Drawing.Point(129, 228);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(99, 30);
            this.btn_Ok.TabIndex = 38;
            this.btn_Ok.Text = "확인";
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(151, 116);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(30, 14);
            this.labelControl1.TabIndex = 37;
            this.labelControl1.Text = "DB Ip";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(149, 142);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(65, 14);
            this.labelControl2.TabIndex = 38;
            this.labelControl2.Text = "Db Account";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(151, 169);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(59, 14);
            this.labelControl6.TabIndex = 39;
            this.labelControl6.Text = "Db Passwd";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(151, 195);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(50, 14);
            this.labelControl7.TabIndex = 40;
            this.labelControl7.Text = "DB Name";
            // 
            // tb_dbIp
            // 
            this.tb_dbIp.EditValue = "";
            this.tb_dbIp.Location = new System.Drawing.Point(220, 113);
            this.tb_dbIp.Name = "tb_dbIp";
            this.tb_dbIp.Size = new System.Drawing.Size(77, 20);
            this.tb_dbIp.TabIndex = 41;
            // 
            // tb_dbAcc
            // 
            this.tb_dbAcc.EditValue = "";
            this.tb_dbAcc.Location = new System.Drawing.Point(220, 139);
            this.tb_dbAcc.Name = "tb_dbAcc";
            this.tb_dbAcc.Size = new System.Drawing.Size(77, 20);
            this.tb_dbAcc.TabIndex = 42;
            // 
            // tb_dbPw
            // 
            this.tb_dbPw.EditValue = "";
            this.tb_dbPw.Location = new System.Drawing.Point(220, 166);
            this.tb_dbPw.Name = "tb_dbPw";
            this.tb_dbPw.Size = new System.Drawing.Size(77, 20);
            this.tb_dbPw.TabIndex = 43;
            // 
            // tb_dbName
            // 
            this.tb_dbName.EditValue = "";
            this.tb_dbName.Location = new System.Drawing.Point(220, 192);
            this.tb_dbName.Name = "tb_dbName";
            this.tb_dbName.Size = new System.Drawing.Size(77, 20);
            this.tb_dbName.TabIndex = 44;
            // 
            // RecoderEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 270);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.groupControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecoderEditForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RecoderEditForm";
            ((System.ComponentModel.ISupportInitialize)(this.tb_DataPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bte_OpenFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_dbIp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_dbAcc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_dbPw.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_dbName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit tb_DataPort;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.ButtonEdit bte_OpenFile;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private DevExpress.XtraEditors.SimpleButton btn_Ok;
        private System.Windows.Forms.BindingSource bindingSource1;
        private DevExpress.XtraEditors.RadioGroup radioGroup1;
        private System.Windows.Forms.ComboBox cb_Protocol;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit tb_dbName;
        private DevExpress.XtraEditors.TextEdit tb_dbPw;
        private DevExpress.XtraEditors.TextEdit tb_dbAcc;
        private DevExpress.XtraEditors.TextEdit tb_dbIp;
    }
}