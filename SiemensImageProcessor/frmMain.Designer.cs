namespace SiemensImageProcessor
{
    partial class frmMain
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

        #region Windows Form Designer GenerateАd code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.sbtnRun = new DevExpress.XtraEditors.SimpleButton();
            this.bwCommon = new System.ComponentModel.BackgroundWorker();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.meLog = new DevExpress.XtraEditors.MemoEdit();
            this.sbtnClose = new DevExpress.XtraEditors.SimpleButton();
            this.teJpgFolderPath = new DevExpress.XtraEditors.TextEdit();
            this.sbtnSelectJpgFolderPath = new DevExpress.XtraEditors.SimpleButton();
            this.fbdJpg = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.meLog.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teJpgFolderPath.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // sbtnRun
            // 
            this.sbtnRun.Image = ((System.Drawing.Image)(resources.GetObject("sbtnRun.Image")));
            this.sbtnRun.Location = new System.Drawing.Point(369, 279);
            this.sbtnRun.Name = "sbtnRun";
            this.sbtnRun.Size = new System.Drawing.Size(151, 41);
            this.sbtnRun.TabIndex = 5;
            this.sbtnRun.Text = "СТАРТ";
            this.sbtnRun.Click += new System.EventHandler(this.sbtnRun_Click);
            // 
            // bwCommon
            // 
            this.bwCommon.WorkerReportsProgress = true;
            this.bwCommon.WorkerSupportsCancellation = true;
            this.bwCommon.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwCommon_DoWork);
            this.bwCommon.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwCommon_ProgressChanged);
            this.bwCommon.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwCommon_RunWorkerCompleted);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl2.Location = new System.Drawing.Point(18, 14);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(166, 16);
            this.labelControl2.TabIndex = 17;
            this.labelControl2.Text = "Директория с файлами JPG:";
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl6.Location = new System.Drawing.Point(18, 46);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(155, 16);
            this.labelControl6.TabIndex = 23;
            this.labelControl6.Text = "Ход выполнения задания:";
            // 
            // meLog
            // 
            this.meLog.Location = new System.Drawing.Point(18, 68);
            this.meLog.Name = "meLog";
            this.meLog.Size = new System.Drawing.Size(659, 205);
            this.meLog.TabIndex = 4;
            this.meLog.EditValueChanged += new System.EventHandler(this.meLog_EditValueChanged);
            // 
            // sbtnClose
            // 
            this.sbtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.sbtnClose.Image = ((System.Drawing.Image)(resources.GetObject("sbtnClose.Image")));
            this.sbtnClose.Location = new System.Drawing.Point(526, 279);
            this.sbtnClose.Name = "sbtnClose";
            this.sbtnClose.Size = new System.Drawing.Size(151, 41);
            this.sbtnClose.TabIndex = 6;
            this.sbtnClose.Text = "ЗАКРЫТЬ";
            this.sbtnClose.Click += new System.EventHandler(this.sbtnClose_Click);
            // 
            // teJpgFolderPath
            // 
            this.teJpgFolderPath.Location = new System.Drawing.Point(188, 11);
            this.teJpgFolderPath.Name = "teJpgFolderPath";
            this.teJpgFolderPath.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.teJpgFolderPath.Properties.Appearance.Options.UseFont = true;
            this.teJpgFolderPath.Size = new System.Drawing.Size(462, 22);
            this.teJpgFolderPath.TabIndex = 0;
            // 
            // sbtnSelectJpgFolderPath
            // 
            this.sbtnSelectJpgFolderPath.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.sbtnSelectJpgFolderPath.Appearance.Options.UseFont = true;
            this.sbtnSelectJpgFolderPath.Location = new System.Drawing.Point(654, 10);
            this.sbtnSelectJpgFolderPath.Name = "sbtnSelectJpgFolderPath";
            this.sbtnSelectJpgFolderPath.Size = new System.Drawing.Size(23, 23);
            this.sbtnSelectJpgFolderPath.TabIndex = 1;
            this.sbtnSelectJpgFolderPath.Text = "...";
            this.sbtnSelectJpgFolderPath.Click += new System.EventHandler(this.sbtnSelectJpgFolderPath_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.sbtnClose;
            this.ClientSize = new System.Drawing.Size(694, 331);
            this.Controls.Add(this.sbtnSelectJpgFolderPath);
            this.Controls.Add(this.teJpgFolderPath);
            this.Controls.Add(this.sbtnClose);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.meLog);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.sbtnRun);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Siemens Image Processor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.meLog.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teJpgFolderPath.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton sbtnRun;
        private System.ComponentModel.BackgroundWorker bwCommon;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.MemoEdit meLog;
        private DevExpress.XtraEditors.SimpleButton sbtnClose;
        private DevExpress.XtraEditors.TextEdit teJpgFolderPath;
        private DevExpress.XtraEditors.SimpleButton sbtnSelectJpgFolderPath;
        private System.Windows.Forms.FolderBrowserDialog fbdJpg;
    }
}

