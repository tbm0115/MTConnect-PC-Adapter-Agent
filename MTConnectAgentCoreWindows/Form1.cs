

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

namespace MTConnectAgentCoreWindows
{
  public partial class Form1 : Form
  {
    Agent agent;
    PC myPC;
    System.Timers.Timer aTimer;
    const string _timeFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffzzz";

    public Form1()
    {
      InitializeComponent();
      agent = new Agent();
      this.button2.Enabled = false;
      aTimer = new System.Timers.Timer(50);
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

    }

    private void aTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      foreach (PCAdapter.Interfaces.IDataItem item in myPC.DataItems)
      {
        item.GetValue();
        if (item.Item.Changed)
        {
          switch (item.ValueType)
          {
            case PCAdapter.Interfaces.DataType.Condition:
              if (agent.StoreCondition(DateTime.Now.ToString(_timeFormat), item.Name, "NORMAL", Convert.ToString(item.GetValue()), null, null) > 0)
              {
                Console.WriteLine("StoreCondition was not successful for " + item.Name + ": " + item.Item.Value.ToString());
              }
              break;
            case PCAdapter.Interfaces.DataType.Event:
              if (agent.StoreEvent(DateTime.Now.ToString(_timeFormat), item.Name, Convert.ToString(item.GetValue()), null, null) > 0)
              {
                Console.WriteLine("StoreEvent was not successful for " + item.Name + ": " + item.Item.Value.ToString());
              }
              break;
            case PCAdapter.Interfaces.DataType.Sample:
              if (agent.StoreSample(DateTime.Now.ToString(_timeFormat), item.Name, Convert.ToString(item.GetValue())) > 0)
              {
                Console.WriteLine("StoreSample was not successful for " + item.Name + ": " + item.Item.Value.ToString());
              }
              break;
            default:
              Console.WriteLine("Cannot determine DataItem ValueType!\nName: " + item.Name + "\nDataType: " + item.ValueType.ToString());
              break;
          }
        }
        else
        {
          //Console.WriteLine("Value (" + item.Name + ") hasn't changed: " + item.Item.Value);
        }
      }
      myPC.Adapter.SendChanged();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      try
      {
        myPC.Start(false);
        agent.Start();
        aTimer.Start();

      }
      catch (AgentException exp)
      {
        String msg = exp.Message;
        if (exp.InnerException != null)
          msg = msg + "\n" + exp.InnerException.Message + exp.InnerException.ToString();
        MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.Dispose();
      }
      this.button1.Enabled = false;
      this.button2.Enabled = true;
      this.WindowState = FormWindowState.Minimized;

      //this.textBox1.Text = "Agent Started";
    }

    private void button2_Click(object sender, EventArgs e)
    {
      aTimer.Stop();
      myPC.Stop();
      agent.Stop();
      this.button1.Enabled = true;
      this.button2.Enabled = false;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      button1_Click(button1, null);
    }
  }
}
