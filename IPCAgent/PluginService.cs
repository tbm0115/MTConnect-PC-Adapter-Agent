using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Xml;

namespace IPCAgent
{
  public interface IPCAgentPlugin
  {
    /// <summary>
    /// Defines the name of the Revolution Plugin
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Provides a static description of the purpose for the Plugin
    /// </summary>
    string Description { get; }
    /// <summary>
    /// Provides a reference to the name of the developer for the Plugin
    /// </summary>
    string Author { get; }
    /// <summary>
    /// Provides a reference to the version for the Plugin
    /// </summary>
    string Version { get; }
  }
  public sealed class PluginService
  {
    public struct AvailablePlugin{
      public string AssemblyPath { get; set; }
      public string ClassName { get; set; }
    }

    public static T[] FindPlugins<T>(string directory, Type findType = null){
      if (findType == null){
        findType = typeof(T);
      }
      List<AvailablePlugin> availablePlugins = new List<AvailablePlugin>();
      AvailablePlugin[] ap = FindAvailablePlugins(directory, findType);
      if (ap != null){
        availablePlugins.AddRange(ap);
      }
      List<T> plugins = new List<T>();
      foreach (AvailablePlugin availablePlugin in availablePlugins)
      {
        T plugin = CreateInstance<T>(availablePlugin);
        try
        {
          plugins.Add(plugin);
        }catch(InvalidCastException ex){
          plugins.Add(default(T));
        }
      }
      return plugins.ToArray();
    }

    public static AvailablePlugin[] FindAvailablePlugins(string directory, Type interfaceType){
      var Plugins = new ArrayList();
      string[] strDLLs;
      Assembly objDLL = null;

      // Go through all DLLsin the directory, attempting to load them
      strDLLs = Directory.GetFileSystemEntries(directory, "*.dll");
      for (int i = 0; i < strDLLs.Length; i++){
        try{
          if (!FindAssembly(strDLLs[i], ref objDLL)){
            objDLL = Assembly.LoadFrom(strDLLs[i]);
          }
          ExamineAssembly(objDLL, interfaceType.Name, Plugins);
        }catch (Exception ex){
          // Error loading DLL
        }
      }
      // Return all plugins found
      AvailablePlugin[] Results = new AvailablePlugin[Plugins.Count];
      if (Plugins.Count != 0){
        Plugins.CopyTo(Results);
        return Results;
      }else{
        return null;
      }
    }

    public static AvailablePlugin? FindClass(string filePath, string fullname, Type type){
      ArrayList Plugins = new ArrayList();
      Assembly objDLL = null;
      // Go through all DLLs in the directory, attempting to load them
      try{
        if (!FindAssembly(fullname, ref objDLL)){
          if (System.IO.File.Exists(filePath)){
            objDLL = Assembly.LoadFrom(filePath);
          }
        }
        ExamineAssembly(objDLL, type.Name, Plugins);
      }catch (Exception ex){
        System.Diagnostics.Debug.WriteLine("[FindPlugin]: " + ex.Message);
      }

      foreach (AvailablePlugin ap in Plugins){
        if (ap.ClassName == fullname){
          return ap;
        }
      }
      return null;
    }

    private static void ExamineAssembly(Assembly objDLL, string interfacename, ArrayList Plugins){
      Type objInterface;
      AvailablePlugin Plugin;

      // Loop through each type in the DLL

      Type[] types = objDLL.GetTypes();
      for (int i = 0; i < types.Length; i++)
      {
        // Only look at public types
        if (types[i].IsPublic){
          // Ignore abstract classes
          if (types[i].Attributes != TypeAttributes.Abstract){
            // See if this type implements our interface
            objInterface = types[i].GetInterface(interfacename, true);
            if (objInterface != null){
              Plugin = new AvailablePlugin();
              Plugin.AssemblyPath = objDLL.Location;
              Plugin.ClassName = types[i].FullName;
              Plugins.Add(Plugin);
            }
          }
        }
      }
    }
    public static object CreateInstance(AvailablePlugin Plugin){
      Assembly objDll = null;
      object objPlugin;

      try{
        // Load dll
        var fi = new System.IO.FileInfo(Plugin.AssemblyPath);
        string fn = fi.Name;
        if (!FindAssembly(Plugin.AssemblyPath, ref objDll)){
          objDll = Assembly.LoadFrom(Plugin.AssemblyPath);
        }

        // Create and return class instance
        objPlugin = objDll.CreateInstance(Plugin.ClassName);
      }catch (Exception ex){
        return null;
      }
      return objPlugin;
    }
    
    public static T CreateInstance<T>(AvailablePlugin Plugin){
      Assembly objDll = null;
      object objPlugin;

      try{
        // Load dll
        var fi = new System.IO.FileInfo(Plugin.AssemblyPath);
        string fn = fi.Name;
        if (!FindAssembly(Plugin.AssemblyPath, ref objDll))
        {
          objDll = Assembly.LoadFrom(Plugin.AssemblyPath);
        }

        // Create and return class instance
        objPlugin = objDll.CreateInstance(Plugin.ClassName);
      }catch (Exception ex){
        return default(T);
      }
      return (T)objPlugin;
    }
    public static Assembly FindAssembly(string AssName){
      Assembly[] asss = AppDomain.CurrentDomain.GetAssemblies();
      for (int i = 0; i < asss.Length; i++)
      {
        if (asss[i].FullName == AssName){
          return asss[i];
        }
      }
      return null;
    }
    public static Boolean FindAssembly(string AssName, ref Assembly AssRef){
      AssRef = FindAssembly(AssName);
      if (AssRef != null){
        return true;
      }
      return false;
    }

    public static string[] GetInterfaceNames()
    {
      return AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsInterface && t.Namespace.Contains("IPCAgent.Interfaces")).Select(t => t.FullName).ToArray();
    }
    public static Type[] GetTypes(){
      return Assembly.GetExecutingAssembly().GetTypes();// AppDomain.CurrentDomain.GetAssemblies();
    }
  }

  public sealed class PluginManager{
    private string _SettingsPath { get; set; }
    private string _PluginDir { get; set; }
    public Dictionary<IPCAgentPlugin, bool> PluginAvailability { get; set; }
    public List<Type> ImplementedInterfaces { get; set; }
    private XmlDocument _xDoc { get; set; }

    /// <summary>
    /// Initialize a new instance of a PluginManager.
    /// </summary>
    /// <param name="relSettingsPath">Reference to the path of the 'pluginsafe.xml'. If the file does not exist, it is created.</param>
    /// <param name="relPluginsDirectory">Reference to the plugin repository. This directory must exist prior to runtime!</param>
    public PluginManager(string relSettingsPath, string relPluginsDirectory)
    {
      this._SettingsPath = relSettingsPath;
      this._PluginDir = relPluginsDirectory;
      this.PluginAvailability = new Dictionary<IPCAgentPlugin, bool>();

      if (!System.IO.Directory.Exists(this._PluginDir)){
        throw new IOException(this._PluginDir + " must exist prior to runtime.");
      }

      this.ImplementedInterfaces = new List<Type>();
      foreach (Type t in Assembly.GetExecutingAssembly().GetTypes()){
        if (t.IsInterface && t.Namespace.Contains("IPCAgent.Interfaces")){
          this.ImplementedInterfaces.Add(t);
        }
      }

      this._xDoc = new XmlDocument();
      XmlElement xRoot = null;
      if (!System.IO.File.Exists(_SettingsPath)){
        this._xDoc.AppendChild(this._xDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
        xRoot = (XmlElement)this._xDoc.AppendChild(this._xDoc.CreateElement("Interfaces"));
        this._xDoc.Save(this._SettingsPath);
      }else{
        this._xDoc.Load(this._SettingsPath);
        xRoot = (XmlElement)_xDoc.SelectSingleNode("//Interfaces");
      }
      
      foreach (Type t in this.ImplementedInterfaces)
      {
        var searchInterfaces = xRoot.SelectNodes("Interface[@name='" + t.FullName + "']");
        XmlElement xInterface = null;
        if (searchInterfaces.Count == 1){
          xInterface = (XmlElement)searchInterfaces[0];
        }else if (searchInterfaces.Count > 1){
          throw new Exception("Cannot have more than one instance of a single Interface in the same system!");
        }else{
          xInterface = (XmlElement)xRoot.AppendChild(this._xDoc.CreateElement("Interface"));
          xInterface.Attributes.Append(this._xDoc.CreateAttribute("name")).Value = t.FullName;
        }
        IPCAgentPlugin[] plugins = PluginService.FindPlugins<IPCAgentPlugin>(this._PluginDir, t);
        foreach (IPCAgentPlugin plugin in plugins){
          Type pluginType = plugin.GetType();
          var searchNode = xInterface.SelectNodes("Plugin[@name=\"" + pluginType.FullName + "\"]");
          XmlElement xPlugin = null;
          if (searchNode.Count == 1){
            xPlugin = (XmlElement)searchNode[0];
          }else if (searchNode.Count > 1){
            throw new Exception("Cannot have more than one instance of a single Plugin in the same system!");
          }else{
            xPlugin = (XmlElement)xInterface.AppendChild(this._xDoc.CreateElement("Plugin"));
            xPlugin.Attributes.Append(this._xDoc.CreateAttribute("name")).Value = pluginType.FullName;
            xPlugin.InnerText = false.ToString();
          }
          if (xPlugin != null){
            bool isActive = Convert.ToBoolean(xPlugin.InnerText);
            this.PluginAvailability.Add(plugin, isActive);
          }
        }
      }
      

      this._xDoc.Save(this._SettingsPath); // Save any new plugins right away
    }

    public bool Toggle(IPCAgentPlugin plugin){
      bool newValue = !this.PluginAvailability[plugin];
      this.PluginAvailability[plugin] = newValue;
      return newValue;
    }
    public bool Toggle(string pluginAssemblyName){
      IPCAgentPlugin plugin = this.PluginAvailability.Keys.FirstOrDefault(o => o.GetType().FullName == pluginAssemblyName);
      return this.Toggle(plugin);
    }
    
    public T[] Get<T>(){
      Type type = typeof(T);
      return this.PluginAvailability.Where(o => o.Value && IsPartOfInterface(o.Key, type)).Select(o => (T)o.Key).ToArray();
    }

    public bool IsPartOfInterface(IPCAgentPlugin plugin, Type interfaceType){
      XmlNode xPlugin = this._xDoc.SelectSingleNode("//Interface[@name='" + interfaceType.FullName + "']/Plugin[@name='" + plugin.GetType().FullName + "']");
      return xPlugin != null;
    }

    public bool IsActive(IPCAgentPlugin plugin){
      bool blnIsActive = false;
      XmlNode xPlugin = this._xDoc.SelectSingleNode("//Plugin[@name='" + plugin.GetType().FullName + "']");
      if (xPlugin != null)
      {
        blnIsActive = Convert.ToBoolean(xPlugin.InnerText);
      }
      return blnIsActive;
    }

    //public InterfaceLabel GetInterfaceLabel(IPCAgent plugin){
    //  string strInterface = "";
    //  XmlNode xPlugin = this._xDoc.SelectSingleNode("//Plugin[@name='" + plugin.GetType().FullName + "']");
    //  if (xPlugin != null){
    //    strInterface = xPlugin.ParentNode.Attributes["name"].Value;
    //    Type interfaceType = Type.GetType(strInterface + ", RevolutionInterface");
    //    return interfaceType.GetCustomAttribute<InterfaceLabel>();
    //  }
    //  return null;
    //}

    public void Save(){
      foreach (KeyValuePair<IPCAgentPlugin, Boolean> kv in this.PluginAvailability)
      {
        Type type = kv.Key.GetType();
        var searchNode = this._xDoc.SelectNodes("//Plugin[@name=\"" + type.FullName + "\"]");
        if (searchNode.Count == 1)
        {
          XmlElement xPlugin = (XmlElement)searchNode[0];
          xPlugin.InnerText = kv.Value.ToString();
        }
      }
      this._xDoc.Save(this._SettingsPath);
    }

  }
}
