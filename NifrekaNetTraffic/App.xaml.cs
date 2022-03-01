using Nifreka;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NifrekaNetTraffic
{
    // #######################################################################
    public partial class App : Application
    {
        public LogList logList;
        public NifrekaNetTrafficSettings nifrekaNetTrafficSettings;

        private DispatcherTimer dispatcherTimer;
        public NetAdapterList netAdapterList;
        public NetAdapter selectedNetAdapter;

        public long previousTicks;
        public long bytesReceived_Previously;
        public long bytesSent_Previously;

        public WindowLogGraph windowLogGraph;
        public WindowLogTable windowLogTable;

        public Corner lastCorner = Corner.BottomRight;
        

        // ========================
        // ctor
        // ========================
        App()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture;

                // om te testen
                // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en"); 
            }
            catch (Exception)
            {

                // throw;
            }
        }

        // ========================================================
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CleanupOldDatafiles();

            logList = new LogList();
            logList.ReadFromFile();

            netAdapterList = new NetAdapterList();

            Create_AdapterList();

            NetworkChange.NetworkAddressChanged += new
            NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged_CallBack);

            NetworkChange.NetworkAvailabilityChanged +=
            new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged_CallBack);

            

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dispatcherTimer.IsEnabled = false;
            dispatcherTimer.Tick += DispatcherTimer_Tick;

            ReadSettings();
            OpenWindows();

            if (nifrekaNetTrafficSettings.CheckForUpdateAuto == true)
            {
                CheckForUpdateAuto();
            }
            
        }

        // ========================================================
        private void CleanupOldDatafiles()
        {
            Delete_File(Const.NifrekaNetTraffic_Settings_PATH_Old_v1);

            Delete_File(Const.NifrekaNetTraffic_Settings_PATH_Old_pre42);
            Delete_File(Const.NifrekaNetTraffic_Log_PATH_Old_pre42);
        }

        private void Delete_File(string filepath)
        {
            try
            {
                if (File.Exists(filepath) == true)
                {
                    File.Delete(filepath);
                }
            }
            catch (Exception)
            {
                // throw;
            }
        }

        // ========================================================
        public void DispatcherTimer_Start()
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Start();
            }
        }
        public void DispatcherTimer_Stop()
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
            }
        }

        // ========================================================
        // NetAdapter 
        // ========================================================
        private void NetworkChange_NetworkAddressChanged_CallBack(object sender, EventArgs e)
        {
            CreateNetAdapterList();
        }

        // ========================================================
        private void NetworkChange_NetworkAvailabilityChanged_CallBack(object sender, EventArgs e)
        {
            CreateNetAdapterList();
        }

        // ========================================================
        delegate void CreateNetAdapterList_Delegate();
        private void CreateNetAdapterList()
        // ====================================
        {
            if (this.Dispatcher.CheckAccess())
            {
                Delegated_CreateNetAdapterList();
            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new CreateNetAdapterList_Delegate(CreateNetAdapterList));
            }
        }

        // ========================================================
        private void Delegated_CreateNetAdapterList()
        {
            Create_AdapterList();

            if (netAdapterList.Count > 0)
            {
                selectedNetAdapter = netAdapterList[0];
            }

            if (windowLogTable != null)
            {
                windowLogTable.Update_comboBox_NetAdapter();
            }
        }


        // ========================================================
        private void Create_AdapterList()
        {
            if (netAdapterList == null)
            {
                netAdapterList = new NetAdapterList();
            }
            else
            {
                netAdapterList.Clear();
            }

            try
            {
                NetworkInterface[] networkInterfaceArr = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in networkInterfaceArr)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        PhysicalAddress physicalAddress = ni.GetPhysicalAddress();
                        if (physicalAddress != null)
                        {
                            string ni_PhysicalAddress_String = string.Join("-", (from z in physicalAddress.GetAddressBytes() select z.ToString("X2")).ToArray());
                            if (ni_PhysicalAddress_String.Length > 0)
                            {
                                netAdapterList.Add(new NetAdapter(ni));
                            }

                        }

                    }

                }
                if (netAdapterList.Count > 0)
                {
                    this.selectedNetAdapter = netAdapterList[0];

                    // SetPreviousIPStatistics
                    //
                    this.bytesReceived_Previously = this.selectedNetAdapter.BytesReceived;
                    this.bytesSent_Previously = this.selectedNetAdapter.BytesSent;
                    DateTime dt = DateTime.Now;
                    this.previousTicks = dt.Ticks;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        bool firstRun = true;
        // ========================================================
        private void ReadSettings()
        {
            string filepath = Const.NifrekaNetTraffic_Settings_PATH;

            if (nifrekaNetTrafficSettings == null)
            {
                nifrekaNetTrafficSettings = new NifrekaNetTrafficSettings();
            }

            if (File.Exists(filepath) == true)
            {
                nifrekaNetTrafficSettings.ReadSettingsData();               
                firstRun = false;
            }
            else
            {
                firstRun = true;
            }

            lastCorner = nifrekaNetTrafficSettings.LastCorner;
        }

        // ========================================================
        private void FirstRunInit()
        {
            System.Windows.Forms.Screen screen = NifrekaScreenUtil.GetScreen_By_Mouse();

            if (screen != null)
            {
                nifrekaNetTrafficSettings.Width_WindowLogGraph = nifrekaNetTrafficSettings.WindowLogGraph_DefaultWidth;
                nifrekaNetTrafficSettings.Height_WindowLogGraph = nifrekaNetTrafficSettings.WindowLogGraph_DefaultHeight;

                nifrekaNetTrafficSettings.Top_WindowLogGraph = screen.WorkingArea.Top 
                                                                + screen.WorkingArea.Height 
                                                                - nifrekaNetTrafficSettings.WindowLogGraph_DefaultHeight;

                nifrekaNetTrafficSettings.Left_WindowLogGraph = screen.WorkingArea.Left 
                                                                + screen.WorkingArea.Width 
                                                                - nifrekaNetTrafficSettings.WindowLogGraph_DefaultWidth;

                // ===

                nifrekaNetTrafficSettings.Width_WindowLogTable = nifrekaNetTrafficSettings.WindowLogGraph_DefaultWidth;
                nifrekaNetTrafficSettings.Height_WindowLogTable = nifrekaNetTrafficSettings.WindowLogTable_DefaultHeight;

                nifrekaNetTrafficSettings.Top_WindowLogTable = nifrekaNetTrafficSettings.Top_WindowLogGraph
                                                                - nifrekaNetTrafficSettings.Height_WindowLogGraph;

                nifrekaNetTrafficSettings.Left_WindowLogTable = nifrekaNetTrafficSettings.Left_WindowLogGraph;

            }

        }


        // ========================================================
        private void OpenWindows()
        {
            if (firstRun)
            {
                FirstRunInit();

                Do_Open_WindowLogGraph();
                Do_Open_WindowLogTable();
            }

            else
            {
                Do_Open_WindowLogGraph();

                if (nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable == true)
                {
                    Do_Open_WindowLogTable();
                }

            }

            windowLogGraph.Activate();

        }

        // ========================================================
        public void Do_Open_WindowLogTable()
        {
            if (this.windowLogTable == null)
            {
                this.windowLogTable = new WindowLogTable();
                this.windowLogTable.Show();
            }
            else
            {
                if (windowLogTable.WindowState != WindowState.Normal)
                {
                    windowLogTable.WindowState = WindowState.Normal;
                }

                this.windowLogTable.Activate();
            }
        }

        // ========================================================
        public void Do_Open_WindowLogGraph()
        {
            if (this.windowLogGraph == null)
            {
                this.windowLogGraph = new WindowLogGraph();
                this.windowLogGraph.Show();
            }
            else
            {
                if (windowLogGraph.WindowState == WindowState.Minimized)
                {
                    windowLogGraph.WindowState = WindowState.Normal;
                }
                this.windowLogGraph.Activate();
            }

        }

        // ========================================================
        public void UnregisterWindow(Window window)
        {
            if (windowLogTable != null)
            {
                if (window.Equals(windowLogTable))
                {
                    nifrekaNetTrafficSettings.Left_WindowLogTable = windowLogTable.Left;
                    nifrekaNetTrafficSettings.Top_WindowLogTable = windowLogTable.Top;
                    nifrekaNetTrafficSettings.Width_WindowLogTable = windowLogTable.ActualWidth;
                    nifrekaNetTrafficSettings.Height_WindowLogTable = windowLogTable.ActualHeight;
                    windowLogTable = null;
                }
            }

        }

        // ========================================================
        private void PreserveVisibility()
        {
            if (windowLogTable != null)
            {
                nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable = true;
            }

            if (windowLogGraph != null)
            {
                nifrekaNetTrafficSettings.VisibleAtStart_WindowLogGraph = true;
            }
            
        }

        // ========================================================
        public void Do_Exit()
        {
            if (windowLogGraph != null)
            {
                windowLogGraph.Close();
            }
        }

        // ========================================================
        public void CloseAllWindows()
        {
            PreserveVisibility();

            if (windowLogTable != null)
            {
                windowLogTable.Close();
            }
            if (windowLogGraph != null)
            {
                // windowLogGraph.Close();
            }
 
        }

        // ========================================================
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        // ========================================================
        {
            DoGuiUpdate();
        }

        // ========================================================
        private void DoGuiUpdate()
        // ========================================================
        {
            if (selectedNetAdapter != null)
            {
                if (selectedNetAdapter.networkInterface != null)
                {
                    DateTime dtNow = DateTime.Now;
                    long nowTicks = dtNow.Ticks;

                    long bytesReceived = selectedNetAdapter.BytesReceived;
                    long bytesSent = selectedNetAdapter.BytesSent;

                    long bytesReceived_Interval = bytesReceived - bytesReceived_Previously;
                    long bytesSent_Interval = bytesSent - bytesSent_Previously;

                    if (bytesReceived_Interval < 0
                        ||
                        bytesSent_Interval < 0
                        )
                    {
                    }
                    else
                    {
                        LogListItem logListItem = new LogListItem(nowTicks,
                                                                bytesReceived,
                                                                bytesSent,
                                                                bytesReceived_Interval,
                                                                bytesSent_Interval);
                        logList.AddItem(logListItem);

                        logList.BytesReceivedTotal = bytesReceived;
                        logList.BytesSentTotal = bytesSent;

                        // =====
                        // Update windowLogGraph
                        if (windowLogGraph != null)
                        {
                            windowLogGraph.UpdateGraph();
                        }

                        Window_LogTable_ScrollToEnd();

                        previousTicks = nowTicks;
                        bytesReceived_Previously = bytesReceived;
                        bytesSent_Previously = bytesSent;

                        // AdjustWindowLeftToFitOnScreen();
                    }

                }
            }

        }



        // ========================================================
        //
        // ========================================================
        bool syncLogs = false;
        public bool SyncLogs
        {
            get { return syncLogs; }
            set { syncLogs = value; }
        }

        public void SetSyncLogs(object sender, bool syncLogsFlag)
        {
            this.syncLogs = syncLogsFlag;

            if (sender.GetType().Equals(typeof(WindowLogGraph)))
            {
                if (windowLogTable != null)
                {
                    windowLogTable.SyncLogs = syncLogsFlag;
                }

            }
            if (sender.GetType().Equals(typeof(WindowLogTable)))
            {
                if (windowLogGraph != null)
                {
                    windowLogGraph.SyncLogs = syncLogsFlag;
                }
            }
        }


        // ========================================================
        public void ClearLog()
        {
            this.logList.ClearList();

            if (windowLogTable != null)
            {
                windowLogTable.ListViewLog_Update();
            }
            
        }

        // ========================================================
        public void ExportAsText(double left, double top)
        {
            logList.ExportAsText(left, top);
        }

        // ========================================================
        //
        // ========================================================

        public void ContextMenu_About()
        {
            var dialog = new WindowAbout();
            dialog.ShowDialog();
        }

        // ========================================================
        public void ContextMenu_GotoHomePage()
        {
            try
            {
                var url = "https://nifreka.nl/nnt/";
                var sInfo = new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                };
                System.Diagnostics.Process.Start(sInfo);
            }
            catch (Exception)
            {
                // throw;
            }
        }

        public UpdateChecker updateChecker;
        // ========================================================
        private void CheckForUpdateAuto()
        {
            bool notifyOnlyNewVersionFlag = true;
            CheckForUpdate(notifyOnlyNewVersionFlag);
        }

        public void ContextMenu_CheckForUpdateNow()
        {
            bool notifyOnlyNewVersionFlag = false;
            CheckForUpdate(notifyOnlyNewVersionFlag);
        }

        public void CheckForUpdate(bool notifyOnlyNewVersionFlag)
        {
            if (updateChecker != null)
            {
                updateChecker.Dispose();
            }

            updateChecker = new UpdateChecker();
            updateChecker.CheckForUpdate(notifyOnlyNewVersionFlag);
        }

        // ========================================================
        public void MenuItemSetCheckedFlag(String key, bool value)
        {
            if (windowLogGraph != null)
            {
                windowLogGraph.SetCheckedFlag(key, value);
            }
            if (windowLogTable != null)
            {
                windowLogTable.SetCheckedFlag(key, value);
            }
        }


        // ========================================================
        // ========================================================
        //
        // Selection
        //
        // ========================================================
        // ========================================================
        public void Window_LogTable_ScrollToEnd()
        {
            if (windowLogTable != null)
            {
                windowLogTable.ListViewLog_ScrollToEnd();
                windowLogTable.ListViewLog_Update();
            }
        }

        public void Window_LogTable_ScrollToSelectedItem(LogListItem logListItem)
        {
            if (windowLogTable != null)
            {
                windowLogTable.ListViewLog_ScrollToSelectedItem(logListItem);
            }

        }

        public void Window_LogGraph_ScrollToSelectedIndex(int idx)
        {
            if (windowLogGraph != null)
            {
                windowLogGraph.ScrollToSelectedIndex(idx);
            }
        }

        // ========================================================
        // ========================================================
        //
        // Window Move
        //
        // ========================================================
        // ========================================================

        public void Window_Position_TopLeft()
        {
            MoveWindowsToCorner(Corner.TopLeft);
        }

        public void Window_Position_TopRight()
        {
            MoveWindowsToCorner(Corner.TopRight);
        }

        public void Window_Position_BottomLeft()
        {
            MoveWindowsToCorner(Corner.BottomLeft);
        }

        public void Window_Position_BottomRight()
        {
            MoveWindowsToCorner(Corner.BottomRight);
        }


        // ========================================================
        public void MoveWindowsToCorner(Corner corner)
        {
            if (windowLogGraph.WindowState != WindowState.Normal)
            {
                windowLogGraph.WindowState = WindowState.Normal;
            }

            lastCorner = corner;
            nifrekaNetTrafficSettings.LastCorner = corner;  

            nifrekaNetTrafficSettings.SetDefaultHeight();
            nifrekaNetTrafficSettings.SetDefaultWidth();

            windowLogGraph.SizeToContent = SizeToContent.Manual;

            windowLogGraph.Width = nifrekaNetTrafficSettings.WindowLogGraph_DefaultWidth;
            windowLogGraph.Height = nifrekaNetTrafficSettings.WindowLogGraph_DefaultHeight;

            windowLogGraph.SetViewVariant();

            MoveWindow_to_Corner(windowLogGraph, corner);

            // wait for redraw
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.Render, null);



            // ==============
            if (windowLogTable != null)
            {
                if (windowLogTable.WindowState != WindowState.Normal)
                {
                    windowLogTable.WindowState = WindowState.Normal;
                }

                windowLogTable.Left = windowLogGraph.Left;
                windowLogTable.Width = windowLogGraph.ActualWidth;
                windowLogTable.Height = nifrekaNetTrafficSettings.WindowLogTable_DefaultHeight;

                if (corner == Corner.TopLeft || corner == Corner.TopRight)
                {
                    windowLogTable.Top = windowLogGraph.Top + windowLogGraph.ActualHeight;
                }
                else
                {
                    windowLogTable.Top = windowLogGraph.Top - windowLogTable.ActualHeight;
                }
            }

            // ==============
            windowLogGraph.Activate();

        }



        // ========================================================
        private void MoveWindow_to_Corner(Window window, Corner corner)
        // ========================================================
        {
            System.Windows.Forms.Screen screen = NifrekaScreenUtil.GetScreen_By_Mouse();

            if (screen != null)
            {
                double top_old = window.Top;
                double left_old = window.Left;

                double top_new = 0;
                double left_new = 0;

                switch (corner)
                {
                    case Corner.TopLeft:
                        {
                            top_new = screen.WorkingArea.Top;
                            left_new = screen.WorkingArea.Left;
                            break;
                        }
                    case Corner.TopRight:
                        {
                            top_new = screen.WorkingArea.Top;
                            left_new = screen.WorkingArea.Left + screen.WorkingArea.Width - window.ActualWidth;
                            break;
                        }
                    case Corner.BottomRight:
                        {
                            top_new = screen.WorkingArea.Top + screen.WorkingArea.Height - window.ActualHeight;
                            left_new = screen.WorkingArea.Left + screen.WorkingArea.Width - window.ActualWidth;
                            break;
                        }
                    case Corner.BottomLeft:
                        {
                            top_new = screen.WorkingArea.Top + screen.WorkingArea.Height - window.ActualHeight;
                            left_new = screen.WorkingArea.Left;
                            break;
                        }
                    case Corner.Center:
                        {
                            top_new = screen.WorkingArea.Top + screen.WorkingArea.Height / 2 - window.ActualHeight / 2;
                            left_new = screen.WorkingArea.Left + screen.WorkingArea.Width / 2 - window.ActualWidth / 2;
                            break;
                        }
                    default:
                        break;
                }

                MoveWindow(window, top_new, left_new);
            }
        }


        // ========================================================
        private void MoveWindow(Window window, double top_new, double left_new)
        // ========================================================
        {
            window.Top = top_new;
            window.Left = left_new;
        }

    }
}
