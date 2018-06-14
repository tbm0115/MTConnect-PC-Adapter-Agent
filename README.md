# MTConnect-PC-Adapter-Agent
A MTConnect Adapter and Agent for Windows PCs. MTConnect streams to port 5000 of the PC. More of a proof-of-concept, this Adapter provides information such as cursor position, active window, CPU usage, Memory usage, etc.


This is based on the following Repositories:

https://github.com/dyanglei/MTConnect-dotnetagent/tree/master/src

https://github.com/mtconnect/adapter

# Usage
The Released *MTCAgentWindows* should be ready to run on any PC without changes. It  requires administrative priveleges to open the port (5000).

The *MTConnectAgentWindowsService* I have found will not work for most User-based data items since Windows Services don't inherently have access to the User level.

# Data Items
Here's a list of currently supported Data Items (Id is enclosed in *()* ): 

 - *(avail)* **Availability** (Always AVAILABLE since the PC is on)
 - *(exec)* **Execution** (AUTOMATIC when user is actively using Keyboard/Mouse, READY after 5mins of inactivity)
 - *(cpuu)* **CPU Usage** (in PERCENT)
 - *(memu)* **Memory Usage** (in MEGABYTES of total currently used)
 - *(cntp)* **Total Process Count**
 - *(maca)* **MAC Address**
 - *(enun)* **Username**
 - *(enud)* **User Domain**
 - *(enmn)* **Machine name**
 - *(enos)* **Operating System Version**
 - *(lclk, rclk)* **Left/Right Mouse Button** down event
 - *(posx, posy)* **Cursor Position** (in PIXELS)
 - *(aapp)* **Active Window Title**
 - *(locx, locy, sizx, sizy)* **Location/Size of Active Window**
 - *(aexe)* **Active Window EXE**
 - *(ares)* **Active Window Response State**
 - *(kalt, kctl, kcap, knum, kscl, ksht)* **Control Key State** for Alt, Ctrl, CapsLock, NumLock, ScrollLock, and Shift

Most of these items are utilizing **user32.dll** methods, others are taken from the Environment namespace.

# Plugins
Plugin functionality has been added by use of the **IPCAgentPlugin** interface. These are the current plugin options:

 - **IMenuOption**: Creates a menu option in the Windows form to execute the plugin upon click.
 - **ITick**: Executes every time the internal "heartbeat" ticks. Essentially every time the application queries the system for the previously mentioned Data Items. This is particularly usefull if attempting to save any data into some form of database.

If you have a plugin available, simply drop it (along with any required fields) in the *Plugins* folder at the root of the application. Please note and remove any *IPCAagent.dll* and *PCAdapter.dll* files to avoid versioning issues.

For the first time running with plugins, start the application then close the application. This should ensure the creation of the *plugins.xml* file at the root. This structured document allows you to toggle which plugins you wish to actually use. Any changes made to this document requires a restart of the application in order to take effect.
