/*
* Copyright (c) 2008, AMT – The Association For Manufacturing Technology (“AMT”)
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the AMT nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* DISCLAIMER OF WARRANTY. ALL MTCONNECT MATERIALS AND SPECIFICATIONS PROVIDED
* BY AMT, MTCONNECT OR ANY PARTICIPANT TO YOU OR ANY PARTY ARE PROVIDED "AS IS"
* AND WITHOUT ANY WARRANTY OF ANY KIND. AMT, MTCONNECT, AND EACH OF THEIR
* RESPECTIVE MEMBERS, OFFICERS, DIRECTORS, AFFILIATES, SPONSORS, AND AGENTS
* (COLLECTIVELY, THE "AMT PARTIES") AND PARTICIPANTS MAKE NO REPRESENTATION OR
* WARRANTY OF ANY KIND WHATSOEVER RELATING TO THESE MATERIALS, INCLUDING, WITHOUT
* LIMITATION, ANY EXPRESS OR IMPLIED WARRANTY OF NONINFRINGEMENT,
* MERCHANTABILITY, OR FITNESS FOR A PARTICULAR PURPOSE. 

* LIMITATION OF LIABILITY. IN NO EVENT SHALL AMT, MTCONNECT, ANY OTHER AMT
* PARTY, OR ANY PARTICIPANT BE LIABLE FOR THE COST OF PROCURING SUBSTITUTE GOODS
* OR SERVICES, LOST PROFITS, LOSS OF USE, LOSS OF DATA OR ANY INCIDENTAL,
* CONSEQUENTIAL, INDIRECT, SPECIAL OR PUNITIVE DAMAGES OR OTHER DIRECT DAMAGES,
* WHETHER UNDER CONTRACT, TORT, WARRANTY OR OTHERWISE, ARISING IN ANY WAY OUT OF
* THIS AGREEMENT, USE OR INABILITY TO USE MTCONNECT MATERIALS, WHETHER OR NOT
* SUCH PARTY HAD ADVANCE NOTICE OF THE POSSIBILITY OF SUCH DAMAGES.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using MTConnectAgentCore;
using System.Timers;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using MTConnect;
using PCAdapter;

namespace MTConnectAgentWindowsService
{
  public partial class MTConnect : ServiceBase
  {
    const string _timeFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffzzz";
    private Agent agent;
    static private String SERVICENAME = "MTConnect";
    private static PC myPC;
    private Timer aTimer;

    public MTConnect()
    {
      InitializeComponent();
      if (!System.Diagnostics.EventLog.SourceExists(SERVICENAME))
      {
        System.Diagnostics.EventLog.CreateEventSource(
           SERVICENAME, "Application");
      }
      eventLog1.Source = SERVICENAME;
      eventLog1.Log = "Application";
      LogToFile.Initialize(true);
      // Initialize Timer
      aTimer = new Timer(50);
      aTimer.Elapsed += new ElapsedEventHandler(aTimer_Elapsed);


      // Note where logs are saved
      eventLog1.WriteEntry("Detailed logs are saved here: " + LogToFile.currentLogFileName);

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
      //eventLog1.WriteEntry("Refreshing " + myPC.DataItems.Count.ToString() + " data items.");
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
                eventLog1.WriteEntry("StoreCondition was not successful for " + item.Name + ": " + item.Item.Value.ToString(), EventLogEntryType.Warning);
              }
              break;
            case PCAdapter.Interfaces.DataType.Event:
              if (agent.StoreEvent(DateTime.Now.ToString(_timeFormat), item.Name, Convert.ToString(item.GetValue()), null, null) > 0)
              {
                eventLog1.WriteEntry("StoreEvent was not successful for " + item.Name + ": " + item.Item.Value.ToString(), EventLogEntryType.Warning);
              }
              break;
            case PCAdapter.Interfaces.DataType.Sample:
              if (agent.StoreSample(DateTime.Now.ToString(_timeFormat), item.Name, Convert.ToString(item.GetValue())) > 0)
              {
                eventLog1.WriteEntry("StoreSample was not successful for " + item.Name + ": " + item.Item.Value.ToString(), EventLogEntryType.Warning);
              }
              break;
            default:
              eventLog1.WriteEntry("Cannot determine DataItem ValueType!\nName: " + item.Name + "\nDataType: " + item.ValueType.ToString(), EventLogEntryType.Warning);
              break;
          }
        }else
        {
          eventLog1.WriteEntry("Value (" + item.Name + ") hasn't changed: " + item.Item.Value);
        }
      }
      myPC.Adapter.SendChanged();
    }

    protected override void OnStart(string[] args)
    {
      agent = new Agent();
      try
      {
        agent.Start();
        myPC.Start(false);
        aTimer.Start();
      }
      catch (AgentException exp)
      {
        String msg = exp.Message;
        if (exp.InnerException != null)
          msg = msg + "\n" + exp.InnerException.Message;
        eventLog1.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Error);
      }
      catch (System.UnauthorizedAccessException eu)
      {
        eventLog1.WriteEntry("Access denied.  Please specify the user account that the MTConnect service can use to log on.", System.Diagnostics.EventLogEntryType.Error);
        throw eu;
      }
    }

    //private void OnTimer_Elapsed(object sender, EventArgs e)
    //{
    //  eventLog1.WriteEntry("Refreshing data " + DateTime.Now.ToString(),EventLogEntryType.Information);

    //  foreach (PCAdapter.Interfaces.IDataItem item in myPC.DataItems)
    //  {
    //    if (item.Item.Changed)
    //    {
    //      switch (item.ValueType)
    //      {
    //        case PCAdapter.Interfaces.DataType.Condition:
    //          agent.StoreCondition(DateTime.Now.ToString(_timeFormat), item.Name, "NORMAL", Convert.ToString(item.Item.Value), "", "");
    //          break;
    //        case PCAdapter.Interfaces.DataType.Event:
    //          agent.StoreEvent(DateTime.Now.ToString(_timeFormat), item.Name, Convert.ToString(item.Item.Value), "", "");
    //          break;
    //        case PCAdapter.Interfaces.DataType.Sample:
    //          agent.StoreSample(DateTime.Now.ToString(_timeFormat), item.Name, Convert.ToString(item.Item.Value));
    //          break;
    //        default:
    //          eventLog1.WriteEntry("Cannot determine DataItem ValueType!\nName: " + item.Name + "\nDataType: " + item.ValueType.ToString(), EventLogEntryType.Warning);
    //          break;
    //      }
    //    }
    //  } 
    //}

    protected override void OnStop()
    {
      //myPC.Elapsed -= (EventHandler)OnTimer_Elapsed;
      aTimer.Stop();
      myPC.Stop();
      agent.Stop();
    }
  }
  public class MtConnectCondition
  {
    private string _timestamp, _dataItemId, _value, _code, _nativeCode;

    public string TimeStamp { get; }
    public string DataItemId { get; }
    public string Condition { get; }
    public string Value { get; }
    public string nativeCode { get; }
    public string code { get; }

    public MtConnectCondition()
    {

    }

    public bool SetValue(Agent agent, string val)
    {
      if (!this.Value.Equals(val))
      {
        this._timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        this._value = val;
        this._code = val;
        this._nativeCode = val;

        return true;
      }
      return false;
    }
  }
  public class MtConnectEvent
  {
    private string _timestamp, _dataItemId, _value, _code, _nativeCode;
    public string TimeStamp { get; }
    public string DataItemId { get; }
    public string Value { get; }
    public string Code { get; }
    public string NativeCode { get; }

    public MtConnectEvent(string dataItemId)
    {
      this._dataItemId = dataItemId;
      this._timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
      this._value = "0";
      this._code = "";
      this._nativeCode = "";
    }

    public bool SetValue(Agent agent, string val)
    {
      if (!this.Value.Equals(val))
      {
        this._timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        this._value = val;
        this._code = val;
        this._nativeCode = val;

        return true;
      }
      return false;
    }
  }
}
public class MtConnectSample
{
  private string _timestamp, _dataItemId, _value;

  public string TimeStamp
  {
    get
    {
      return this._timestamp;
    }
  }
  public string DataItemId
  {
    get
    {
      return this._dataItemId;
    }
  }
  public string Value
  {
    get
    {
      return this._value;
    }
  }

  public MtConnectSample(string dataItemId)
  {
    this._dataItemId = dataItemId;
    this._timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    this._value = "0";
  }

  public bool SetValue(Agent agent, string val)
  {
    if (!this.Value.Equals(val))
    {
      this._timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
      this._value = val;

      return true;
    }
    return false;
  }
}
