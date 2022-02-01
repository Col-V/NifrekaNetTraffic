using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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


    // #######################################################################
    public partial class WindowMain : Window
    {
        private App app;
        private DispatcherTimer dispatcherTimer;

        private NetAdapterList netAdapterList { get; set; }
        private NetAdapter selectedNetAdapter;

        private long lastTicks;
        private long bytesReceived_Last;
        private long bytesSent_Last;

        private Corner corner = Corner.BottomRight;

        private Dictionary<String, int> dictContextMenu_resIdentifierToInt;

        private NifrekaNetTrafficSettings nifrekaNetTrafficSettings;

        // public WindowReadme windowReadme;

        // ========================
        // ctor
        // ========================
        public WindowMain()
        {
            this.app = (App)Application.Current;

            netAdapterList = new NetAdapterList();

            nifrekaNetTrafficSettings = new NifrekaNetTrafficSettings();
            corner = Corner.BottomRight;

            InitializeComponent();

            label_Received.ToolTip = Properties.Resources.toolTip_Received;
            label_Sent.ToolTip = Properties.Resources.toolTip_Sent;

            dictContextMenu_resIdentifierToInt = new Dictionary<string, int>();
            CreateContextmenu();

            labelClose.SetWindow(this);

            Create_ComboBox_NetAdapter();

            DateTime dt = DateTime.Now;
            lastTicks = dt.Ticks;

            bytesReceived_Last = GetBytesReceived();
            bytesSent_Last = GetBytesSent();

            this.Loaded += new RoutedEventHandler(this.Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            this.Closed += new EventHandler(this.Window_Closed);

            this.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Window_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Window_MouseLeave);

            NetworkChange.NetworkAddressChanged += new
            NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged_CallBack);

            NetworkChange.NetworkAvailabilityChanged +=
            new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged_CallBack);

            DoGuiUpdate();

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000); // 250 Millisekunden
            dispatcherTimer.IsEnabled = false;
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }


        bool windowLoaded = false;
        // ========================================================
        private void Window_Loaded(object sender, RoutedEventArgs e)
        // ========================================================
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;

            

            string filepath = Const.NifrekaNetTraffic_Settings_PATH;
            if (File.Exists(filepath) == true)
            {
                nifrekaNetTrafficSettings.ReadSettingsData();

                this.Top = this.nifrekaNetTrafficSettings.WindowPos_Top;
                this.Left = this.nifrekaNetTrafficSettings.WindowPos_Left;

                this.isTopmost = this.nifrekaNetTrafficSettings.Window_Topmost;
                this.Topmost = isTopmost;

                int id = GetMenuItem_Idx("main_context_Topmost");
                MenuItemSetCheckedFlag(id, isTopmost);
            }
            else
            {
                this.MoveWindow_to_Corner(this, corner);
                
            }

            

            dispatcherTimer.Start();

            windowLoaded = true;

        }



        // ========================================================
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        // ========================================================
        {
            this.nifrekaNetTrafficSettings.WindowPos_Top = this.Top;
            this.nifrekaNetTrafficSettings.WindowPos_Left = this.Left;
            this.nifrekaNetTrafficSettings.Window_Topmost = this.isTopmost;


            this.nifrekaNetTrafficSettings.WriteSettingsData();   
        }

        // ========================================================
        private void Window_Closed(object sender, EventArgs e)
        // ========================================================
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
            }

            /*
            if (windowReadme != null)
            {
                windowReadme.Close();
            }
            */

        }

        /*
        // ========================================================
        public void OpenAndRegisterWindowReadme()
        {

            this.windowReadme = new WindowReadme(this);
            this.windowReadme.Show();  

        }

        // ========================================================
        public void UnregisterWindowReadme()
        {
            windowReadme = null;
        }
        */




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


                    long bytesReceived = selectedNetAdapter.networkInterface.GetIPStatistics().BytesReceived;
                    long bytesSent = selectedNetAdapter.networkInterface.GetIPStatistics().BytesSent;

                    textBlock_Bytes_Received.Text = bytesReceived.ToString("#,##0") + "  B";
                    textBlock_Bytes_Sent.Text = bytesSent.ToString("#,##0") + "  B";

                    long bytesReceived_Diff = bytesReceived - bytesReceived_Last;
                    long bytesSent_Diff = bytesSent - bytesSent_Last;

                    textBlock_Bytes_Received_Diff.Text = bytesReceived_Diff.ToString("#,##0") + "  B";
                    textBlock_Bytes_Sent_Diff.Text = bytesSent_Diff.ToString("#,##0") + "  B";

                    // long ticksDiff = nowTicks - lastTicks;
                    // textBlock_TicksDiff.Text = ticksDiff.ToString("#,##0");

                    lastTicks = nowTicks;

                    bytesReceived_Last = bytesReceived;
                    bytesSent_Last = bytesSent;


                    AdjustRight();

                }
            }


        }

        // ========================================================
        private void AdjustRight()
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
        private long GetBytesReceived()
        {
            long bytesReceived = 0;

            if (selectedNetAdapter != null)
            {
                if (selectedNetAdapter.networkInterface != null)
                {
                    bytesReceived = selectedNetAdapter.networkInterface.GetIPStatistics().BytesReceived;
                }
            }

            return bytesReceived;
        }

        private long GetBytesSent()
        {
            long bytesSent = 0;

            if (selectedNetAdapter != null)
            {
                if (selectedNetAdapter.networkInterface != null)
                {
                    bytesSent = selectedNetAdapter.networkInterface.GetIPStatistics().BytesSent;
                }
            }

            return bytesSent;
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

            comboBox_NetAdapter.Items.Clear();

            for (int i = 0; i < netAdapterList.Count(); i++)
            {
                NetAdapter netAdapter = netAdapterList[i];
                comboBox_NetAdapter.Items.Add(netAdapter.networkInterface.Name);
            }

            if (comboBox_NetAdapter.Items.Count > 0)
            {
                comboBox_NetAdapter.SelectedIndex = 0;
                selectedNetAdapter = netAdapterList[comboBox_NetAdapter.SelectedIndex];
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

            NetworkInterface[] networkInterfaceArr = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaceArr)
            {
                netAdapterList.Add(new NetAdapter(networkInterface));
            }
        }

        // ========================================================
        private void ComboBox_NetAdapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboBox_NetAdapter.SelectedIndex >= 0)
            {
                selectedNetAdapter = netAdapterList[comboBox_NetAdapter.SelectedIndex];
            }

        }

        // ========================================================


        bool isMouseButtonState_Pressed = false;
        // ========================================================
        public void WindowAdjust()
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
        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            DoMenuItem_About();

        }

        private void DoMenuItem_About()
        {
            var dialog = new WindowAbout();
            dialog.ShowDialog();
        }

        // ========================================================
        private void DoGotoHomePage()
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
        private void DoOpenReadMe()
        {
            /*
            if (windowReadme == null)
            {
                OpenAndRegisterWindowReadme();
            }
            else
            {
                this.windowReadme.Activate();
            }
            */

            // DoOpenReadMeURL();
            DoOpenWithLocalProg();
        }


        // ========================================================

        private void DoOpenWithLocalProg()
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

        // ========================================================
        private void MenuItem_Position_TopLeft_Click(object sender, RoutedEventArgs e)
        {
            DoMoveWindow_to_Corner_TopLeft();
        }
        private void DoMoveWindow_to_Corner_TopLeft()
        {
            this.MoveWindow_to_Corner(this, Corner.TopLeft);
        }

        // ===========================================
        private void MenuItem_Position_TopRight_Click(object sender, RoutedEventArgs e)
        {
            DoMoveWindow_to_Corner_TopRight();
        }

        private void DoMoveWindow_to_Corner_TopRight()
        {
            this.MoveWindow_to_Corner(this, Corner.TopRight);
        }

        // ===========================================

        private void MenuItem_Position_BottomLeft_Click(object sender, RoutedEventArgs e)
        {
            DoMoveWindow_to_Corner_BottomLeft();
        }

        private void DoMoveWindow_to_Corner_BottomLeft()
        {
            this.MoveWindow_to_Corner(this, Corner.BottomLeft);
        }

        // ===========================================
        private void MenuItem_Position_BottomRight_Click(object sender, RoutedEventArgs e)
        {
            DoMoveWindow_to_Corner_BottomRight();
        }

        private void DoMoveWindow_to_Corner_BottomRight()
        {
            this.MoveWindow_to_Corner(this, Corner.BottomRight);
        }

        // ========================================================
        private bool isTopmost = false;
        private void MenuItem_Topmost_Click(object sender, RoutedEventArgs e)
        {

            DoMenuItem_Topmost_Click();
        }

        private void DoMenuItem_Topmost_Click()
        {
            isTopmost = !isTopmost;
            this.Topmost = isTopmost;

            int id = GetMenuItem_Idx("main_context_Topmost");
            MenuItemSetCheckedFlag(id, isTopmost);
        }

        // ========================================================
        private void MenuItem_File_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                isMouseButtonState_Pressed = true;
                dispatcherTimer.Stop();
                this.DragMove();

                this.corner = Corner.NoCorner;
            }
        }

        // ========================================================
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowAdjust();

            dispatcherTimer.Start();
            isMouseButtonState_Pressed = false;
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
        // Context Menu
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
            // main_context_Position_TopLeft
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("main_context_Position_TopLeft",
                                    menuItem_Idx, Properties.Resources.main_context_Position_TopLeft,
                                    "PositionTopLeft.png", delegate { Main_context_Position_TopLeft_Click(); });


            // --------------------------------
            // main_context_Position_TopRight
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("main_context_Position_TopRight",
                                    menuItem_Idx, Properties.Resources.main_context_Position_TopRight,
                                    "PositionTopRight.png", delegate { Main_context_Position_TopRight_Click(); });


            // --------------------------------
            // main_context_Position_BottomLeft
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("main_context_Position_BottomLeft",
                                    menuItem_Idx, Properties.Resources.main_context_Position_BottomLeft,
                                    "PositionBottomLeft.png", delegate { Main_context_Position_BottomLeft_Click(); });

            // --------------------------------
            // main_context_Position_BottomRight
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("main_context_Position_BottomRight",
                                    menuItem_Idx, Properties.Resources.main_context_Position_BottomRight,
                                    "PositionBottomRight.png", delegate { Main_context_Position_BottomRight_Click(); });


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();


            // --------------------------------
            // main_context_Top
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("main_context_Topmost", menuItem_Idx, Properties.Resources.main_context_Topmost, delegate { Main_context_Topmost_Click(); });


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
        // -----------------------------------------------------
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
            DoMenuItem_About(); 
        }

        private void Main_context_GotoHomePage_Click()
        {
            DoGotoHomePage();
        }

        /*
        private void Main_context_OpenReadMe_Click()
        {
            DoOpenReadMe();
        }
        */

        private void Main_context_Position_TopLeft_Click()
        {
            DoMoveWindow_to_Corner_TopLeft();
        }

        private void Main_context_Position_TopRight_Click()
        {
            DoMoveWindow_to_Corner_TopRight();
        }

        private void Main_context_Position_BottomLeft_Click()
        {
            DoMoveWindow_to_Corner_BottomLeft();
        }

        private void Main_context_Position_BottomRight_Click()
        {
            DoMoveWindow_to_Corner_BottomRight();
        }

        private void Main_context_Topmost_Click()
        {
            DoMenuItem_Topmost_Click();
        }



        private void Main_context_Close_Click() { this.Close(); }

    }
}
