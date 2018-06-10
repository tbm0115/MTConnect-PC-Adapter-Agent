namespace MTConnectAgentCoreWindows
{
    partial class Form1
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.btnStart = new System.Windows.Forms.Button();
      this.btnStop = new System.Windows.Forms.Button();
      this.taskIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.linkCurrent = new System.Windows.Forms.LinkLabel();
      this.linkProbe = new System.Windows.Forms.LinkLabel();
      this.btnPause = new System.Windows.Forms.Button();
      this.lblStatus = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnStart
      // 
      this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.btnStart.Location = new System.Drawing.Point(9, 109);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(115, 32);
      this.btnStart.TabIndex = 0;
      this.btnStart.Text = "Start Agent";
      this.btnStart.UseVisualStyleBackColor = false;
      this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
      // 
      // btnStop
      // 
      this.btnStop.BackColor = System.Drawing.Color.Red;
      this.btnStop.Location = new System.Drawing.Point(251, 109);
      this.btnStop.Name = "btnStop";
      this.btnStop.Size = new System.Drawing.Size(115, 32);
      this.btnStop.TabIndex = 1;
      this.btnStop.Text = "Stop Agent";
      this.btnStop.UseVisualStyleBackColor = false;
      this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
      // 
      // taskIcon
      // 
      //this.taskIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.taskIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
      this.taskIcon.BalloonTipText = "{Enter Message}";
      this.taskIcon.BalloonTipTitle = "{Enter Title}";
      this.taskIcon.Text = "MTConnect Adapter/Agent";
      this.taskIcon.Visible = true;
      this.taskIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TaskIcon_MouseDoubleClick);
      // 
      // linkCurrent
      // 
      this.linkCurrent.AutoSize = true;
      this.linkCurrent.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.linkCurrent.Location = new System.Drawing.Point(90, 156);
      this.linkCurrent.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.linkCurrent.Name = "linkCurrent";
      this.linkCurrent.Size = new System.Drawing.Size(210, 21);
      this.linkCurrent.TabIndex = 2;
      this.linkCurrent.TabStop = true;
      this.linkCurrent.Text = "http://localhost:5000/current";
      // 
      // linkProbe
      // 
      this.linkProbe.AutoSize = true;
      this.linkProbe.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.linkProbe.Location = new System.Drawing.Point(90, 188);
      this.linkProbe.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.linkProbe.Name = "linkProbe";
      this.linkProbe.Size = new System.Drawing.Size(201, 21);
      this.linkProbe.TabIndex = 3;
      this.linkProbe.TabStop = true;
      this.linkProbe.Text = "http://localhost:5000/probe";
      // 
      // btnPause
      // 
      this.btnPause.BackColor = System.Drawing.Color.Silver;
      this.btnPause.Location = new System.Drawing.Point(130, 109);
      this.btnPause.Name = "btnPause";
      this.btnPause.Size = new System.Drawing.Size(115, 32);
      this.btnPause.TabIndex = 4;
      this.btnPause.Text = "Pause Agent";
      this.btnPause.UseVisualStyleBackColor = false;
      this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.Location = new System.Drawing.Point(168, 44);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(37, 13);
      this.lblStatus.TabIndex = 5;
      this.lblStatus.Text = "Status";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(372, 218);
      this.Controls.Add(this.lblStatus);
      this.Controls.Add(this.btnPause);
      this.Controls.Add(this.linkProbe);
      this.Controls.Add(this.linkCurrent);
      this.Controls.Add(this.btnStop);
      this.Controls.Add(this.btnStart);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      // 
      // taskIcon
      // 
      this.taskIcon.Icon = this.Icon;
      //
      // Form1
      //
      this.MaximizeBox = false;
      this.Name = "Form1";
      this.Text = "MTConnect Core Agent";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

    private void LinkProbe_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start("http://localhost:5000/probe");
    }

    private void LinkCurrent_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start("http://localhost:5000/current");
    }

    private void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
    {
      this.btnStop_Click(this.btnStop, null);
      this.taskIcon.Dispose();
    }

    private void TaskIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      this.Form_Show();
    }
    

    #endregion

    private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
    private System.Windows.Forms.LinkLabel linkCurrent;
    private System.Windows.Forms.LinkLabel linkProbe;
    private System.Windows.Forms.Button btnPause;
    public System.Windows.Forms.NotifyIcon taskIcon;
    private System.Windows.Forms.Label lblStatus;
  }
}

