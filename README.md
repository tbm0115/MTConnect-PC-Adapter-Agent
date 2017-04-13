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