// ==============================
// Copyright 2022 nifreka.nl
// ==============================

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
        public App app;
        private Dictionary<String, int> dictContextMenu_resIdentifierToInt;

        // ========================
        // ctor
        // ========================
        public WindowMain()
        {
            this.app = (App)Application.Current;

            // ==================
            InitializeComponent();

            this.Left = app.nifrekaNetTrafficSettings.Left_WindowMain;
            this.Top = app.nifrekaNetTrafficSettings.Top_WindowMain;

            label_Received.ToolTip = Properties.Resources.toolTip_Received;
            label_Sent.ToolTip = Properties.Resources.toolTip_Sent;

            dictContextMenu_resIdentifierToInt = new Dictionary<string, int>();
            CreateContextMenu();

            labelExit.SetWindow(this);

            // Additional EventHandlers, see also xaml
            this.Loaded += new RoutedEventHandler(this.Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            this.Closed += new EventHandler(this.Window_Closed);

            this.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Window_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Window_MouseLeave);
        }

        bool windowLoaded = false;
        // ========================================================
        private void Window_Loaded(object sender, RoutedEventArgs e)
        // ========================================================
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;

            this.Topmost = app.nifrekaNetTrafficSettings.Topmost_WindowMain;
            MenuItemSetCheckedFlag(GetMenuItem_Idx("ContextMenu_Topmost"), app.nifrekaNetTrafficSettings.Topmost_WindowMain);
            MenuItemSetCheckedFlag(GetMenuItem_Idx("ContextMenu_CheckForUpdateAuto"), app.nifrekaNetTrafficSettings.CheckForUpdateAuto);

            windowLoaded = true;

            app.DispatcherTimer_Start();

        }

        // ========================================================
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        // ========================================================
        {
            if (app.windowLogTable != null)
            {
                app.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable = true;
            }
            else
            {
                app.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable = false;
            }

            if (app.windowLogGraph != null)
            {
                app.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogGraph = true;
            }
            else
            {
                app.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogGraph = false;
            }
            
        }

        // ========================================================
        private void Window_Closed(object sender, EventArgs e)
        // ========================================================
        {
            app.DispatcherTimer_Stop();
            WriteLog();
            app.CloseAllWindows();
            WriteSettings();
        }

        // ========================================================
        private void WriteLog()
        {
            app.logList.WriteToFile();
        }

        // ========================================================
        private void WriteSettings()
        {
            app.nifrekaNetTrafficSettings.Top_WindowMain = this.Top;
            app.nifrekaNetTrafficSettings.Left_WindowMain = this.Left;
            app.nifrekaNetTrafficSettings.Topmost_WindowMain = this.Topmost;

            app.nifrekaNetTrafficSettings.WriteSettingsData();
        }

        // ========================================================
        public void Do_Open_WindowLogTable()
        {
            app.Do_Open_WindowLogTable();
        }

        // ========================================================
        public void Do_Open_WindowLogGraph()
        {
            app.Do_Open_WindowLogGraph();
        }

        // ========================================================
        public void WindowLogGraph_DoPause()
        {
            if (app.windowLogGraph != null)
            {
                app.windowLogGraph.DoPause();
            }
        }

        public void WindowLogGraph_DoContinue()
        {
            if (app.windowLogGraph != null)
            {
                app.windowLogGraph.DoContinue();
            }
        }

        // ========================================================
        public void GuiUpdate(long bytesReceived,
                              long bytesSent,
                              long bytesReceived_Previously,
                              long bytesSent_Previously)
        // ========================================================
        {
            textBlock_Bytes_Received.Text = bytesReceived.ToString("#,##0") + "  B";
            textBlock_Bytes_Sent.Text = bytesSent.ToString("#,##0") + "  B";

            long bytesReceived_Interval = bytesReceived - bytesReceived_Previously;
            long bytesSent_Interval = bytesSent - bytesSent_Previously;

            textBlock_Bytes_Received_Interval.Text = bytesReceived_Interval.ToString("#,##0") + "  B";
            textBlock_Bytes_Sent_Interval.Text = bytesSent_Interval.ToString("#,##0") + "  B";

            AdjustWindowLeftToFitOnScreen();
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
                    // dispatcherTimer.Stop();
                    this.DragMove();
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

                // dispatcherTimer.Start();
                isMouseButtonState_Pressed = false;

                e.Handled = false;
            }

        }

        // ========================================================
        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;

            if (labelExit != null)
            {
                labelExit.SetVisible();
            }
        }

        // ========================================================
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;

            if (labelExit != null)
            {
                labelExit.SetInVisible();
            }

            e.Handled = true;
        }

        // ========================================================
        // CreateContextmenu
        // ========================================================

        private void CreateContextMenu()
        {

            this.ContextMenu = new ContextMenu();

            // --------------------------------
            // ContextMenu_About
            // --------------------------------
            int menuItem_Idx = 0;
            ContextMenu_Add_WithIcon("ContextMenu_About",
                                    menuItem_Idx, Properties.Resources.ContextMenu_About,
                                    "NifrekaNetTraffic_18x18.png", delegate { app.ContextMenu_About(); });


            // --------------------------------
            // ContextMenu_GotoHomePage
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("ContextMenu_GotoHomePage",
                                menuItem_Idx, Properties.Resources.ContextMenu_GotoHomePage,
                                delegate { app.ContextMenu_GotoHomePage(); });


            // --------------------------------
            // ContextMenu_CheckForUpdateNow
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("ContextMenu_CheckForUpdateNow",
                                menuItem_Idx, Properties.Resources.ContextMenu_CheckForUpdateNow,
                                delegate { app.ContextMenu_CheckForUpdateNow(); });


            // --------------------------------
            // ContextMenu_CheckForUpdateAuto
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("ContextMenu_CheckForUpdateAuto",
                                menuItem_Idx, Properties.Resources.ContextMenu_CheckForUpdateAuto,
                                delegate { ContextMenu_CheckForUpdateAuto(); });



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
            // ContextMenu_OpenGraph
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("ContextMenu_OpenGraph",
                                menuItem_Idx, Properties.Resources.ContextMenu_OpenGraph,
                                delegate { ContextMenu_OpenGraph(); });

            // --------------------------------
            // ContextMenu_OpenTable
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("ContextMenu_OpenTable",
                                menuItem_Idx, Properties.Resources.ContextMenu_OpenTable,
                                delegate { ContextMenu_OpenTable(); });

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
                                    "PositionTopLeft.png", delegate { app.Window_Position_TopLeft(this); });


            // --------------------------------
            // Window_Position_TopRight
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_TopRight",
                                    menuItem_Idx, Properties.Resources.Window_Position_TopRight,
                                    "PositionTopRight.png", delegate { app.Window_Position_TopRight(this); });


            // --------------------------------
            // Window_Position_BottomLeft
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_BottomLeft",
                                    menuItem_Idx, Properties.Resources.Window_Position_BottomLeft,
                                    "PositionBottomLeft.png", delegate { app.Window_Position_BottomLeft(this); });

            // --------------------------------
            // Window_Position_BottomRight
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_BottomRight",
                                    menuItem_Idx, Properties.Resources.Window_Position_BottomRight,
                                    "PositionBottomRight.png", delegate { app.Window_Position_BottomRight(this); });


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();

            // --------------------------------
            // ContextMenu_Topmost
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("ContextMenu_Topmost", 
                                menuItem_Idx, Properties.Resources.ContextMenu_Topmost, 
                                delegate { ContextMenu_Topmost(); });


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();


            // --------------------------------
            // ContextMenu_Exit
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("ContextMenu_Exit",
                                    menuItem_Idx, Properties.Resources.ContextMenu_Exit,
                                    "Close_18.png", delegate { this.Close(); });

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
        public void SetCheckedFlag(String key, bool value)
        {
            int id = GetMenuItem_Idx(key);
            MenuItemSetCheckedFlag(id, value);
        }
        
        // ========================================================
        public int GetMenuItem_Idx(String key)
        {
            int value = -1;
            dictContextMenu_resIdentifierToInt.TryGetValue(key, out value);
            return value;
        }

        // ========================================================
        private void MenuItemSetCheckedFlag(int itemIndex, Boolean flag)
        {
            if (itemIndex >= 0)
            {
                ItemCollection ic = this.ContextMenu.Items;
                MenuItem menuItem = (MenuItem)ic.GetItemAt(itemIndex);
                menuItem.IsChecked = flag;
            }
        }


        // ========================================================
        // Menu Clicks
        // ========================================================

        private void ContextMenu_CheckForUpdateAuto()
        {
            Do_Toggle_CheckForUpdateAuto();
        }

        private void Do_Toggle_CheckForUpdateAuto()
        {
            app.nifrekaNetTrafficSettings.CheckForUpdateAuto = !app.nifrekaNetTrafficSettings.CheckForUpdateAuto;

            app.MenuItemSetCheckedFlag("ContextMenu_CheckForUpdateAuto", app.nifrekaNetTrafficSettings.CheckForUpdateAuto);
        }

        private void ContextMenu_OpenGraph()
        {
            Do_Open_WindowLogGraph();
        }

        private void ContextMenu_OpenTable()
        {
            Do_Open_WindowLogTable();
        }

        private void ContextMenu_Topmost()
        {
            Do_Toggle_Topmost();
        }



        // ========================================================
        // Menu Commands
        // ========================================================

        // ========================================================
        private void Do_Toggle_Topmost()
        {
            this.Topmost = !this.Topmost;

            app.MenuItemSetCheckedFlag("ContextMenu_Topmost", this.Topmost);
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
            if (e.Key.Equals(Key.T))
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
    }
}
