// ==============================
// Copyright 2022 nifreka.nl
// ==============================

using Nifreka;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NifrekaNetTraffic
{
    // ###############################################################
    public partial class WindowLogGraph : Window
    {
        public App app;

        private int pixelWidth;
        private int pixelHeight;

        private int plotWidth;
        private int plotHeight;

        public int labelWidth = 100;

        // calc max
        private long max_Received = 0;
        private long max_Sent = 0;

        private long max_Received_scaled;
        private long max_Sent_scaled;

        private double faktorReceived = 1;
        private double faktorSent = 1;

        private int displayStartOffset = 0;
        private int displayEndOffset = 0;

        private Dictionary<String, int> dictContextMenu_resIdentifierToInt;

        private bool syncLogs = false;
        public bool SyncLogs
        {
            get { return syncLogs; }
            set
            {
                syncLogs = value;
                app.MenuItemSetCheckedFlag("WindowLog_context_SyncLogs", syncLogs);
            }
        }

        public bool dataUnit_useBit = true;
        ViewVariant viewVariant;

        // ========================
        // ctor
        // ========================
        public WindowLogGraph()
        {
            this.app = (App)Application.Current;

            dictContextMenu_resIdentifierToInt = new Dictionary<string, int>();
            CreateContextmenu();

            InitializeComponent();

            textBox_Scale_Received.Visibility = Visibility.Hidden;
            textBox_Scale_Sent.Visibility = Visibility.Hidden;

            this.Left = app.nifrekaNetTrafficSettings.Left_WindowLogGraph;
            this.Top = app.nifrekaNetTrafficSettings.Top_WindowLogGraph;           
            this.Width = app.nifrekaNetTrafficSettings.Width_WindowLogGraph;
            this.Height = app.nifrekaNetTrafficSettings.Height_WindowLogGraph;

            
            viewVariant = this.app.nifrekaNetTrafficSettings.ViewVariant;

            this.Loaded += new RoutedEventHandler(this.Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            this.Closed += new EventHandler(this.Window_Closed);


            SyncLogs = app.SyncLogs;

            dataUnit_useBit = true;

        }

        // ========================================================
        public void SetWindowPosAndSize(double left,
                                        double top,
                                        double width,
                                        double height)
        {
            this.Left = left; 
            this.Top = top;            
            this.Width = width;
            this.Height = height;
        }

        


        bool loaded = false;
        // ========================================================
        private void Window_Loaded(object sender, RoutedEventArgs e)
        // ========================================================
        {
            SetViewVariant();

            pixelWidth = (int)logGraphics_Received.ActualWidth;
            pixelHeight = (int)logGraphics_Received.ActualHeight;

            plotWidth = pixelWidth - labelWidth;
            plotHeight = pixelHeight;

            logGraphics_Received.InitGraph(this, DataDirection.Received, System.Drawing.Color.Cyan);
            logGraphics_Sent.InitGraph(this, DataDirection.Sent, System.Drawing.Color.Magenta);

            this.Topmost = app.nifrekaNetTrafficSettings.Topmost_WindowLogGraph;

            UpdateMenuChecks();

            CalcStartEndOffset();

            loaded = true;
            app.DispatcherTimer_Start();
        }


        private void UpdateMenuChecks()
        {
            MenuItemSetCheckedFlag(GetMenuItem_Idx("Topmost_WindowLogGraph"), this.Topmost);
            MenuItemSetCheckedFlag(GetMenuItem_Idx("ContextMenu_CheckForUpdateAuto"), app.nifrekaNetTrafficSettings.CheckForUpdateAuto);

        }

        // ========================================================
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        // ========================================================
        {
            app.nifrekaNetTrafficSettings.Top_WindowLogGraph = this.Top;
            app.nifrekaNetTrafficSettings.Left_WindowLogGraph = this.Left;
            app.nifrekaNetTrafficSettings.Width_WindowLogGraph = this.ActualWidth;
            app.nifrekaNetTrafficSettings.Height_WindowLogGraph = this.ActualHeight;


            if (app.windowLogTable != null)
            {
                app.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable = true;
            }
            else
            {
                app.nifrekaNetTrafficSettings.VisibleAtStart_WindowLogTable = false;
            }

        }

        // ========================================================
        private void Window_Closed(object sender, EventArgs e)
        // ========================================================
        {
            app.DispatcherTimer_Stop();

            DoDispose();
       
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
            app.nifrekaNetTrafficSettings.Top_WindowLogGraph = this.Top;
            app.nifrekaNetTrafficSettings.Left_WindowLogGraph = this.Left;
            app.nifrekaNetTrafficSettings.Topmost_WindowLogGraph = this.Topmost;

            app.nifrekaNetTrafficSettings.WriteSettingsData();
        }


        // ========================================================
        private void CalcStartEndOffset()
        {
            if (slider_DisplayRange != null)
            {
                displayStartOffset = (int)slider_DisplayRange.Value;

                if (app.logList.Count < plotWidth)
                {
                    displayEndOffset = app.logList.Count;
                    slider_DisplayRange.Maximum = app.logList.Count;
                }
                else
                {
                    displayEndOffset = displayStartOffset + plotWidth;
                    slider_DisplayRange.Maximum = app.logList.Count - plotWidth;
                }

                textBlock_DisplayStartOffset.Text = displayStartOffset.ToString("#,##0");
                textBlock_DisplayEndOffset.Text = displayEndOffset.ToString("#,##0");

                int idx_displayStartOffset = app.logList.Count - 1 - displayStartOffset;
                if (idx_displayStartOffset >= 0 && idx_displayStartOffset < app.logList.Count)
                {
                    LogListItem logListItem_displayStartOffset = app.logList.ElementAt(idx_displayStartOffset);
                    textBlock_StartOffsetDateTime.Text = logListItem_displayStartOffset.DataTime_Str;
                }

                int idx_displayEndOffset = app.logList.Count - displayEndOffset;
                if (idx_displayEndOffset >= 0 && idx_displayEndOffset < app.logList.Count)
                {
                    LogListItem logListItem_displayEndOffset = app.logList.ElementAt(idx_displayEndOffset);
                    textBlock_EndOffsetDateTime.Text = logListItem_displayEndOffset.DataTime_Str;  
                }

            }

        }

        

        // ========================================================
        private void CalcDisplayRangeMaxValues()
        {
            // calc max
            max_Received = 0;
            max_Sent = 0;

            CalcStartEndOffset();

            for (int i = 0; i < plotWidth; i++)
            {
                int idx = app.logList.Count - 1 - i - displayStartOffset;
                if (idx > 0)
                {
                    LogListItem logListItem = app.logList.ElementAt(idx);
                    LogListItem logListItem_previous = app.logList.ElementAt(idx - 1);     

                    max_Received = Math.Max(max_Received, LogList.CalcBytesPerSecond(logListItem, logListItem_previous, DataDirection.Received));
                    max_Sent = Math.Max(max_Sent, LogList.CalcBytesPerSecond(logListItem, logListItem_previous, DataDirection.Sent));
                }
                else
                {
                    break;
                }

            }

            if (this.dataUnit_useBit == true)
            {
                textBlock_max_Received_MaxXB.Text = NifrekaConversionUtil.Bit1000Str_from_LongBytes(max_Received);
                textBlock_max_Sent_MaxXB.Text = NifrekaConversionUtil.Bit1000Str_from_LongBytes(max_Sent);

                textBlock_Received_Max.Text = (max_Received * 8).ToString("#,##0");
                textBlock_Sent_Max.Text = (max_Sent * 8).ToString("#,##0");

                button_DataUnit_Received.Content = "Bit";
                button_DataUnit_Sent.Content = "Bit";
            }
            else
            {
                textBlock_max_Received_MaxXB.Text = NifrekaConversionUtil.Bytes1000Str_from_LongBytes(max_Received);
                textBlock_max_Sent_MaxXB.Text = NifrekaConversionUtil.Bytes1000Str_from_LongBytes(max_Sent);

                textBlock_Received_Max.Text = max_Received.ToString("#,##0");
                textBlock_Sent_Max.Text = max_Sent.ToString("#,##0");

                button_DataUnit_Received.Content = "B";
                button_DataUnit_Sent.Content = "B";
            }

            // =================
            long sliderReceivedValue = Convert.ToInt64(slider_ScalerReceived.Value);
            if (sliderReceivedValue == 0)
            {
                sliderReceivedValue = 1;
            }
            textBox_Scale_Received.Text = "x " + sliderReceivedValue.ToString();

            max_Received_scaled = max_Received / sliderReceivedValue;

            // =================
            long sliderSentValue = Convert.ToInt64(slider_ScalerSent.Value);
            if (sliderSentValue == 0)
            {
                sliderSentValue = 1;
            }
            textBox_Scale_Sent.Text = "x " + sliderSentValue.ToString();

            max_Sent_scaled = max_Sent / sliderSentValue;


            if (max_Received == 0)
            {
                faktorReceived = 0;
            }
            else
            {
                faktorReceived = (double)pixelHeight / max_Received * slider_ScalerReceived.Value;
            }

            if (max_Sent == 0)
            {
                faktorSent = 0;
            }
            else
            {
                faktorSent = (double)pixelHeight / max_Sent * slider_ScalerSent.Value;
            }

        }

        // ========================================================
        public void UpdateGraph()
        {
            if (loaded == true)
            {
                if (isRunning == true)
                {
                    CalcDisplayRangeMaxValues();

                    if (faktorReceived > 0)
                    {
                        UpdateGraphReceived();
                    }
                    if (faktorSent > 0)
                    {
                       UpdateGraphSent();
                    }

                    UpdateCompact();
                }
                    
            }
            
        }

        // ========================================================
        private void UpdateCompact()
        {
            if (app.logList.Count > 0)
            {
                int idx = app.logList.Count - 1;
                LogListItem recentLogListItem = app.logList.ElementAt(idx);

                textBlock_Bytes_Received.Text = recentLogListItem.BytesReceived.ToString("#,##0") + "  B";
                textBlock_Bytes_Sent.Text = recentLogListItem.BytesSent.ToString("#,##0") + "  B";

                long bytesReceived_Interval = recentLogListItem.BytesReceivedInterval;
                long bytesSent_Interval = recentLogListItem.BytesSentInterval;

                textBlock_Bytes_Received_Interval.Text = bytesReceived_Interval.ToString("#,##0") + "  B";
                textBlock_Bytes_Sent_Interval.Text = bytesSent_Interval.ToString("#,##0") + "  B";

            }
        }

        // ========================================================
        public void UpdateGraphReceived()
        {
            logGraphics_Received.UpdateGraph(faktorReceived, displayStartOffset, max_Received, max_Received_scaled);
        }

        public void UpdateGraphSent()
        {
            logGraphics_Sent.UpdateGraph(faktorSent, displayStartOffset, max_Sent, max_Sent_scaled);
        }

        // ========================================================
        private void DoDispose()
        {
            logGraphics_Received.Dispose();
            logGraphics_Sent.Dispose();
        }

        // ========================================================
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isResizing == false)
            {
                // DoResize();
            }
        }

        private void grid_ViewGraph_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isResizing == false)
            {
                DoResize();
            }
        }

        // ========================================================
        bool isResizing = false;  
        private void DoResize()
        {
            if (loaded == true)
            {
                isResizing = true;

                logGraphics_Received.Dispose();
                logGraphics_Sent.Dispose();

                // wait for re-layout of logGraphics_Received
                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.Render, null);

                pixelWidth = (int)logGraphics_Received.ActualWidth;
                pixelHeight = (int)logGraphics_Received.ActualHeight;
                
                plotWidth = pixelWidth - labelWidth;
                plotHeight = pixelHeight;

                logGraphics_Received.InitGraph(this, DataDirection.Received, System.Drawing.Color.Cyan);
                logGraphics_Sent.InitGraph(this, DataDirection.Sent, System.Drawing.Color.Magenta);

                CalcStartEndOffset();
                
                isResizing = false;
            }
        }

        // ========================================================
        // slider_DisplayRange
        // ========================================================

        private void slider_DisplayRange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {           
            CalcStartEndOffset();
            UpdateGraph();
        }

       
        // ========================================================
        // slider_Scaler
        // ========================================================
        private void slider_ScalerReceived_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateGraph();
        }

        private void slider_ScalerSent_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateGraph();
        }

        // ========================================================
        // LogGraphics MouseEvents
        // ========================================================
        private void logGraphics_Received_MouseMove(object sender, MouseEventArgs e)
        {
            if (syncLogs)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point mousePoint = e.GetPosition(logGraphics_Received);
                    logGraphics_SetSingleSelection(sender, mousePoint);
                }

            }
        }

        private void logGraphics_Sent_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePoint = e.GetPosition(logGraphics_Sent);
                logGraphics_SetSingleSelection(sender, mousePoint);
            }

        }

        // ================================
        private void logGraphics_MouseMove(object sender, MouseEventArgs e, Point mousePoint)
        {

        }


        private void logGraphics_Received_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void logGraphics_Received_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (syncLogs)
            {
                Point mousePoint = e.GetPosition(logGraphics_Received);
                logGraphics_SetSingleSelection(sender, mousePoint);
            }
                
        }

        private void logGraphics_Sent_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (syncLogs)
            {
                Point mousePoint = e.GetPosition(logGraphics_Sent);
                logGraphics_SetSingleSelection(sender, mousePoint);
            }
                
        }
        
        private void logGraphics_SetSingleSelection(object sender, Point mousePoint)
        {
            int idx = app.logList.Count - 1 - displayStartOffset - (int)mousePoint.X;
            if (idx >= 0 && idx < app.logList.Count)
            {
                LogListItem logListItem = app.logList.ElementAt(idx);
                app.Window_LogTable_ScrollToSelectedItem(logListItem);
            }
        }

        // ========================================================
        public void ScrollToSelectedIndex(int idx)
        {
            int sliderDisplayValue = app.logList.Count - 1 - (plotWidth / 2) - idx;
            
            if (sliderDisplayValue < 0)
            {
                sliderDisplayValue = 0;
            }
            slider_DisplayRange.Value = app.logList.Count - 1 - (plotWidth / 2) - idx;
        }


        // ========================================================
        // CreateContextmenu
        // ========================================================

        private void CreateContextmenu()
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


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();


            // --------------------------------
            // WindowLog_context_SyncLogs
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("WindowLog_context_SyncLogs",
                                menuItem_Idx, Properties.Resources.WindowLog_context_SyncLogs,
                                delegate { WindowLog_context_SyncLogs(); });


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();



            // --------------------------------
            // WindowLogGraph_View_Graph
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("WindowLogGraph_View_Graph",
                                menuItem_Idx, Properties.Resources.WindowLogGraph_View_Graph,
                                delegate { WindowLogGraph_View_Graph(); });

            // --------------------------------
            // WindowLogGraph_View_Small
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("WindowLogGraph_View_Small",
                                menuItem_Idx, Properties.Resources.WindowLogGraph_View_Small,
                                delegate { WindowLogGraph_View_Small(); });

            // --------------------------------
            // WindowLogGraph_View_Both
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("WindowLogGraph_View_Both",
                                menuItem_Idx, Properties.Resources.WindowLogGraph_View_Both,
                                delegate { WindowLogGraph_View_Both(); });

            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();


            // --------------------------------
            // ContextMenu_OpenTable
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("ContextMenu_OpenTable",
                                menuItem_Idx, Properties.Resources.ContextMenu_OpenTable,
                                delegate { app.Do_Open_WindowLogTable(); });



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
                                    "PositionTopLeft.png", delegate { app.Window_Position_TopLeft(); });


            // --------------------------------
            // Window_Position_TopRight
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_TopRight",
                                    menuItem_Idx, Properties.Resources.Window_Position_TopRight,
                                    "PositionTopRight.png", delegate { app.Window_Position_TopRight(); });


            // --------------------------------
            // Window_Position_BottomLeft
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_BottomLeft",
                                    menuItem_Idx, Properties.Resources.Window_Position_BottomLeft,
                                    "PositionBottomLeft.png", delegate { app.Window_Position_BottomLeft(); });

            // --------------------------------
            // Window_Position_BottomRight
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("Window_Position_BottomRight",
                                    menuItem_Idx, Properties.Resources.Window_Position_BottomRight,
                                    "PositionBottomRight.png", delegate { app.Window_Position_BottomRight(); });

            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();


            // --------------------------------
            // ContextMenu_Topmost
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("Topmost_WindowLogGraph",
                                menuItem_Idx, Properties.Resources.ContextMenu_Topmost,
                                delegate { ContextMenu_Topmost(); });


            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();

            // --------------------------------
            // contextmenu_exit
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add_WithIcon("ContextMenu_Exit",
                                    menuItem_Idx, Properties.Resources.ContextMenu_Exit,
                                    "Close_18.png", delegate { app.Do_Exit(); });


        }

        // ========================================================
        // Menu Clicks
        // ========================================================
        private void WindowLog_context_SyncLogs()
        {
            Do_Toggle_Sync();
        }
        
        private void Do_Toggle_Sync()
        {
            syncLogs = !syncLogs;
            app.SetSyncLogs(this, syncLogs);

            app.MenuItemSetCheckedFlag("WindowLog_context_SyncLogs", syncLogs);
        }

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

        private void ContextMenu_Topmost()
        {
            Do_Toggle_Topmost();
        }

        private void Do_Toggle_Topmost()
        {
            this.Topmost = !this.Topmost;
            app.MenuItemSetCheckedFlag("Topmost_WindowLogGraph", this.Topmost);
        }


        // ========================================================
        // ========================================================
        private void WindowLogGraph_View_Graph()
        {
            this.viewVariant = ViewVariant.Graph;
            SetViewVariant();
        }

        private void WindowLogGraph_View_Small()
        {
            this.viewVariant = ViewVariant.Small;
            SetViewVariant();
        }

        private void WindowLogGraph_View_Both()
        {
            this.viewVariant = ViewVariant.Both;
            SetViewVariant();
        }


        // ========================================================
        private void ClearVariantMenuChecks()
        {
            MenuItemSetCheckedFlag(GetMenuItem_Idx("WindowLogGraph_View_Graph"), false);
            MenuItemSetCheckedFlag(GetMenuItem_Idx("WindowLogGraph_View_Small"), false);
            MenuItemSetCheckedFlag(GetMenuItem_Idx("WindowLogGraph_View_Both"), false);
        }

        // ========================================================
        public void SetViewVariant()
        {
            ClearVariantMenuChecks();

            switch (this.viewVariant)
            {
                case ViewVariant.Graph:
                    {
                        Do_WindowLogGraph_View_Graph();
                        MenuItemSetCheckedFlag(GetMenuItem_Idx("WindowLogGraph_View_Graph"), true);
                        this.Title = Properties.Resources.Window_Title_Graph;
                        break;
                    }
                case ViewVariant.Small:
                    {
                        Do_WindowLogGraph_View_Small();
                        MenuItemSetCheckedFlag(GetMenuItem_Idx("WindowLogGraph_View_Small"), true);
                        this.Title = Properties.Resources.Window_Title_Small;
                        break;
                    }
                case ViewVariant.Both:
                    {
                        Do_WindowLogGraph_View_Both();
                        MenuItemSetCheckedFlag(GetMenuItem_Idx("WindowLogGraph_View_Both"), true);
                        this.Title = Properties.Resources.Window_Title_GraphAndSmall;
                        break;
                    }
            }
        }

        // ========================================================
        // ========================================================
        private void Do_WindowLogGraph_View_Graph()
        {
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.CanResize;
            this.MinHeight = 230;

            this.SizeToContent = SizeToContent.Manual;

            if (this.ActualHeight < MinHeight)
            {
                this.Height = MinHeight;
            }

            grid_ViewGraph.Visibility = Visibility.Visible;
            rowDefinition_Graph.Height = new GridLength(1, GridUnitType.Star);
            border_ViewSmall.Visibility = Visibility.Collapsed;

            this.viewVariant = ViewVariant.Graph;
            app.nifrekaNetTrafficSettings.ViewVariant = ViewVariant.Graph;
        }

        // ========================================================
        private void Do_WindowLogGraph_View_Small()
        {
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;

            this.MinHeight = 0;

            this.SizeToContent = SizeToContent.Height;

            this.Width = app.nifrekaNetTrafficSettings.WindowLogGraph_DefaultWidth;
            border_ViewSmall.Visibility = Visibility.Visible;
            grid_ViewGraph.Visibility = Visibility.Collapsed;
            rowDefinition_Graph.Height = new GridLength(0, GridUnitType.Star);

            this.viewVariant = ViewVariant.Small;
            app.nifrekaNetTrafficSettings.ViewVariant = ViewVariant.Small;
        }

        // ========================================================
        public void Do_WindowLogGraph_View_Both()
        {
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.CanResize;
            this.MinHeight = 250;

            this.SizeToContent = SizeToContent.Manual;

            if (this.ActualHeight < MinHeight)
            {
                this.Height = MinHeight;
            }

            border_ViewSmall.Visibility = Visibility.Visible;

            grid_ViewGraph.Visibility = Visibility.Visible;
            rowDefinition_Graph.Height = new GridLength(1, GridUnitType.Star);

            this.viewVariant = ViewVariant.Both;
            app.nifrekaNetTrafficSettings.ViewVariant = ViewVariant.Both;

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
            catch (Exception)
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
        public int GetMenuItem_Idx(String key)
        {
            int value = -1;
            dictContextMenu_resIdentifierToInt.TryGetValue(key, out value);
            return value;
        }

        // ========================================================
        // slider_Scaler
        // ========================================================
        private void slider_ScalerReceived_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            textBox_Scale_Received.Visibility = Visibility.Visible;
        }

        private void slider_ScalerReceived_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            textBox_Scale_Received.Visibility = Visibility.Hidden;
        }

        private void slider_ScalerSent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            textBox_Scale_Sent.Visibility = Visibility.Visible;
        }

        private void slider_ScalerSent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            textBox_Scale_Sent.Visibility = Visibility.Hidden;
        }


        // ========================================================
        // Button Clicks
        // ========================================================
        private void button_DataUnit_Received_Click(object sender, RoutedEventArgs e)
        {
            this.dataUnit_useBit = !this.dataUnit_useBit;
            UpdateGraph();
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            slider_DisplayRange.Value = 0;
        }

        private void button_End_Click(object sender, RoutedEventArgs e)
        {
            slider_DisplayRange.Value = slider_DisplayRange.Maximum;
        }

        // ========================================================
        // KeyEvents
        // ========================================================
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                slider_DisplayRange.Value = Math.Max(0, slider_DisplayRange.Value - plotWidth / 20);
            }

            if (e.Key == Key.Right)
            {
                slider_DisplayRange.Value = Math.Min(slider_DisplayRange.Maximum, slider_DisplayRange.Value + plotWidth / 20);
            }

            e.Handled = true;
        }


        bool isRunning = true;
        // ========================================================
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ModifierKeys modifierKeysShiftControl = ModifierKeys.Shift | ModifierKeys.Control;

            if (Keyboard.Modifiers == modifierKeysShiftControl && e.Key.Equals(Key.P))
            {
                this.isRunning = !this.isRunning;
            }

            if (e.Key.Equals(Key.Home))
            {
                slider_DisplayRange.Value = 0;
            }

            if (e.Key.Equals(Key.End))
            {
                slider_DisplayRange.Value = slider_DisplayRange.Maximum;
            }

            e.Handled = true;
        }

        // ========================================================
        public void DoPause()
        {
            this.isRunning = false;
        }

        public void DoContinue()
        {
            this.isRunning = true;
        }


        // ========================================================
 

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
            WindowLogGraph_View_Both();
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
            app.Do_Open_WindowLogTable();
            border_Table.BorderBrush = Brushes.White;
        }


        // ========================================================
        private void border_ViewSmall_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                    // isMouseButtonState_Pressed = true;
                    this.Cursor = Cursors.SizeAll;
                    this.DragMove();
                }

            }
        }

        // ========================================================
        private void border_ViewSmall_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseOverImageGraph || isMouseOverImageTable)
            {
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                this.Cursor = Cursors.Hand;
            }
        }

        private void border_ViewSmall_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        // ========================================================










        // ========================================================

    }
}
