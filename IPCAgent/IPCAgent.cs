using System.Collections.Generic;
using System.Linq;

namespace IPCAgent.Interfaces
{
  
  public interface IMenuOption: IPCAgentPlugin{
    /// <summary>
    /// Raised when the menu option is chosen on the main form.
    /// </summary>
    void Clicked();
  }
  /// <summary>
  /// Wrapper for all MTConnect data
  /// </summary>
  public class MTConnectData{
    /// <summary>
    /// List of structured MTConnect Data Items
    /// </summary>
    public List<MTConnectDataItem> DataItems { get; set; }

    public MTConnectData(){
      this.DataItems = new List<MTConnectDataItem>();
    }

    /// <summary>
    /// Returns a list of all changed MTConnect Data Items
    /// </summary>
    /// <returns></returns>
    public MTConnectDataItem[] GetChangedItems()
    {
      return this.DataItems.Where(o => o.Changed).ToArray();
    }
    /// <summary>
    /// Returns the first MTConnect Data Item by the specified name. If requested, only if the value has changed. If no value is found, null is returned.
    /// </summary>
    /// <param name="name">Query of the full name.</param>
    /// <param name="onlyIfChanged">Flags whether to search only items that have changed.</param>
    /// <returns></returns>
    public MTConnectDataItem GetItem(string name, bool onlyIfChanged = false){
      return this.DataItems.FirstOrDefault(o => (onlyIfChanged ? o.Changed : true) && o.Name.ToLower() == name.ToLower());
    }

    /// <summary>
    /// Wrapper for an individual MTConnect Data Item
    /// </summary>
    public class MTConnectDataItem{
      /// <summary>
      /// Specifies the enumerable type of a Data Item (Condition, Event, or Sample)
      /// </summary>
      public Types Type { get; set; }
      /// <summary>
      /// Specifies the 'name' attribute from the MTConnect stream for this Data Item
      /// </summary>
      public string Name { get; set; }
      /// <summary>
      /// Specifies the current value
      /// </summary>
      public string Value { get; set; }
      /// <summary>
      /// Flags whether the value has changed
      /// </summary>
      public bool Changed { get; set; }

      public enum Types{
        Condition = PCAdapter.Interfaces.DataType.Condition,
        Event = PCAdapter.Interfaces.DataType.Event,
        Sample = PCAdapter.Interfaces.DataType.Sample
      }

      public MTConnectDataItem(PCAdapter.Interfaces.IDataItem dataItem, string value, bool changed){
        this.Type = (Types)dataItem.ValueType;
        this.Name = dataItem.Name;
        this.Value = value;
        this.Changed = changed;
      }
    }
  }
  public interface ITick : IPCAgentPlugin
  {
    /// <summary>
    /// Raised when the MTConnect stream "beats". Essentially whenever the internal timer "ticks".
    /// </summary>
    /// <param name="data">A snapshot of the current MTConnect dataset is passed.</param>
    void Ticked(MTConnectData data);
  }
}
