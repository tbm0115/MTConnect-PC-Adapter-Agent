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
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.taskIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.linkCurrent = new System.Windows.Forms.LinkLabel();
      this.linkProbe = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
      this.button1.Location = new System.Drawing.Point(68, 43);
      this.button1.Margin = new System.Windows.Forms.Padding(4);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(153, 39);
      this.button1.TabIndex = 0;
      this.button1.Text = "Start Agent";
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.BackColor = System.Drawing.Color.Red;
      this.button2.Location = new System.Drawing.Point(273, 43);
      this.button2.Margin = new System.Windows.Forms.Padding(4);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(153, 39);
      this.button2.TabIndex = 1;
      this.button2.Text = "Stop Agent";
      this.button2.UseVisualStyleBackColor = false;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // taskIcon
      // 
      this.taskIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("taskIcon.Icon")));
      this.taskIcon.Text = "MTConnect Adapter/Agent";
      this.taskIcon.Visible = true;
      // 
      // linkCurrent
      // 
      this.linkCurrent.AutoSize = true;
      this.linkCurrent.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.linkCurrent.Location = new System.Drawing.Point(115, 104);
      this.linkCurrent.Name = "linkCurrent";
      this.linkCurrent.Size = new System.Drawing.Size(266, 28);
      this.linkCurrent.TabIndex = 2;
      this.linkCurrent.TabStop = true;
      this.linkCurrent.Text = "http://localhost:5000/current";
      this.linkCurrent.LinkClicked += LinkCurrent_LinkClicked;
      // 
      // linkProbe
      // 
      this.linkProbe.AutoSize = true;
      this.linkProbe.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.linkProbe.Location = new System.Drawing.Point(115, 143);
      this.linkProbe.Name = "linkProbe";
      this.linkProbe.Size = new System.Drawing.Size(257, 28);
      this.linkProbe.TabIndex = 3;
      this.linkProbe.TabStop = true;
      this.linkProbe.Text = "http://localhost:5000/probe";
      this.linkProbe.LinkClicked += LinkProbe_LinkClicked;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(496, 180);
      this.Controls.Add(this.linkProbe);
      this.Controls.Add(this.linkCurrent);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MaximizeBox = false;
      this.Name = "Form1";
      this.Text = "MTConnect Core Agent";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.Resize += new System.EventHandler(this.Form1_Resize);
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
      this.button2_Click(this.button2, null);
    }

    private void TaskIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      this.ShowInTaskbar = true;
      this.Show();
    }
    
    private void Form1_Resize(object sender, System.EventArgs e)
    {
      this.ShowInTaskbar = true;
      if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
      {
        Hide();
        this.ShowInTaskbar = false;
        this.taskIcon.Visible = true;
        this.taskIcon.BalloonTipText = "Click the MTConnect PC Agent/Adapter Icon to show again...";
        this.taskIcon.BalloonTipTitle = "MTConnect PC Agent/Adapter";
        this.taskIcon.Text = "MTConnect PC Agent/Adapter";
        this.taskIcon.ShowBalloonTip(1000);
      }
    }

    #endregion

    private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    public System.Windows.Forms.NotifyIcon taskIcon;
    private System.Windows.Forms.LinkLabel linkCurrent;
    private System.Windows.Forms.LinkLabel linkProbe;
  }
}

