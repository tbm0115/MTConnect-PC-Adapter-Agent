

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MTConnectAgentCore;
using MTConnect;
using PCAdapter;
using System.Timers;
using System.Configuration;

namespace MTConnectAgentCoreWindows
{
  public partial class Form1 : Form
  {
    Agent agent;
    PC myPC;
    List<IPCAgent.Interfaces.IMenuOption> MenuOptionPlugins;
    List<IPCAgent.Interfaces.ITick> TickPlugins;
    System.Timers.Timer aTimer;
    const string _timeFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffzzz";
    public States State { get; set; }
    public enum States{
      Started = 1,
      Stopped = -1,
      Paused = 0
    }

    public void MenuOptionPlugin_Clicked(object obj, EventArgs e){
      ToolStripItem tsi = (ToolStripItem)obj;
      ((IPCAgent.Interfaces.IMenuOption)tsi.Tag).Clicked();
    }

    public Form1()
    {
      var appSettings = ConfigurationManager.AppSettings;
      this.State = States.Stopped;


      InitializeComponent();

      // Manage Plugins
      //    Initialize
      this.MenuOptionPlugins = new List<IPCAgent.Interfaces.IMenuOption>();
      this.TickPlugins = new List<IPCAgent.Interfaces.ITick>();
      //    Load PluginManager from settings
      string exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
      exeDir = exeDir.Remove(exeDir.LastIndexOf(@"\"));
      var pluginManager = new IPCAgent.PluginManager(System.IO.Path.Combine(exeDir, "plugins.xml"), System.IO.Path.Combine(exeDir, "Plugins"));
      pluginManager.Save();
      //    Find proper plugins
      this.MenuOptionPlugins.AddRange(pluginManager.Get<IPCAgent.Interfaces.IMenuOption>());
      this.TickPlugins.AddRange(pluginManager.Get<IPCAgent.Interfaces.ITick>());
      //    Add Menu Option plugins
      foreach (IPCAgent.Interfaces.IMenuOption mnuPlugin in this.MenuOptionPlugins)
      {
        ToolStripItem tsi = mnuPlugins.Items.Add(mnuPlugin.Name);
        tsi.ToolTipText = mnuPlugin.Description;
        tsi.Tag = mnuPlugin;
        tsi.Click += MenuOptionPlugin_Clicked;
      }

      // Initialize Agent
      agent = new Agent();
      this.btnStop.Enabled = false;
      // Initialize Timer
      int sampleRate = 50;
      if (!Int32.TryParse(appSettings["Rate"], out sampleRate)){
        sampleRate = 50;
      }
      aTimer = new System.Timers.Timer(sampleRate);
      aTimer.Elapsed += new ElapsedEventHandler(aTimer_Elapsed);
      myPC = new PC();
      // Initialize PCAdapter
      myPC = new PC();
      myPC.DataItems.Add(new PCAdapter.Interfaces.Availability(myPC.Adapter, "avail"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.PositionX(myPC.Adapter, "posx"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.PositionY(myPC.Adapter, "posy"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.MouseLeftClicked(myPC.Adapter, "lclk"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.MouseRightClicked(myPC.Adapter, "rclk"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.CPUUsage(myPC.Adapter, "cpuu"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.MemoryUsage(myPC.Adapter, "memu"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ActiveWindowTitle(myPC.Adapter, "aapp"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ActiveWindowLocationX(myPC.Adapter, "locx"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ActiveWindowLocationY(myPC.Adapter, "locy"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ActiveWindowLocationWidth(myPC.Adapter, "sizx"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ActiveWindowLocationHeight(myPC.Adapter, "sizy"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.EnvironmentUsername(myPC.Adapter, "enun"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.EnvironmentUserDomain(myPC.Adapter, "enud"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.EnvironmentMachineName(myPC.Adapter, "enmn"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.EnvironmentOS(myPC.Adapter, "enos"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.AltKeyDown(myPC.Adapter, "kalt"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.CtrlKeyDown(myPC.Adapter, "kctl"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.CapsLockDown(myPC.Adapter, "kcap"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.NumLockDown(myPC.Adapter, "knum"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ScrollLockDown(myPC.Adapter, "kscl"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ShiftKeyDown(myPC.Adapter, "ksht"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ProcessCount(myPC.Adapter, "cntp"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ActiveWindowEXE(myPC.Adapter, "aexe"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ActiveWindowResponding(myPC.Adapter, "ares"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.EnvironmentMAC(myPC.Adapter, "maca"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ExecutionStatus(myPC.Adapter, "exec"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.BatteryStatus(myPC.Adapter, "bsts"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.ACStatus(myPC.Adapter, "bacs"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.BatteryLevel(myPC.Adapter, "blvl"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.LastWin32Error(myPC.Adapter, "werr"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.LastWin32Source(myPC.Adapter, "wexs"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.LastWin32Exception(myPC.Adapter, "wexe"));
      myPC.DataItems.Add(new PCAdapter.Interfaces.LastWin32Target(myPC.Adapter, "wext"));
    }

    private void aTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      IPCAgent.Interfaces.MTConnectData mtcSend = new IPCAgent.Interfaces.MTConnectData();

      // Iterate through each DataItem (set in Initialization)
      foreach (PCAdapter.Interfaces.IDataItem item in myPC.DataItems){

        // Refresh value
#if DEBUG
        string val = "TESTING"; // This is done to avoid errors with elevated priviledges.
#else
        string val = Convert.ToString(item.GetValue());
#endif

        if (this.State == States.Paused){
          val = "UNAVAILABLE";
        }

        // Populate mtcSend
        mtcSend.DataItems.Add(new IPCAgent.Interfaces.MTConnectData.MTConnectDataItem(item, val, item.Item.Changed));

        if (item.Item.Changed)
        {
          // Check DataItem type (Condition, Sample, Event)
          // Also check that the agent didn't have any issues 'storing' the value.
          switch (item.ValueType)
          {
            case PCAdapter.Interfaces.DataType.Condition:
              if (agent.StoreCondition(DateTime.Now.ToString(_timeFormat), item.Name, "NORMAL", val, null, null) > 0){
                Console.WriteLine("StoreCondition was not successful for " + item.Name + ": " + item.Item.Value.ToString());
              }
              break;
            case PCAdapter.Interfaces.DataType.Event:
              if (agent.StoreEvent(DateTime.Now.ToString(_timeFormat), item.Name, val, null, null) > 0){
                Console.WriteLine("StoreEvent was not successful for " + item.Name + ": " + item.Item.Value.ToString());
              }
              break;
            case PCAdapter.Interfaces.DataType.Sample:
              if (agent.StoreSample(DateTime.Now.ToString(_timeFormat), item.Name, val) > 0){
                Console.WriteLine("StoreSample was not successful for " + item.Name + ": " + item.Item.Value.ToString());
              }
              break;
            default:
              Console.WriteLine("Cannot determine DataItem ValueType!\nName: " + item.Name + "\nDataType: " + item.ValueType.ToString());
              break;
          }
        }else{
          //Console.WriteLine("Value (" + item.Name + ") hasn't changed: " + item.Item.Value);
        }
      }
      // Just to be safe, update Adapter with all current values
      myPC.Adapter.SendChanged();

      // Send data to all Tick plugins
      foreach (IPCAgent.Interfaces.ITick tickPlugin in this.TickPlugins){
        tickPlugin.Ticked(mtcSend);
      }
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
      try{
        if (this.State != States.Paused){
          // Start all tasks; Adapter (myPC), Agent (agent), and Timer Collection (aTimer)
          myPC.Start(false);
          agent.Start();
          aTimer.Start();
        }
        //this.State = States.Started;
        UpdateStatus(States.Started);
        Form_Hide();
      }
      catch (AgentException exp){
        String msg = exp.Message;
        if (exp.InnerException != null)
          msg = msg + "\n" + exp.InnerException.Message + exp.InnerException.ToString();
        MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.Dispose();
        //this.State = States.Stopped;
        UpdateStatus(States.Stopped);
      }
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
      // Stop all tasks; Adapter (myPC), Agent (agent), and Timer Collection (aTimer)
      aTimer.Stop();
      myPC.Stop();
      agent.Stop();
      UpdateStatus(States.Stopped);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      btnStart_Click(btnStart, null);
    }

    private void btnPause_Click(object sender, EventArgs e)
    {
      UpdateStatus(States.Paused);
    }
    
    private void UpdateStatus(States state){
      this.State = state;
      switch (this.State)
      {
        case States.Started:
          this.btnStart.Enabled = false;
          this.btnStop.Enabled = true;
          this.btnPause.Enabled = true;
          lblStatus.Text = "Started";
          lblStatus.BackColor = btnStart.BackColor;
          break;
        case States.Stopped:
          this.btnStart.Enabled = true;
          this.btnStop.Enabled = false;
          this.btnPause.Enabled = false;
          lblStatus.Text = "Stopped";
          lblStatus.BackColor = btnStop.BackColor;
          break;
        case States.Paused:
          this.btnPause.Enabled = false;
          this.btnStart.Enabled = true;
          this.btnStop.Enabled = false;
          lblStatus.Text = "Paused";
          lblStatus.BackColor = btnPause.BackColor;
          break;
        default:
          this.btnStart.Enabled = true;
          this.btnStop.Enabled = false;
          this.btnPause.Enabled = false;
          lblStatus.Text = "Press Start";
          break;
      }
    }

    private void Form_Hide(){
      this.ShowInTaskbar = false;
      ToolTipMessage("Click the MTConnect PC Agent/Adapter Icon to show again...");
      //this.WindowState = FormWindowState.Minimized;
      this.Hide();
    }
    private void ToolTipMessage(string msg, int length = 5000)
    {
      this.taskIcon.ShowBalloonTip(length, "MTConnect PC Agent/Adapter", msg, ToolTipIcon.Info);
    }
    private void Form_Show()
    {
      this.ShowInTaskbar = true;
      //this.taskIcon.Visible = false;
      this.Show();
      this.WindowState = System.Windows.Forms.FormWindowState.Normal;
      this.Focus();
    }
  }
}
