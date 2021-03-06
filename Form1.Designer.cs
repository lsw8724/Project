﻿namespace TestCms1
{
    partial class WaveMonitor
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaveMonitor));
            this.TimeChart = new Steema.TeeChart.TChart();
            this.TimeSerise1 = new Steema.TeeChart.Styles.FastLine();
            this.TimeSerise2 = new Steema.TeeChart.Styles.FastLine();
            this.TimeSerise3 = new Steema.TeeChart.Styles.FastLine();
            this.TimeSerise4 = new Steema.TeeChart.Styles.FastLine();
            this.TimeSerise5 = new Steema.TeeChart.Styles.FastLine();
            this.TimeSerise6 = new Steema.TeeChart.Styles.FastLine();
            this.TimeSerise7 = new Steema.TeeChart.Styles.FastLine();
            this.TimeSerise8 = new Steema.TeeChart.Styles.FastLine();
            this.FFTChart = new Steema.TeeChart.TChart();
            this.FFTSerise1 = new Steema.TeeChart.Styles.FastLine();
            this.FFTSerise2 = new Steema.TeeChart.Styles.FastLine();
            this.FFTSerise3 = new Steema.TeeChart.Styles.FastLine();
            this.FFTSerise4 = new Steema.TeeChart.Styles.FastLine();
            this.FFTSerise5 = new Steema.TeeChart.Styles.FastLine();
            this.FFTSerise6 = new Steema.TeeChart.Styles.FastLine();
            this.FFTSerise7 = new Steema.TeeChart.Styles.FastLine();
            this.FFTSerise8 = new Steema.TeeChart.Styles.FastLine();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.btn_integrate = new DevExpress.XtraEditors.SimpleButton();
            this.btn_raw = new DevExpress.XtraEditors.SimpleButton();
            this.btnStart = new DevExpress.XtraEditors.SimpleButton();
            this.gr_Recoder = new DevExpress.XtraEditors.GroupControl();
            this.lb_Recoder = new System.Windows.Forms.ListBox();
            this.btn_DelRecode = new DevExpress.XtraEditors.SimpleButton();
            this.btn_AddRecode = new DevExpress.XtraEditors.SimpleButton();
            this.gr_Measure = new DevExpress.XtraEditors.GroupControl();
            this.lb_Measure = new System.Windows.Forms.ListBox();
            this.btn_DelMeasure = new DevExpress.XtraEditors.SimpleButton();
            this.btn_AddMeasure = new DevExpress.XtraEditors.SimpleButton();
            this.Gr_Receiver = new DevExpress.XtraEditors.GroupControl();
            this.lb_Receiver = new System.Windows.Forms.ListBox();
            this.btn_DelReceive = new DevExpress.XtraEditors.SimpleButton();
            this.btn_AddRceive = new DevExpress.XtraEditors.SimpleButton();
            this.btnStop = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TrendChart = new Steema.TeeChart.TChart();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gr_Recoder)).BeginInit();
            this.gr_Recoder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gr_Measure)).BeginInit();
            this.gr_Measure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Gr_Receiver)).BeginInit();
            this.Gr_Receiver.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TimeChart
            // 
            // 
            // 
            // 
            this.TimeChart.Aspect.View3D = false;
            this.TimeChart.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // 
            // 
            this.TimeChart.Header.Visible = false;
            // 
            // 
            // 
            this.TimeChart.Legend.CheckBoxes = true;
            // 
            // 
            // 
            // 
            // 
            // 
            this.TimeChart.Legend.Font.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(192)))), ((int)(((byte)(93)))));
            this.TimeChart.Legend.FontSeriesColor = true;
            this.TimeChart.Legend.LegendStyle = Steema.TeeChart.LegendStyles.Series;
            this.TimeChart.Location = new System.Drawing.Point(3, 3);
            this.TimeChart.Name = "TimeChart";
            this.TimeChart.Series.Add(this.TimeSerise1);
            this.TimeChart.Series.Add(this.TimeSerise2);
            this.TimeChart.Series.Add(this.TimeSerise3);
            this.TimeChart.Series.Add(this.TimeSerise4);
            this.TimeChart.Series.Add(this.TimeSerise5);
            this.TimeChart.Series.Add(this.TimeSerise6);
            this.TimeChart.Series.Add(this.TimeSerise7);
            this.TimeChart.Series.Add(this.TimeSerise8);
            this.TimeChart.Size = new System.Drawing.Size(1127, 173);
            this.TimeChart.TabIndex = 4;
            // 
            // TimeSerise1
            // 
            this.TimeSerise1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(102)))), ((int)(((byte)(163)))));
            this.TimeSerise1.ColorEach = false;
            // 
            // 
            // 
            this.TimeSerise1.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(102)))), ((int)(((byte)(163)))));
            this.TimeSerise1.Title = "Ch1";
            this.TimeSerise1.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            // 
            // 
            // 
            this.TimeSerise1.XValues.DataMember = "X";
            // 
            // 
            // 
            this.TimeSerise1.YValues.DataMember = "Y";
            // 
            // TimeSerise2
            // 
            this.TimeSerise2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(53)))));
            this.TimeSerise2.ColorEach = false;
            // 
            // 
            // 
            this.TimeSerise2.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(53)))));
            this.TimeSerise2.SeriesData = resources.GetString("TimeSerise2.SeriesData");
            this.TimeSerise2.Title = "Ch2";
            this.TimeSerise2.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.TimeSerise2.Visible = false;
            // 
            // 
            // 
            this.TimeSerise2.XValues.DataMember = "X";
            // 
            // 
            // 
            this.TimeSerise2.YValues.DataMember = "Y";
            // 
            // TimeSerise3
            // 
            this.TimeSerise3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(76)))), ((int)(((byte)(20)))));
            this.TimeSerise3.ColorEach = false;
            // 
            // 
            // 
            this.TimeSerise3.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(76)))), ((int)(((byte)(20)))));
            this.TimeSerise3.SeriesData = resources.GetString("TimeSerise3.SeriesData");
            this.TimeSerise3.Title = "Ch3";
            this.TimeSerise3.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.TimeSerise3.Visible = false;
            // 
            // 
            // 
            this.TimeSerise3.XValues.DataMember = "X";
            // 
            // 
            // 
            this.TimeSerise3.YValues.DataMember = "Y";
            // 
            // TimeSerise4
            // 
            this.TimeSerise4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(151)))), ((int)(((byte)(168)))));
            this.TimeSerise4.ColorEach = false;
            // 
            // 
            // 
            this.TimeSerise4.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(151)))), ((int)(((byte)(168)))));
            this.TimeSerise4.SeriesData = resources.GetString("TimeSerise4.SeriesData");
            this.TimeSerise4.Title = "Ch4";
            this.TimeSerise4.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.TimeSerise4.Visible = false;
            // 
            // 
            // 
            this.TimeSerise4.XValues.DataMember = "X";
            // 
            // 
            // 
            this.TimeSerise4.YValues.DataMember = "Y";
            // 
            // TimeSerise5
            // 
            this.TimeSerise5.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(64)))), ((int)(((byte)(107)))));
            this.TimeSerise5.ColorEach = false;
            // 
            // 
            // 
            this.TimeSerise5.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(64)))), ((int)(((byte)(107)))));
            this.TimeSerise5.SeriesData = resources.GetString("TimeSerise5.SeriesData");
            this.TimeSerise5.Title = "Ch5";
            this.TimeSerise5.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.TimeSerise5.Visible = false;
            // 
            // 
            // 
            this.TimeSerise5.XValues.DataMember = "X";
            // 
            // 
            // 
            this.TimeSerise5.YValues.DataMember = "Y";
            // 
            // TimeSerise6
            // 
            this.TimeSerise6.Color = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(123)))), ((int)(((byte)(99)))));
            this.TimeSerise6.ColorEach = false;
            // 
            // 
            // 
            this.TimeSerise6.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(123)))), ((int)(((byte)(99)))));
            this.TimeSerise6.SeriesData = resources.GetString("TimeSerise6.SeriesData");
            this.TimeSerise6.Title = "Ch6";
            this.TimeSerise6.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.TimeSerise6.Visible = false;
            // 
            // 
            // 
            this.TimeSerise6.XValues.DataMember = "X";
            // 
            // 
            // 
            this.TimeSerise6.YValues.DataMember = "Y";
            // 
            // TimeSerise7
            // 
            this.TimeSerise7.Color = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(8)))), ((int)(((byte)(14)))));
            this.TimeSerise7.ColorEach = false;
            // 
            // 
            // 
            this.TimeSerise7.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(8)))), ((int)(((byte)(14)))));
            this.TimeSerise7.SeriesData = resources.GetString("TimeSerise7.SeriesData");
            this.TimeSerise7.Title = "Ch7";
            this.TimeSerise7.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.TimeSerise7.Visible = false;
            // 
            // 
            // 
            this.TimeSerise7.XValues.DataMember = "X";
            // 
            // 
            // 
            this.TimeSerise7.YValues.DataMember = "Y";
            // 
            // TimeSerise8
            // 
            this.TimeSerise8.Color = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(192)))), ((int)(((byte)(93)))));
            this.TimeSerise8.ColorEach = false;
            // 
            // 
            // 
            this.TimeSerise8.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(192)))), ((int)(((byte)(93)))));
            this.TimeSerise8.SeriesData = resources.GetString("TimeSerise8.SeriesData");
            this.TimeSerise8.Title = "Ch8";
            this.TimeSerise8.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.TimeSerise8.Visible = false;
            // 
            // 
            // 
            this.TimeSerise8.XValues.DataMember = "X";
            // 
            // 
            // 
            this.TimeSerise8.YValues.DataMember = "Y";
            // 
            // FFTChart
            // 
            // 
            // 
            // 
            this.FFTChart.Aspect.View3D = false;
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            this.FFTChart.Axes.Bottom.Labels.Style = Steema.TeeChart.AxisLabelStyle.Value;
            this.FFTChart.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // 
            // 
            this.FFTChart.Header.Visible = false;
            // 
            // 
            // 
            this.FFTChart.Legend.CheckBoxes = true;
            // 
            // 
            // 
            // 
            // 
            // 
            this.FFTChart.Legend.Font.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(192)))), ((int)(((byte)(93)))));
            this.FFTChart.Legend.FontSeriesColor = true;
            this.FFTChart.Legend.LegendStyle = Steema.TeeChart.LegendStyles.Series;
            this.FFTChart.Location = new System.Drawing.Point(3, 182);
            this.FFTChart.Name = "FFTChart";
            this.FFTChart.Series.Add(this.FFTSerise1);
            this.FFTChart.Series.Add(this.FFTSerise2);
            this.FFTChart.Series.Add(this.FFTSerise3);
            this.FFTChart.Series.Add(this.FFTSerise4);
            this.FFTChart.Series.Add(this.FFTSerise5);
            this.FFTChart.Series.Add(this.FFTSerise6);
            this.FFTChart.Series.Add(this.FFTSerise7);
            this.FFTChart.Series.Add(this.FFTSerise8);
            this.FFTChart.Size = new System.Drawing.Size(1127, 173);
            this.FFTChart.TabIndex = 2;
            // 
            // FFTSerise1
            // 
            this.FFTSerise1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(102)))), ((int)(((byte)(163)))));
            this.FFTSerise1.ColorEach = false;
            // 
            // 
            // 
            this.FFTSerise1.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(102)))), ((int)(((byte)(163)))));
            this.FFTSerise1.Title = "Ch1";
            this.FFTSerise1.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            // 
            // 
            // 
            this.FFTSerise1.XValues.DataMember = "X";
            // 
            // 
            // 
            this.FFTSerise1.YValues.DataMember = "Y";
            // 
            // FFTSerise2
            // 
            this.FFTSerise2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(53)))));
            this.FFTSerise2.ColorEach = false;
            // 
            // 
            // 
            this.FFTSerise2.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(53)))));
            this.FFTSerise2.SeriesData = resources.GetString("FFTSerise2.SeriesData");
            this.FFTSerise2.Title = "Ch2";
            this.FFTSerise2.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.FFTSerise2.Visible = false;
            // 
            // 
            // 
            this.FFTSerise2.XValues.DataMember = "X";
            // 
            // 
            // 
            this.FFTSerise2.YValues.DataMember = "Y";
            // 
            // FFTSerise3
            // 
            this.FFTSerise3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(76)))), ((int)(((byte)(20)))));
            this.FFTSerise3.ColorEach = false;
            // 
            // 
            // 
            this.FFTSerise3.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(76)))), ((int)(((byte)(20)))));
            this.FFTSerise3.SeriesData = resources.GetString("FFTSerise3.SeriesData");
            this.FFTSerise3.Title = "Ch3";
            this.FFTSerise3.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.FFTSerise3.Visible = false;
            // 
            // 
            // 
            this.FFTSerise3.XValues.DataMember = "X";
            // 
            // 
            // 
            this.FFTSerise3.YValues.DataMember = "Y";
            // 
            // FFTSerise4
            // 
            this.FFTSerise4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(151)))), ((int)(((byte)(168)))));
            this.FFTSerise4.ColorEach = false;
            // 
            // 
            // 
            this.FFTSerise4.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(151)))), ((int)(((byte)(168)))));
            this.FFTSerise4.SeriesData = resources.GetString("FFTSerise4.SeriesData");
            this.FFTSerise4.Title = "Ch4";
            this.FFTSerise4.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.FFTSerise4.Visible = false;
            // 
            // 
            // 
            this.FFTSerise4.XValues.DataMember = "X";
            // 
            // 
            // 
            this.FFTSerise4.YValues.DataMember = "Y";
            // 
            // FFTSerise5
            // 
            this.FFTSerise5.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(64)))), ((int)(((byte)(107)))));
            this.FFTSerise5.ColorEach = false;
            // 
            // 
            // 
            this.FFTSerise5.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(64)))), ((int)(((byte)(107)))));
            this.FFTSerise5.SeriesData = resources.GetString("FFTSerise5.SeriesData");
            this.FFTSerise5.Title = "Ch5";
            this.FFTSerise5.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.FFTSerise5.Visible = false;
            // 
            // 
            // 
            this.FFTSerise5.XValues.DataMember = "X";
            // 
            // 
            // 
            this.FFTSerise5.YValues.DataMember = "Y";
            // 
            // FFTSerise6
            // 
            this.FFTSerise6.Color = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(123)))), ((int)(((byte)(99)))));
            this.FFTSerise6.ColorEach = false;
            // 
            // 
            // 
            this.FFTSerise6.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(123)))), ((int)(((byte)(99)))));
            this.FFTSerise6.SeriesData = resources.GetString("FFTSerise6.SeriesData");
            this.FFTSerise6.Title = "Ch6";
            this.FFTSerise6.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.FFTSerise6.Visible = false;
            // 
            // 
            // 
            this.FFTSerise6.XValues.DataMember = "X";
            // 
            // 
            // 
            this.FFTSerise6.YValues.DataMember = "Y";
            // 
            // FFTSerise7
            // 
            this.FFTSerise7.Color = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(8)))), ((int)(((byte)(14)))));
            this.FFTSerise7.ColorEach = false;
            // 
            // 
            // 
            this.FFTSerise7.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(8)))), ((int)(((byte)(14)))));
            this.FFTSerise7.SeriesData = resources.GetString("FFTSerise7.SeriesData");
            this.FFTSerise7.Title = "Ch7";
            this.FFTSerise7.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.FFTSerise7.Visible = false;
            // 
            // 
            // 
            this.FFTSerise7.XValues.DataMember = "X";
            // 
            // 
            // 
            this.FFTSerise7.YValues.DataMember = "Y";
            // 
            // FFTSerise8
            // 
            this.FFTSerise8.Color = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(192)))), ((int)(((byte)(93)))));
            this.FFTSerise8.ColorEach = false;
            // 
            // 
            // 
            this.FFTSerise8.LinePen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(192)))), ((int)(((byte)(93)))));
            this.FFTSerise8.SeriesData = resources.GetString("FFTSerise8.SeriesData");
            this.FFTSerise8.Title = "Ch8";
            this.FFTSerise8.TreatNulls = Steema.TeeChart.Styles.TreatNullsStyle.Ignore;
            this.FFTSerise8.Visible = false;
            // 
            // 
            // 
            this.FFTSerise8.XValues.DataMember = "X";
            // 
            // 
            // 
            this.FFTSerise8.YValues.DataMember = "Y";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.btn_integrate);
            this.groupControl1.Controls.Add(this.btn_raw);
            this.groupControl1.Controls.Add(this.btnStart);
            this.groupControl1.Controls.Add(this.gr_Recoder);
            this.groupControl1.Controls.Add(this.gr_Measure);
            this.groupControl1.Controls.Add(this.Gr_Receiver);
            this.groupControl1.Controls.Add(this.btnStop);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(3, 540);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.ShowCaption = false;
            this.groupControl1.Size = new System.Drawing.Size(1127, 149);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "groupControl1";
            // 
            // btn_integrate
            // 
            this.btn_integrate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_integrate.Location = new System.Drawing.Point(771, 78);
            this.btn_integrate.Name = "btn_integrate";
            this.btn_integrate.Size = new System.Drawing.Size(124, 56);
            this.btn_integrate.TabIndex = 35;
            this.btn_integrate.Tag = "";
            this.btn_integrate.Text = "Raw";
            this.btn_integrate.Click += new System.EventHandler(this.Raw_Click);
            // 
            // btn_raw
            // 
            this.btn_raw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_raw.Location = new System.Drawing.Point(771, 16);
            this.btn_raw.Name = "btn_raw";
            this.btn_raw.Size = new System.Drawing.Size(124, 56);
            this.btn_raw.TabIndex = 34;
            this.btn_raw.Tag = "";
            this.btn_raw.Text = "Integrate";
            this.btn_raw.Click += new System.EventHandler(this.Integrate_Click);
            // 
            // btnStart
            // 
            this.btnStart.Appearance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnStart.Appearance.Options.UseFont = true;
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnStart.Location = new System.Drawing.Point(906, 2);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(110, 145);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Recive\r\nStart";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gr_Recoder
            // 
            this.gr_Recoder.Controls.Add(this.lb_Recoder);
            this.gr_Recoder.Controls.Add(this.btn_DelRecode);
            this.gr_Recoder.Controls.Add(this.btn_AddRecode);
            this.gr_Recoder.Dock = System.Windows.Forms.DockStyle.Left;
            this.gr_Recoder.Location = new System.Drawing.Point(502, 2);
            this.gr_Recoder.Name = "gr_Recoder";
            this.gr_Recoder.Size = new System.Drawing.Size(258, 145);
            this.gr_Recoder.TabIndex = 33;
            this.gr_Recoder.Text = "Recoder";
            // 
            // lb_Recoder
            // 
            this.lb_Recoder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_Recoder.FormattingEnabled = true;
            this.lb_Recoder.ItemHeight = 12;
            this.lb_Recoder.Location = new System.Drawing.Point(2, 22);
            this.lb_Recoder.Name = "lb_Recoder";
            this.lb_Recoder.Size = new System.Drawing.Size(256, 88);
            this.lb_Recoder.TabIndex = 22;
            // 
            // btn_DelRecode
            // 
            this.btn_DelRecode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_DelRecode.Location = new System.Drawing.Point(129, 117);
            this.btn_DelRecode.Name = "btn_DelRecode";
            this.btn_DelRecode.Size = new System.Drawing.Size(124, 23);
            this.btn_DelRecode.TabIndex = 28;
            this.btn_DelRecode.Tag = "";
            this.btn_DelRecode.Text = "Delete";
            // 
            // btn_AddRecode
            // 
            this.btn_AddRecode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_AddRecode.Location = new System.Drawing.Point(5, 117);
            this.btn_AddRecode.Name = "btn_AddRecode";
            this.btn_AddRecode.Size = new System.Drawing.Size(118, 23);
            this.btn_AddRecode.TabIndex = 25;
            this.btn_AddRecode.Text = "Add";
            // 
            // gr_Measure
            // 
            this.gr_Measure.Controls.Add(this.lb_Measure);
            this.gr_Measure.Controls.Add(this.btn_DelMeasure);
            this.gr_Measure.Controls.Add(this.btn_AddMeasure);
            this.gr_Measure.Dock = System.Windows.Forms.DockStyle.Left;
            this.gr_Measure.Location = new System.Drawing.Point(245, 2);
            this.gr_Measure.Name = "gr_Measure";
            this.gr_Measure.Size = new System.Drawing.Size(257, 145);
            this.gr_Measure.TabIndex = 32;
            this.gr_Measure.Text = "Measure";
            // 
            // lb_Measure
            // 
            this.lb_Measure.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_Measure.FormattingEnabled = true;
            this.lb_Measure.ItemHeight = 12;
            this.lb_Measure.Location = new System.Drawing.Point(2, 22);
            this.lb_Measure.Name = "lb_Measure";
            this.lb_Measure.Size = new System.Drawing.Size(255, 88);
            this.lb_Measure.TabIndex = 22;
            // 
            // btn_DelMeasure
            // 
            this.btn_DelMeasure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_DelMeasure.Location = new System.Drawing.Point(135, 117);
            this.btn_DelMeasure.Name = "btn_DelMeasure";
            this.btn_DelMeasure.Size = new System.Drawing.Size(116, 23);
            this.btn_DelMeasure.TabIndex = 28;
            this.btn_DelMeasure.Tag = "";
            this.btn_DelMeasure.Text = "Delete";
            // 
            // btn_AddMeasure
            // 
            this.btn_AddMeasure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_AddMeasure.Location = new System.Drawing.Point(6, 117);
            this.btn_AddMeasure.Name = "btn_AddMeasure";
            this.btn_AddMeasure.Size = new System.Drawing.Size(123, 23);
            this.btn_AddMeasure.TabIndex = 25;
            this.btn_AddMeasure.Text = "Add";
            // 
            // Gr_Receiver
            // 
            this.Gr_Receiver.Controls.Add(this.lb_Receiver);
            this.Gr_Receiver.Controls.Add(this.btn_DelReceive);
            this.Gr_Receiver.Controls.Add(this.btn_AddRceive);
            this.Gr_Receiver.Dock = System.Windows.Forms.DockStyle.Left;
            this.Gr_Receiver.Location = new System.Drawing.Point(2, 2);
            this.Gr_Receiver.Name = "Gr_Receiver";
            this.Gr_Receiver.Size = new System.Drawing.Size(243, 145);
            this.Gr_Receiver.TabIndex = 31;
            this.Gr_Receiver.Text = "Receiver";
            // 
            // lb_Receiver
            // 
            this.lb_Receiver.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lb_Receiver.FormattingEnabled = true;
            this.lb_Receiver.ItemHeight = 12;
            this.lb_Receiver.Location = new System.Drawing.Point(2, 22);
            this.lb_Receiver.Name = "lb_Receiver";
            this.lb_Receiver.Size = new System.Drawing.Size(241, 88);
            this.lb_Receiver.TabIndex = 22;
            // 
            // btn_DelReceive
            // 
            this.btn_DelReceive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_DelReceive.Location = new System.Drawing.Point(128, 117);
            this.btn_DelReceive.Name = "btn_DelReceive";
            this.btn_DelReceive.Size = new System.Drawing.Size(110, 23);
            this.btn_DelReceive.TabIndex = 28;
            this.btn_DelReceive.Tag = "";
            this.btn_DelReceive.Text = "Delete";
            // 
            // btn_AddRceive
            // 
            this.btn_AddRceive.Location = new System.Drawing.Point(7, 117);
            this.btn_AddRceive.Name = "btn_AddRceive";
            this.btn_AddRceive.Size = new System.Drawing.Size(115, 23);
            this.btn_AddRceive.TabIndex = 25;
            this.btn_AddRceive.Tag = "";
            this.btn_AddRceive.Text = "Add";
            // 
            // btnStop
            // 
            this.btnStop.Appearance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.btnStop.Appearance.Options.UseFont = true;
            this.btnStop.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(1016, 2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(109, 145);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Recive\r\nStop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupControl1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.FFTChart, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TimeChart, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TrendChart, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.01156F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.25434F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1133, 692);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // TrendChart
            // 
            // 
            // 
            // 
            this.TrendChart.Aspect.View3D = false;
            this.TrendChart.Cursor = System.Windows.Forms.Cursors.Default;
            this.TrendChart.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // 
            // 
            this.TrendChart.Header.Visible = false;
            // 
            // 
            // 
            this.TrendChart.Legend.CheckBoxes = true;
            this.TrendChart.Legend.FontSeriesColor = true;
            this.TrendChart.Location = new System.Drawing.Point(3, 361);
            this.TrendChart.Name = "TrendChart";
            // 
            // 
            // 
            this.TrendChart.Panel.MarginBottom = 8D;
            this.TrendChart.Size = new System.Drawing.Size(1127, 173);
            this.TrendChart.TabIndex = 5;
            // 
            // WaveMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1133, 692);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WaveMonitor";
            this.ShowIcon = false;
            this.Text = "Wave Monitor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gr_Recoder)).EndInit();
            this.gr_Recoder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gr_Measure)).EndInit();
            this.gr_Measure.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Gr_Receiver)).EndInit();
            this.Gr_Receiver.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Steema.TeeChart.TChart TimeChart;
        private Steema.TeeChart.Styles.FastLine TimeSerise1;
        private Steema.TeeChart.Styles.FastLine TimeSerise2;
        private Steema.TeeChart.Styles.FastLine TimeSerise3;
        private Steema.TeeChart.Styles.FastLine TimeSerise4;
        private Steema.TeeChart.Styles.FastLine TimeSerise5;
        private Steema.TeeChart.Styles.FastLine TimeSerise6;
        private Steema.TeeChart.Styles.FastLine TimeSerise7;
        private Steema.TeeChart.Styles.FastLine TimeSerise8;
        private Steema.TeeChart.TChart FFTChart;
        private Steema.TeeChart.Styles.FastLine FFTSerise1;
        private Steema.TeeChart.Styles.FastLine FFTSerise2;
        private Steema.TeeChart.Styles.FastLine FFTSerise3;
        private Steema.TeeChart.Styles.FastLine FFTSerise4;
        private Steema.TeeChart.Styles.FastLine FFTSerise5;
        private Steema.TeeChart.Styles.FastLine FFTSerise6;
        private Steema.TeeChart.Styles.FastLine FFTSerise7;
        private Steema.TeeChart.Styles.FastLine FFTSerise8;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.GroupControl gr_Recoder;
        public System.Windows.Forms.ListBox lb_Recoder;
        private DevExpress.XtraEditors.SimpleButton btn_DelRecode;
        private DevExpress.XtraEditors.SimpleButton btn_AddRecode;
        private DevExpress.XtraEditors.GroupControl gr_Measure;
        public System.Windows.Forms.ListBox lb_Measure;
        private DevExpress.XtraEditors.SimpleButton btn_DelMeasure;
        private DevExpress.XtraEditors.SimpleButton btn_AddMeasure;
        private DevExpress.XtraEditors.GroupControl Gr_Receiver;
        public System.Windows.Forms.ListBox lb_Receiver;
        private DevExpress.XtraEditors.SimpleButton btn_DelReceive;
        private DevExpress.XtraEditors.SimpleButton btn_AddRceive;
        private DevExpress.XtraEditors.SimpleButton btnStop;
        private DevExpress.XtraEditors.SimpleButton btnStart;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Steema.TeeChart.TChart TrendChart;
        private DevExpress.XtraEditors.SimpleButton btn_integrate;
        private DevExpress.XtraEditors.SimpleButton btn_raw;
    }
}

