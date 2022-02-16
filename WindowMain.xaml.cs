using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Nifreka;

namespace NifrekaNetTraffic
{
    // ###############################################################
    public partial class WindowMain : Window
    {
        private App app;
        private DispatcherTimer dispatcherTimer;

        public NetAdapterList netAdapterList { get; set; }
        public NetAdapter selectedNetAdapter;

        private long previousTicks;
        private long bytesReceived_Previously;
        private long bytesSent_Previously;

        public LogList logList;

        private Corner corner = Corner.BottomRight;

        private Dictionary<String, int> dictContextMenu_resIdentifierToInt;

        public NifrekaNetTrafficSettings nifrekaNetTrafficSettings;

        private WindowLogTable windowLogTable;
        private WindowLogGraph windowLogGraph;

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






        // ========================
        // ctor
        // ========================
        public WindowMain()
        {
            this.app = (App)Application.Current;

            netAdapterList = new NetAdapterList();
            logList = new LogList();

            nifrekaNetTrafficSettings = new NifrekaNetTrafficSettings();
            corner = Corner.BottomRight;

            // ==================
            InitializeComponent();

            label_Received.ToolTip = Properties.Resources.toolTip_Received;
            label_Sent.ToolTip = Properties.Resources.toolTip_Sent;

            dictContextMenu_resIdentifierToInt = new Dictionary<string, int>();
            CreateContextmenu();

            labelClose.SetWindow(this);

            Create_ComboBox_NetAdapter();

            // SetPreviousIPStatistics
            //
            DateTime dt = DateTime.Now;
            this.previousTicks = dt.Ticks;

            if (this.selectedNetAdapter != null)
            {
                this.bytesReceived_Previously = this.selectedNetAdapter.BytesReceived;
                this.bytesSent_Previously = this.selectedNetAdapter.BytesSent;
            }

            // Additional EventHandlers, see also xaml
            this.Loaded += new RoutedEventHandler(this.Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            this.Closed += new EventHandler(this.Window_Closed);

            this.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Window_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Window_MouseLeave);

            NetworkChange.NetworkAddressChanged += new
            NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged_CallBack);

            NetworkChange.NetworkAvailabilityChanged +=
            new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged_CallBack);

            // DoGuiUpdate();

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dispatcherTimer.IsEnabled = false;
            dispatcherTimer.Tick += DispatcherTimer_Tick;

            logList.ReadFromFile();

        }


        bool windowLoaded = false;
        bool firstRun = false;
        // ========================================================
        private void Window_Loaded(object sender, RoutedEventArgs e)
        // ========================================================
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;

            string filepath = Const.NifrekaNetTraffic_Settings_PATH;
            if (File.Exists(filepath) == true)
            {
                nifrekaNetTrafficSettings.CleanUp_OldDataFile();
                nifrekaNetTrafficSettings.ReadSettingsData();

                this.Top = this.nifrekaNetTrafficSettings.Top_WindowMain;
                this.Left = this.nifrekaNetTrafficSettings.Left_WindowMain;

                this.isTopmost = this.nifrekaNetTrafficSettings.Topmost_WindowMain;
                this.Topmost = isTopmost;
            }
            else
            {   // First Time Start
                firstRun = true;
                MoveWindowsToCorner(Corner.BottomRight);

                this.Topmost = false;
                this.nifrekaNetTrafficSettings.CheckForUpdateAuto = true;
            }

            MenuItemSetCheckedFlag(GetMenuItem_Idx("main_context_Topmost"), isTopmost);
            MenuItemSetCheckedFlag(GetMenuItem_Idx("CheckForUpdateAuto"), this.nifrekaNetTrafficSettings.CheckForUpdateAuto);

            if (this.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable)
            {
                Do_Open_WindowLogTable();
            }
            if (this.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogGraph)
            {
                Do_Open_WindowLogGraph();
            }

            dispatcherTimer.Start();

            if (this.nifrekaNetTrafficSettings.CheckForUpdateAuto)
            {
                if (updateChecker != null)
                {
                    updateChecker.Dispose();
                }

                updateChecker = new UpdateChecker(this);
                bool notifyOnlyNewVersion = true;
                updateChecker.CheckForUpdate(notifyOnlyNewVersion);
            }
            
            windowLoaded = true;

        }

        UpdateChecker updateChecker;

        // ========================================================
        public void MoveWindowsToCorner(Corner corner)
        {
            this.MoveWindow_to_Corner(this, corner);

            // wait for redraw
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.Render, null);

            this.nifrekaNetTrafficSettings.SetDefaultHeight();

            this.nifrekaNetTrafficSettings.Left_WindowLogGraph = this.Left;           
            this.nifrekaNetTrafficSettings.Width_WindowLogGraph = this.ActualWidth;

            this.nifrekaNetTrafficSettings.Left_WindowLogTable = this.Left;          
            this.nifrekaNetTrafficSettings.Width_WindowLogTable = this.ActualWidth;

            if (corner == Corner.BottomLeft || corner == Corner.BottomRight)
            {            
                this.nifrekaNetTrafficSettings.Top_WindowLogGraph = this.Top - this.nifrekaNetTrafficSettings.Height_WindowLogGraph;
                this.nifrekaNetTrafficSettings.Top_WindowLogTable = this.Top - this.nifrekaNetTrafficSettings.Height_WindowLogGraph - this.nifrekaNetTrafficSettings.Height_WindowLogTable;
            }

            if (corner == Corner.TopLeft || corner == Corner.TopRight)
            {               
                this.nifrekaNetTrafficSettings.Top_WindowLogGraph = this.Top + this.ActualHeight;
                this.nifrekaNetTrafficSettings.Top_WindowLogTable = this.Top + this.ActualHeight + this.nifrekaNetTrafficSettings.Height_WindowLogGraph;
            }



            this.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable = true;
            this.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogGraph = true;

            if (windowLogGraph == null)
            {
                Do_Open_WindowLogGraph();
            }
            else
            {
                windowLogGraph.SetWindowPosAndSize(this.Left,
                                                    this.nifrekaNetTrafficSettings.Top_WindowLogGraph,
                                                    this.nifrekaNetTrafficSettings.Width_WindowLogGraph,
                                                    this.nifrekaNetTrafficSettings.WindowLogGraph_DefaultHeight);
                windowLogGraph.Activate();
            }

            if (windowLogTable == null)
            {
                Do_Open_WindowLogTable();
            }
            else
            {
                windowLogTable.SetWindowPosAndSize(this.Left,
                                                    this.nifrekaNetTrafficSettings.Top_WindowLogTable,
                                                    this.nifrekaNetTrafficSettings.Width_WindowLogTable,
                                                    this.nifrekaNetTrafficSettings.Height_WindowLogTable);
                windowLogTable.Activate();
            }

        }

    

        // ========================================================
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        // ========================================================
        {
            if (windowLogTable != null)
            {
                this.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable = true;
            }
            else
            {
                this.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable = false;
            }

            if (windowLogGraph != null)
            {
                this.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogGraph = true;
            }
            else
            {
                this.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogGraph = false;
            }
        }

        // ========================================================
        private void Window_Closed(object sender, EventArgs e)
        // ========================================================
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
            }

            if (windowLogTable != null)
            {
                windowLogTable.Close();
            }

            if (windowLogGraph != null)
            {
                windowLogGraph.Close();
            }

            if (updateChecker != null)
            {
                updateChecker.Dispose();
            }

            


            // ================================
            WriteSettings();
            WriteLog();

        }



        // ========================================================
        private void WriteSettings()
        {
            this.nifrekaNetTrafficSettings.Top_WindowMain = this.Top;
            this.nifrekaNetTrafficSettings.Left_WindowMain = this.Left;
            this.nifrekaNetTrafficSettings.Topmost_WindowMain = this.isTopmost;

            this.nifrekaNetTrafficSettings.WriteSettingsData();
        }

        // ========================================================
        private void WriteLog()
        {
            this.logList.WriteToFile();
        }



        // ========================================================
        public void Do_Open_WindowLogTable()
        {
            if (this.windowLogTable == null)
            {
                this.windowLogTable = new WindowLogTable(this);
                this.windowLogTable.Show();
            }
            else
            {
                if (windowLogTable.WindowState == WindowState.Minimized)
                {
                    windowLogTable.WindowState = WindowState.Normal;
                }

                this.windowLogTable.Activate();
            }
        }

        public void Unregister_WindowLogTable()
        {
            windowLogTable = null;
        }


        // ========================================================
        public void Do_Open_WindowLogGraph()
        {
            if (this.windowLogGraph == null)
            {
                this.windowLogGraph = new WindowLogGraph(this);
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

        public void Unregister_WindowLogGraph()
        {
            windowLogGraph = null;
        }

        // ========================================================
        public void ClearLog()
        {
            this.logList.ClearList();
        }

        // ========================================================
        public void WindowLogGraph_DoPause()
        {
            if (this.windowLogGraph != null)
            {
                this.windowLogGraph.DoPause();
            }
        }

        public void WindowLogGraph_DoContinue()
        {
            if (this.windowLogGraph != null)
            {
                this.windowLogGraph.DoContinue();
            }
        }


        // ========================================================
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        // ========================================================
        {
            DoGuiUpdate();
            if (this.windowLogGraph != null)
            {
                this.windowLogGraph.UpdateGraph();
            }
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

                    textBlock_Bytes_Received.Text = bytesReceived.ToString("#,##0") + "  B";
                    textBlock_Bytes_Sent.Text = bytesSent.ToString("#,##0") + "  B";

                    long bytesReceived_Interval = bytesReceived - bytesReceived_Previously;
                    long bytesSent_Interval = bytesSent - bytesSent_Previously;

                    textBlock_Bytes_Received_Interval.Text = bytesReceived_Interval.ToString("#,##0") + "  B";
                    textBlock_Bytes_Sent_Interval.Text = bytesSent_Interval.ToString("#,##0") + "  B";

                    // long ticksInterval = nowTicks - previousTicks;
                    // textBlock_TicksInterval.Text = ticksInterval.ToString("#,##0");

                    this.previousTicks = nowTicks;

                    bytesReceived_Previously = bytesReceived;
                    bytesSent_Previously = bytesSent;

                    // =====
                    // for history

                    LogListItem logListItem = new LogListItem(nowTicks,
                                                            bytesReceived_Interval,
                                                            bytesSent_Interval);
                    logList.AddItem(logListItem);

                    logList.BytesReceivedTotal = bytesReceived;
                    logList.BytesSentTotal = bytesSent;

                    Window_LogTable_ScrollToEnd();

                    AdjustWindowLeftToFitOnScreen();

                }
            }


        }

        // ========================================================
        private void AdjustWindowLeftToFitOnScreen()
        {
            if (windowLoaded == true)
            {
                System.Windows.Forms.Screen screen = NifrekaScreenUtil.GetScreen_by_Window_Intersects_Most(this);

                if (screen != null)
                {
                    double window_Left = this.Left;
                    double wimdow_Right = this.Left + this.ActualWidth;

                    double screen_Right = screen.WorkingArea.Left + screen.WorkingArea.Width;
                    if (wimdow_Right > screen_Right)
                    {
                        double moveLeft = wimdow_Right - screen_Right;
                        this.Left = window_Left - moveLeft;
                    }
                }

            }

        }

        // ========================================================
        public void Window_LogTable_ScrollToEnd()
        {
            if (this.windowLogTable != null)
            {
                this.windowLogTable.ListViewLog_ScrollToEnd();
                this.windowLogTable.ListViewLog_Update();
            }
        }

        public void Window_LogTable_ScrollToSelectedItem(LogListItem logListItem)
        {
            if (this.windowLogTable != null)
            {
                this.windowLogTable.ListViewLog_ScrollToSelectedItem(logListItem);
            }

        }

        public void Window_LogGraph_ScrollToSelectedIndex(int idx)
        {
            if (windowLogGraph != null)
            {
                windowLogGraph.ScrollToSelectedIndex(idx);
            }
        }


        public void Window_LogTable_SetSelectionRange(int idx_Start, int idx_End)
        {
            if (windowLogGraph != null)
            {
                this.windowLogTable.ListViewLog_SetSelectionRange(idx_Start, idx_End);
            }
        }

        // ========================================================
        private void NetworkChange_NetworkAddressChanged_CallBack(object sender, EventArgs e)
        {
            Create_ComboBox_NetAdapter();
        }

        // ========================================================
        private void NetworkChange_NetworkAvailabilityChanged_CallBack(object sender, EventArgs e)
        {
            Create_ComboBox_NetAdapter();
        }


        // ========================================================
        delegate void Create_ComboBox_NetAdapter_Delegate();
        private void Create_ComboBox_NetAdapter()
        // ====================================
        {
            if (this.Dispatcher.CheckAccess())
            {
                Delegated_Create_ComboBox_NetAdapter();
            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Create_ComboBox_NetAdapter_Delegate(Create_ComboBox_NetAdapter));
            }
        }

        // ====================================
        private void Delegated_Create_ComboBox_NetAdapter()
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
            }
            catch (Exception)
            {

                throw;
            }
        }

        // ========================================================
        private void ComboBox_NetAdapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        // ========================================================
        bool isMouseButtonState_Pressed = false;
        // ========================================================
        public void AdjustWindowToFitOnScreen()
        {
            System.Windows.Forms.Screen screen = NifrekaScreenUtil.GetScreen_By_Mouse();

            if (isMouseButtonState_Pressed)
            {
                screen = NifrekaScreenUtil.GetScreen_By_Mouse();
            }
            else
            {
                screen = NifrekaScreenUtil.GetScreen_By_WindowCenter(this);
            }

            if (screen != null)
            {
                if (!Double.IsNaN(this.Left)
                    &&
                    !Double.IsNaN(this.Top))
                {
                    if ((this.Left + this.ActualWidth) > (screen.WorkingArea.Right))
                    {
                        this.Left = screen.WorkingArea.Right - this.ActualWidth;
                    }

                    if (this.Left < screen.WorkingArea.Left)
                    {
                        this.Left = screen.WorkingArea.Left;
                    }

                    if ((this.Top + this.ActualHeight) > (screen.WorkingArea.Bottom))
                    {
                        this.Top = screen.WorkingArea.Bottom - this.ActualHeight;
                    }

                    if (this.Top < screen.WorkingArea.Top)
                    {
                        this.Top = screen.WorkingArea.Top;
                    }

                }
            }
        }



        // ========================================================
        public void MoveWindow_to_Corner(Window window, Corner corner)
        // ========================================================
        {
            this.corner = corner;

            System.Windows.Forms.Screen screen = NifrekaScreenUtil.GetScreen_By_WindowCenter(this);

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

                MoveWindow(this, top_new, left_new);
            }
        }


        // ========================================================
        private void MoveWindow(Window window, double top_new, double left_new)
        // ========================================================
        {
            window.Top = top_new;
            window.Left = left_new;
        }


        // ========================================================
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isMouseOverImageGraph || isMouseOverImageTable)
            {
                if (isMouseOverImageGraph)
                {
                    sender = image_Graph;
                    image_Graph_MouseLeftButtonDown(sender, e);
                }

                if (isMouseOverImageTable)
                {
                    sender = image_Table;
                    image_Table_MouseLeftButtonDown(sender, e);
                }
            }

            else
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    isMouseButtonState_Pressed = true;
                    dispatcherTimer.Stop();
                    this.DragMove();

                    this.corner = Corner.NoCorner;
                }

                e.Handled = false;
            }

        }

        // ========================================================
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isMouseOverImageGraph || isMouseOverImageTable)
            {
                if (isMouseOverImageGraph)
                {
                    sender = image_Graph;
                    image_Graph_MouseLeftButtonUp(sender, e);
                }

                if (isMouseOverImageTable)
                {
                    sender = image_Table;
                    image_Table_MouseLeftButtonUp(sender, e);
                }
            }
            else
            {
                AdjustWindowToFitOnScreen();

                dispatcherTimer.Start();
                isMouseButtonState_Pressed = false;

                e.Handled = false;
            }

        }


        // ========================================================
        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;

            if (labelClose != null)
            {
                labelClose.SetVisible();
            }
        }

        // ========================================================
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;

            if (labelClose != null)
            {
                labelClose.SetInVisible();
            }

            e.Handled = true;
        }


        // ========================================================
        // CreateContextmenu
        // ========================================================

        private void CreateContextmenu()
        {

            this.ContextMenu = new ContextMenu();

            // --------------------------------
            // main_context_About
            // --------------------------------
            int menuItem_Idx = 0;
            ContextMenu_Add_WithIcon("main_context_About",
                                    menuItem_Idx, Properties.Resources.main_context_About,
                                    "NifrekaNetTraffic_18x18.png", delegate { Main_context_About_Click(); });


            // --------------------------------
            // main_context_GotoHomePage
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("main_context_GotoHomePage",
                                menuItem_Idx, Properties.Resources.main_context_GotoHomePage,
                                delegate { Main_context_GotoHomePage_Click(); });

            // --------------------------------
            // Window_CheckForUpdate
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("CheckForUpdateNow",
                                menuItem_Idx, Properties.Resources.CheckForUpdateNow,
                                delegate { CheckForUpdateNow(); });


            // --------------------------------
            // Window_CheckForUpdate
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("CheckForUpdateAuto",
                                menuItem_Idx, Properties.Resources.CheckForUpdateAuto,
                                delegate { CheckForUpdateAuto(); });



            /*
            // --------------------------------
            // main_context_OpenReadme
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("main_context_OpenReadme",
                                menuItem_Idx, Properties.Resources.main_context_OpenReadme,
                                delegate { Main_context_OpenReadMe_Click(); });

            */


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();

            // --------------------------------
            // main_context_OpenGraph
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("main_context_OpenGraph",
                                menuItem_Idx, Properties.Resources.main_context_OpenGraph,
                                delegate { Main_context_OpenGraph_Click(); });

            // --------------------------------
            // main_context_OpenGraph
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("main_context_OpenTable",
                                menuItem_Idx, Properties.Resources.main_context_OpenTable,
                                delegate { Main_context_OpenTable_Click(); });

            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();


            // --------------------------------
            // Window_Position_TopLeft
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_TopLeft",
                                    menuItem_Idx, Properties.Resources.Window_Position_TopLeft,
                                    "PositionTopLeft.png", delegate { Window_Position_TopLeft(); });


            // --------------------------------
            // Window_Position_TopRight
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_TopRight",
                                    menuItem_Idx, Properties.Resources.Window_Position_TopRight,
                                    "PositionTopRight.png", delegate { Window_Position_TopRight(); });


            // --------------------------------
            // Window_Position_BottomLeft
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_BottomLeft",
                                    menuItem_Idx, Properties.Resources.Window_Position_BottomLeft,
                                    "PositionBottomLeft.png", delegate { Window_Position_BottomLeft(); });

            // --------------------------------
            // Window_Position_BottomRight
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_BottomRight",
                                    menuItem_Idx, Properties.Resources.Window_Position_BottomRight,
                                    "PositionBottomRight.png", delegate { Window_Position_BottomRight(); });


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();

            // --------------------------------
            // main_context_Top
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("main_context_Topmost", 
                                menuItem_Idx, Properties.Resources.main_context_Topmost, 
                                delegate { Main_context_Topmost_Click(); });


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();


            // --------------------------------
            // main_context_Close
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("main_context_Close",
                                    menuItem_Idx, Properties.Resources.main_context_Close,
                                    "Close_18.png", delegate { Main_context_Close_Click(); });

        }

        // ========================================================
        // ContextMenu Utils
        // ========================================================
        private void Contextmenu_addSeparator()
        {
            Separator s = new Separator();
            this.ContextMenu.Items.Add(s);
        }

        // ========================================================
        private void ContextMenu_Add(String resIdentifier, int menuItem_Idx, String menuItem_Header, RoutedEventHandler routedEventHandler)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = menuItem_Header;
            menuItem.Click += routedEventHandler;
            this.ContextMenu.Items.Add(menuItem);

            dictContextMenu_resIdentifierToInt.Add(resIdentifier, menuItem_Idx);
        }

        // ========================================================
        private void ContextMenu_Add_WithIcon(String resIdentifier, int menuItem_Idx, String menuItem_Header, String resFilename, RoutedEventHandler routedEventHandler)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = menuItem_Header;
            menuItem.Click += routedEventHandler;
            menuItem.Icon = GetImageFromResource(resFilename);

            this.ContextMenu.Items.Add(menuItem);
            dictContextMenu_resIdentifierToInt.Add(resIdentifier, menuItem_Idx);
        }

        // ========================================================
        private Image GetImageFromResource(String resId)
        {
            Image image = new Image();

            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(@"\Resources\" + resId, UriKind.RelativeOrAbsolute);
                bi.EndInit();

                image.Source = bi;
            }
            catch (Exception ex)
            {
                // throw;
            }

            return image;
        }

        // ========================================================
        private void MenuItemSetCheckedFlag(int itemIndex, Boolean flag)
        {
            ItemCollection ic = this.ContextMenu.Items;
            MenuItem menuItem = (MenuItem)ic.GetItemAt(itemIndex);
            menuItem.IsChecked = flag;
        }

        // ========================================================
        public int GetMenuItem_Idx(String key)
        {
            int value = 0;
            dictContextMenu_resIdentifierToInt.TryGetValue(key, out value);
            return value;
        }



        // ========================================================
        // Menu Clicks
        // ========================================================
        private void Main_context_About_Click()
        {
            Do_About();
        }

        private void Main_context_GotoHomePage_Click()
        {
            Do_GotoHomePage();
        }

        private void CheckForUpdateNow()
        {
            if (updateChecker != null)
            {
                updateChecker.Dispose();
            }

            updateChecker = new UpdateChecker(this);
            bool notifyOnlyNewVersion = false;
            updateChecker.CheckForUpdate(notifyOnlyNewVersion);
        }

        private void CheckForUpdateAuto()
        {
            Do_Toggle_CheckForUpdateAuto();
        }

        private void Do_Toggle_CheckForUpdateAuto()
        {
            this.nifrekaNetTrafficSettings.CheckForUpdateAuto = !this.nifrekaNetTrafficSettings.CheckForUpdateAuto;

            int id = GetMenuItem_Idx("CheckForUpdateAuto");
            MenuItemSetCheckedFlag(id, this.nifrekaNetTrafficSettings.CheckForUpdateAuto);
        }




        /*
        private void Main_context_OpenReadMe_Click()
        {
            Do_OpenReadMe();
        }
        */

        private void Main_context_OpenGraph_Click()
        {
            Do_Open_WindowLogGraph();
        }

        private void Main_context_OpenTable_Click()
        {
            Do_Open_WindowLogTable();
        }

        private void Window_Position_TopLeft()
        {
            MoveWindowsToCorner(Corner.TopLeft);
        }

        private void Window_Position_TopRight()
        {
            MoveWindowsToCorner(Corner.TopRight);
        }

        private void Window_Position_BottomLeft()
        {
            MoveWindowsToCorner(Corner.BottomLeft);
        }

        private void Window_Position_BottomRight()
        {
            MoveWindowsToCorner(Corner.BottomRight);
        }

        
        private void Main_context_Topmost_Click()
        {
            Do_Toggle_Topmost();
        }

        private void Main_context_Close_Click() 
        {
            Do_Close(); 
        }

        // ========================================================
        // Menu Commands
        // ========================================================
        private void Do_About()
        {
            var dialog = new WindowAbout();
            dialog.ShowDialog();
        }

        // ========================================================
        public void Do_GotoHomePage()
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

        // ========================================================






        /*
        // ========================================================
        private void DoOpenReadMe()
        {
            
            if (windowReadme == null)
            {
                OpenAndRegisterWindowReadme();
            }
            else
            {
                this.windowReadme.Activate();
            }
            

            // DoOpenReadMeURL();
            // DoOpenReadMeWithLocalProg();
        }
        */

        /*
        // ========================================================

        private void DoOpenReadMeWithLocalProg()
        {
            try
            {
                String exepath = NifrekaPathUtil.GetAppAbsolutePath();
                FileInfo fileinfo = new FileInfo(exepath);
                String exeDirPath = fileinfo.DirectoryName;

                String filename_Readme = NifrekaNetTraffic.Properties.Resources.filename_Readme_rtf;
                String filepath_Readme = exeDirPath + @"\" + filename_Readme;

                var sInfo = new System.Diagnostics.ProcessStartInfo(filepath_Readme)
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
        */

        // ========================================================
        private void Do_MoveWindow_to_Corner_TopLeft()
        {
            this.MoveWindow_to_Corner(this, Corner.TopLeft);
        }

        // ===========================================
        private void Do_MoveWindow_to_Corner_TopRight()
        {
            this.MoveWindow_to_Corner(this, Corner.TopRight);
        }

        // ===========================================
        private void Do_MoveWindow_to_Corner_BottomLeft()
        {
            this.MoveWindow_to_Corner(this, Corner.BottomLeft);
        }

        // ===========================================
        private void Do_MoveWindow_to_Corner_BottomRight()
        {
            this.MoveWindow_to_Corner(this, Corner.BottomRight);
        }


        // ========================================================
        private bool isTopmost = false;
        private void Do_Toggle_Topmost()
        {
            isTopmost = !isTopmost;
            this.Topmost = isTopmost;

            int id = GetMenuItem_Idx("main_context_Topmost");
            MenuItemSetCheckedFlag(id, isTopmost);
        }

        // ========================================================
        private void Do_Close()
        {
            this.Close();
        }



        // ========================================================
        // Window_KeyUp
        // ========================================================
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.L))
            {
                Do_Open_WindowLogTable();

            }

            if (e.Key.Equals(Key.G))
            {
                Do_Open_WindowLogGraph();

            }

        }




        // ========================================================
        // MouseEvents
        // ========================================================
        private void comboBox_NetAdapter_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void comboBox_NetAdapter_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        // ========================================================
        bool isMouseOverImageGraph = false;
        private void image_Graph_MouseEnter(object sender, MouseEventArgs e)
        {
            border_Graph.BorderBrush = Brushes.Cyan;
            isMouseOverImageGraph = true;
            this.Cursor = Cursors.Arrow;
        }

        private void image_Graph_MouseLeave(object sender, MouseEventArgs e)
        {
            border_Graph.BorderBrush = Brushes.White;
            isMouseOverImageGraph = false;
            this.Cursor = Cursors.Hand;
        }

        private void image_Graph_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            border_Graph.BorderBrush = Brushes.Red;
        }
        private void image_Graph_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Do_Open_WindowLogGraph();
            border_Graph.BorderBrush = Brushes.White;
        }

        // ========================================================
        bool isMouseOverImageTable = false;
        private void image_Table_MouseEnter(object sender, MouseEventArgs e)
        {
            border_Table.BorderBrush = Brushes.Cyan;
            isMouseOverImageTable = true;
            this.Cursor = Cursors.Arrow;
        }

        private void image_Table_MouseLeave(object sender, MouseEventArgs e)
        {
            border_Table.BorderBrush = Brushes.White;
            isMouseOverImageTable = false;
            this.Cursor = Cursors.Hand;
        }

        private void image_Table_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            border_Table.BorderBrush = Brushes.Red;
        }

        private void image_Table_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Do_Open_WindowLogTable();
            border_Table.BorderBrush = Brushes.White;
        }


        // ========================================================
        // ExportAsText
        // ========================================================

        public void ExportAsText()
        {
            logList.ExportAsText(this.Left, this.Top);
        }
       
        
        // ========================================================
    }
}
