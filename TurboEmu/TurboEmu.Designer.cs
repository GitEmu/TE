
namespace TurboEmu
{
    partial class TurboEmu
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TurboEmu));
            this.GBPackage = new System.Windows.Forms.GroupBox();
            this.CB_Package = new System.Windows.Forms.ComboBox();
            this.CBTrial = new System.Windows.Forms.CheckBox();
            this.PanelHeader = new System.Windows.Forms.Panel();
            this.LabelExit = new System.Windows.Forms.Label();
            this.LB_Version = new System.Windows.Forms.Label();
            this.CBAutostart = new System.Windows.Forms.CheckBox();
            this.BT_Start = new System.Windows.Forms.Button();
            this.GB_Method = new System.Windows.Forms.GroupBox();
            this.LB_Info = new System.Windows.Forms.Label();
            this.LB_Connection = new System.Windows.Forms.Label();
            this.CB_Method = new System.Windows.Forms.ComboBox();
            this.LB_Warning = new System.Windows.Forms.Label();
            this.TR_Warning = new System.Windows.Forms.Timer(this.components);
            this.PB_TurboEmu = new System.Windows.Forms.PictureBox();
            this.GBPackage.SuspendLayout();
            this.PanelHeader.SuspendLayout();
            this.GB_Method.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_TurboEmu)).BeginInit();
            this.SuspendLayout();
            // 
            // GBPackage
            // 
            this.GBPackage.Controls.Add(this.CB_Package);
            this.GBPackage.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.GBPackage.ForeColor = System.Drawing.Color.White;
            this.GBPackage.Location = new System.Drawing.Point(16, 184);
            this.GBPackage.Name = "GBPackage";
            this.GBPackage.Size = new System.Drawing.Size(207, 53);
            this.GBPackage.TabIndex = 0;
            this.GBPackage.TabStop = false;
            this.GBPackage.Text = "Select your Package";
            // 
            // CB_Package
            // 
            this.CB_Package.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.CB_Package.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Package.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_Package.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.CB_Package.ForeColor = System.Drawing.Color.White;
            this.CB_Package.FormattingEnabled = true;
            this.CB_Package.Items.AddRange(new object[] {
            "Free (Tier 1)",
            "Standard (Tier 2)",
            "Unleashed (Tier 3)"});
            this.CB_Package.Location = new System.Drawing.Point(6, 20);
            this.CB_Package.Name = "CB_Package";
            this.CB_Package.Size = new System.Drawing.Size(195, 28);
            this.CB_Package.TabIndex = 0;
            this.CB_Package.SelectedIndexChanged += new System.EventHandler(this.CB_Package_SelectedIndexChanged);
            // 
            // CBTrial
            // 
            this.CBTrial.AutoSize = true;
            this.CBTrial.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.CBTrial.ForeColor = System.Drawing.Color.Red;
            this.CBTrial.Location = new System.Drawing.Point(112, 243);
            this.CBTrial.Name = "CBTrial";
            this.CBTrial.Size = new System.Drawing.Size(115, 24);
            this.CBTrial.TabIndex = 2;
            this.CBTrial.Text = "Enable TRIAL";
            this.CBTrial.UseVisualStyleBackColor = true;
            this.CBTrial.CheckedChanged += new System.EventHandler(this.CBTrial_CheckedChanged);
            // 
            // PanelHeader
            // 
            this.PanelHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(102)))), ((int)(((byte)(0)))));
            this.PanelHeader.Controls.Add(this.LabelExit);
            this.PanelHeader.Controls.Add(this.LB_Version);
            this.PanelHeader.Location = new System.Drawing.Point(0, 0);
            this.PanelHeader.Name = "PanelHeader";
            this.PanelHeader.Size = new System.Drawing.Size(238, 20);
            this.PanelHeader.TabIndex = 5;
            this.PanelHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MovePanel);
            // 
            // LabelExit
            // 
            this.LabelExit.AutoSize = true;
            this.LabelExit.Font = new System.Drawing.Font("Segoe UI", 10.75F);
            this.LabelExit.ForeColor = System.Drawing.Color.White;
            this.LabelExit.Location = new System.Drawing.Point(218, -1);
            this.LabelExit.Name = "LabelExit";
            this.LabelExit.Size = new System.Drawing.Size(16, 20);
            this.LabelExit.TabIndex = 4;
            this.LabelExit.Text = "x";
            this.LabelExit.Click += new System.EventHandler(this.LabelExit_Click);
            // 
            // LB_Version
            // 
            this.LB_Version.AutoSize = true;
            this.LB_Version.Font = new System.Drawing.Font("Segoe UI Semibold", 9.25F, System.Drawing.FontStyle.Bold);
            this.LB_Version.ForeColor = System.Drawing.Color.White;
            this.LB_Version.Location = new System.Drawing.Point(3, 1);
            this.LB_Version.Name = "LB_Version";
            this.LB_Version.Size = new System.Drawing.Size(91, 17);
            this.LB_Version.TabIndex = 0;
            this.LB_Version.Text = "Build {0}.{1}.{2}";
            this.LB_Version.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MovePanel);
            // 
            // CBAutostart
            // 
            this.CBAutostart.AutoSize = true;
            this.CBAutostart.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.CBAutostart.ForeColor = System.Drawing.Color.White;
            this.CBAutostart.Location = new System.Drawing.Point(22, 243);
            this.CBAutostart.Name = "CBAutostart";
            this.CBAutostart.Size = new System.Drawing.Size(89, 24);
            this.CBAutostart.TabIndex = 1;
            this.CBAutostart.Text = "Autostart";
            this.CBAutostart.CheckedChanged += new System.EventHandler(this.CBAutostart_CheckedChanged);
            // 
            // BT_Start
            // 
            this.BT_Start.BackColor = System.Drawing.Color.White;
            this.BT_Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Start.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.BT_Start.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.BT_Start.Location = new System.Drawing.Point(39, 273);
            this.BT_Start.Name = "BT_Start";
            this.BT_Start.Size = new System.Drawing.Size(161, 43);
            this.BT_Start.TabIndex = 3;
            this.BT_Start.Text = "Start TurboHUD";
            this.BT_Start.UseVisualStyleBackColor = false;
            this.BT_Start.Click += new System.EventHandler(this.BT_Start_Click);
            // 
            // GB_Method
            // 
            this.GB_Method.Controls.Add(this.LB_Info);
            this.GB_Method.Controls.Add(this.LB_Connection);
            this.GB_Method.Controls.Add(this.CB_Method);
            this.GB_Method.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GB_Method.ForeColor = System.Drawing.Color.White;
            this.GB_Method.Location = new System.Drawing.Point(16, 98);
            this.GB_Method.Name = "GB_Method";
            this.GB_Method.Size = new System.Drawing.Size(207, 80);
            this.GB_Method.TabIndex = 1;
            this.GB_Method.TabStop = false;
            this.GB_Method.Text = "Method";
            // 
            // LB_Info
            // 
            this.LB_Info.AutoSize = true;
            this.LB_Info.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.LB_Info.ForeColor = System.Drawing.Color.White;
            this.LB_Info.Location = new System.Drawing.Point(6, 51);
            this.LB_Info.Name = "LB_Info";
            this.LB_Info.Size = new System.Drawing.Size(38, 20);
            this.LB_Info.TabIndex = 10;
            this.LB_Info.Text = "Info:";
            // 
            // LB_Connection
            // 
            this.LB_Connection.AutoSize = true;
            this.LB_Connection.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.LB_Connection.ForeColor = System.Drawing.Color.White;
            this.LB_Connection.Location = new System.Drawing.Point(41, 51);
            this.LB_Connection.Name = "LB_Connection";
            this.LB_Connection.Size = new System.Drawing.Size(117, 20);
            this.LB_Connection.TabIndex = 9;
            this.LB_Connection.Text = "Select a method";
            // 
            // CB_Method
            // 
            this.CB_Method.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.CB_Method.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Method.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_Method.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_Method.ForeColor = System.Drawing.Color.White;
            this.CB_Method.FormattingEnabled = true;
            this.CB_Method.Items.AddRange(new object[] {
            "Webserver",
            "Proxy server (MITM)"});
            this.CB_Method.Location = new System.Drawing.Point(6, 20);
            this.CB_Method.Name = "CB_Method";
            this.CB_Method.Size = new System.Drawing.Size(195, 28);
            this.CB_Method.TabIndex = 0;
            this.CB_Method.SelectedIndexChanged += new System.EventHandler(this.CB_Method_SelectedIndexChanged);
            // 
            // LB_Warning
            // 
            this.LB_Warning.AutoSize = true;
            this.LB_Warning.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Warning.ForeColor = System.Drawing.Color.Red;
            this.LB_Warning.Location = new System.Drawing.Point(2, 78);
            this.LB_Warning.Name = "LB_Warning";
            this.LB_Warning.Size = new System.Drawing.Size(439, 20);
            this.LB_Warning.TabIndex = 7;
            this.LB_Warning.Text = "Don\'t use the original TurboHUD.exe from ros-bot.com anymore!";
            // 
            // TR_Warning
            // 
            this.TR_Warning.Interval = 60;
            this.TR_Warning.Tick += new System.EventHandler(this.TR_Warning_Tick);
            // 
            // PB_TurboEmu
            // 
            this.PB_TurboEmu.Image = global::TurboEmu.Properties.Resources.turboemu_logo;
            this.PB_TurboEmu.Location = new System.Drawing.Point(4, 23);
            this.PB_TurboEmu.Name = "PB_TurboEmu";
            this.PB_TurboEmu.Size = new System.Drawing.Size(231, 55);
            this.PB_TurboEmu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PB_TurboEmu.TabIndex = 6;
            this.PB_TurboEmu.TabStop = false;
            // 
            // TurboEmu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(239, 326);
            this.Controls.Add(this.LB_Warning);
            this.Controls.Add(this.PB_TurboEmu);
            this.Controls.Add(this.GB_Method);
            this.Controls.Add(this.CBTrial);
            this.Controls.Add(this.BT_Start);
            this.Controls.Add(this.CBAutostart);
            this.Controls.Add(this.PanelHeader);
            this.Controls.Add(this.GBPackage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TurboEmu";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TurboEmu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TurboEmu_FormClosing);
            this.Load += new System.EventHandler(this.TurboEmu_Load);
            this.Shown += new System.EventHandler(this.TurboEmu_Shown);
            this.GBPackage.ResumeLayout(false);
            this.PanelHeader.ResumeLayout(false);
            this.PanelHeader.PerformLayout();
            this.GB_Method.ResumeLayout(false);
            this.GB_Method.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_TurboEmu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox GBPackage;
        private System.Windows.Forms.CheckBox CBTrial;
        private System.Windows.Forms.Panel PanelHeader;
        private System.Windows.Forms.Label LabelExit;
        private System.Windows.Forms.Label LB_Version;
        private System.Windows.Forms.CheckBox CBAutostart;
        private System.Windows.Forms.ComboBox CB_Package;
        private System.Windows.Forms.Button BT_Start;
        private System.Windows.Forms.GroupBox GB_Method;
        private System.Windows.Forms.ComboBox CB_Method;
        private System.Windows.Forms.Label LB_Connection;
        private System.Windows.Forms.Label LB_Info;
        private System.Windows.Forms.PictureBox PB_TurboEmu;
        private System.Windows.Forms.Label LB_Warning;
        private System.Windows.Forms.Timer TR_Warning;
    }
}

