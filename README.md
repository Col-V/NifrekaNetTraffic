# NifrekaNetTraffic
Version 1.3\
Build 45\
2022-03-02

---
**System requirements:**\
Windows 10 or Windows 11\
NET 6.0 Runtime
\
\
**Development environment:**\
NifrekaNetTraffic is developed using Visual Studio 2022 and NET 6.0/C#/WPF.

---

NifrekaNetTraffic is a simple WPF desktop program for Windows 10 and Windows 11 and displays information about the current network interface traffic in compact, graphical and tablular representation.

NifrekaNetTraffic uses very little CPU resources, so it can run even on less powerful elder computers.\
To monitor the network performance NifrekaNetTraffic logs the data approximately every 1 second, currently limited to 86400 intervals (= 1 day).
\
\
NifrekaNetTraffic while watching a livestream\
![02](https://user-images.githubusercontent.com/32561354/155433764-57be9fd1-a900-4e99-b9fd-5ec319cc03a3.png)
\
\
NifrekaNetTraffic comes with two windows:

- Main window (with 3 different views)
    - Compact
    - Graph
    - Graph + Compact
- Window LogTable

\
**Main window : Compact**\
The compact view has no window title to consume only little space on screen.\
![03](https://user-images.githubusercontent.com/32561354/155433784-88013065-d1b6-428f-8f80-a8b656a23e77.png)

**Main window : Graph**\
This view shows a graphical representation of the log data.\
![04](https://user-images.githubusercontent.com/32561354/155433807-0f87999a-0d42-410f-bc9f-39741387cf21.png)


Use the slider at the bottom or the left/right arrow keys to select the display range. \
Use the sliders at the right side to scale the display. \
To toggle between bit or bytes display use button "Bit" ( or "B").
\
\
**Main window : Graph + Compact**\
Combined view\
![03-3](https://user-images.githubusercontent.com/32561354/156348703-da98fe90-e02c-4195-8a0c-84f1ed7e33a9.png)

**Window : LogTable**\
optional window with tabular representation of the log data\
![05](https://user-images.githubusercontent.com/32561354/155433835-3025c60f-f256-4835-9e00-419f82336fbc.png)

Hint:
Use SHIFT-Click to select multiple items and display the sums in the header section.
\
\
**Export as Text:**\
Log data is saved as TAB-formatted text file.

Format:
- DataTime_Str <\t>
- BytesReceived <\t>
- BytesSent <\t>
- BytesReceivedInterval <\t>
- BytesSentInterval <\r\n>

\
**Log Directory:**\
Opens the folder
User folder -> Documents -> NifrekaNetTraffic -> NifrekaNetTraffic_Data
\
\
**Check for Update:**\
![06a](https://user-images.githubusercontent.com/32561354/156364001-9adc74b0-e7cd-42cb-aeb1-21cdd6a58589.png)
\
When this option is marked, at startup the NifrekaNetTraffic server is automatically checked for update. You will get a notice, if a newer version is available.
\
\
**Sync Log Windows:**\
Use the context menu to sync both Log Windows\
![06b](https://user-images.githubusercontent.com/32561354/156364113-15e070aa-8a59-415e-b0e2-12141994782d.png)
\
\
When this option is marked, the display of the log windows is in sync when you click into either log window.

Hint:
To continuously sync the LogTable display, switch off "AutoScroll" and keep the mouse left button pressed while moving in the LogGraph window.
\
\
**Window position:**\
For convenience both NifrekaNetTraffic windows can be moved at once to the selected corner using the context menu.\
![07](https://user-images.githubusercontent.com/32561354/156364217-025f4a92-b26e-4835-abcb-10f6f654d077.png)
\
In the case of multiple monitors, the windows are moved to the selected corner of the monitor on which the main window is already located.

When exiting the window position/status is preserved, so that NifrekaNetTraffic appears in the same place on the monitor as before the next time the app is started. This information is saved in the file:

User folder -> Documents -> NifrekaNetTraffic -> NifrekaNetTraffic_Data -> NifrekaNetTraffic_Settings.nntDATA

If you delete the settings file, NifrekaNetTraffic will show up at the bottom right corner of the screen the next time it is started.
\
\
**Topmost:**\
If this option is selected, the NifrekaNetTraffic main window is always on top of other windows.
\
\
**Network Interface:**\
When switching the network connection, e.g. Ethernet > WLAN or back, this change is automatically detected by NifrekaNetTraffic.
\
\
**Digital signature:**\
The reason is quite simple. Code signing unfortunately costs money and also additional time/maintenance, which is disproportionate for such a simple program as "NifrekaNetTraffic".

This has irritating effects for some users:

1. Some virus scanners consider unsigned program code to be dangerous because the manufacturer of the software is unknown and cannot be verified.

2. Windows also gives a conspicuous warning if you want to run unsigned programs that you have recently downloaded from the Internet.

Of course, the use of programs from unknown sources is a risk that should not be underestimated, but in the past malware was found digitally signed as well.

A digital signature is therefore no guarantee of harmlessness!
\
\
**Download/Install/Uninstall:**\
NifrekaNetTraffic can be downloaded as a zip archive "NifrekaNetTraffic.zip" here:
https://github.com/Col-V/NifrekaNetTraffic/releases

All program components of NifrekaNetTraffic are virus-free to the best of our knowledge and belief.

To verify, you may want to have a look at the source code of NifrekaNetTraffic or compile it yourself. The source code is available here:
https://github.com/Col-V/NifrekaNetTraffic

Before NifrekaNetTraffic can be used, the contents of the zip archive "NifrekaNetTraffic.zip" must be unpacked.

The program "NifrekaNetTraffic.exe" and all other associated program components are located within the zip archive in the folder "NifrekaNetTraffic".

The folder "NifrekaNetTraffic" can be moved to any location after unpacking the archive, because "NifrekaNetTraffic.exe" can in principle be executed in any location.

To uninstall you only need to delete the folder "NifrekaNetTraffic" and the folder of the same name in the user documents folder:
"User folder -> Documents -> NifrekaNetTraffic"

---
**If you are unsure how to proceed after downloading a ZIP archive, here are some hints.**
\
\
**Step 1.**\
After finishing the download, open the Downloads folder.\
![08](https://user-images.githubusercontent.com/32561354/156365185-cffc30e6-77d7-4f3f-bbf6-4d12b8a842d1.png)
\
\
**Step 2.**\
Open the ZIP archive "NifrekaNetTraffic.zip".\
![09](https://user-images.githubusercontent.com/32561354/156365258-9e4523ad-59c1-4652-8b90-959452eba4b7.png)
\
\
**Step 3.**\
Copy the NifrekaNetTraffic folder to any location (e.g. Documents).\
![10](https://user-images.githubusercontent.com/32561354/156365292-c1f10971-fbc9-4338-bbc3-19c26998f4d4.png)
\
\
**Step 4.**\
In the "Documents folder", open the copied folder "NifrekaNetTraffic" and run the program "NifrekaNetTraffic.exe".\
![11](https://user-images.githubusercontent.com/32561354/156365322-c0c055ed-a6ec-47e5-b9eb-c902c604ee5e.png)
\
\
**Step 5.**\
Handle the warning (NifrekaNetTraffic is not digitally signed): Click On "More information".\
![12](https://user-images.githubusercontent.com/32561354/156365362-3193e519-2d32-440f-bc52-12cc873c4115.png)
\
\
**Step 6.**\
To continue click "Run anyway".\
![13](https://user-images.githubusercontent.com/32561354/156365382-80b54463-3013-4ae0-9096-a1ec4b6b9aff.png)
\
\
**Step 7.**\
To start "NifrekaNetTraffic.exe" quickly at any time, it is best to use the usual Windows techniques

- Create a "NifrekaNetTraffic.exe - Shortcut" on the desktop
- Pin to taskbar
- Pin to "Start"

**Step 8.**\
The zip archive "NifrekaNetTraffic.zip" is no longer needed after unpacking and can be deleted now.


---
**Autostart:**\
If you put a "NifrekaNetTraffic.exe - Shortcut" into the "Startup" folder, "NifrekaNetTraffic.exe" will statup automatically when Windows is started.

C:\Users\User11\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
(User11 = user name)
User folder > AppData > Roaming > Microsoft > Windows > Start Menu> Programs > Startup

NifrekaNetTraffic.exe - Shortcut within the folder "Startup"\
![14](https://user-images.githubusercontent.com/32561354/156365469-3e8bbd90-bd76-49b3-9327-937c61a3b8a9.png)



The AppData folder within the user folder is visible only if the option
- "Show hidden files, folders, and drives"

is selected in
- Control Panel -> Explorer Options -> View

![15](https://user-images.githubusercontent.com/32561354/156365566-8e8eab08-d401-4eb1-8676-d7e983404923.png)
---
# NET 6.0 Runtime Installation
NifrekaNetTraffic requires NET 6.0 Runtime.

If NET 6.0 Runtime is not yet installed on the computer, Windows will point this out and offers downloading the runtime from microsoft.com.

![16](https://user-images.githubusercontent.com/32561354/156365670-89eeb232-b083-49a5-a9cc-ab386bb1dc13.png)

![17](https://user-images.githubusercontent.com/32561354/156365736-2a9f93b5-6acc-4c96-b671-183571af58b1.png)

Open the downloaded file and perform the installation.

---
nifreka.nl



