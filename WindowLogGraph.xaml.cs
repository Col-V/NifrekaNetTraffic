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
        private App app;
        public WindowMain windowMain;

        int pixelWidth;
        int pixelHeight;

        private WriteableBitmap wBitmap;
        private System.Drawing.Graphics graphics;

        // calc max
        long max_Received = 0;
        long max_Sent = 0;

        double faktorReceived = 1;
        double faktorSent = 1;

        int displayStartOffset = 0;
        int displayEndOffset = 0;

        private Dictionary<String, int> dictContextMenu_resIdentifierToInt;

        bool syncLogs = false;
        public bool SyncLogs
        {
            get { return syncLogs; }
            set
            {
                syncLogs = value;
                int id = GetMenuItem_Idx("WindowLog_context_SyncLogs");
                MenuItemSetCheckedFlag(id, syncLogs);
            }
        }

        // ========================
        // ctor
        // ========================
        public WindowLogGraph(WindowMain windowMain)
        {
            this.app = (App)Application.Current;
            this.windowMain = windowMain;

            dictContextMenu_resIdentifierToInt = new Dictionary<string, int>();
            CreateContextmenu();

            InitializeComponent();

            

            this.Left = this.windowMain.nifrekaNetTrafficSettings.Left_WindowLogGraph;
            this.Top = this.windowMain.nifrekaNetTrafficSettings.Top_WindowLogGraph;           
            this.Width = this.windowMain.nifrekaNetTrafficSettings.Width_WindowLogGraph;
            this.Height = this.windowMain.nifrekaNetTrafficSettings.Height_WindowLogGraph;

            this.Loaded += new RoutedEventHandler(this.Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            this.Closed += new EventHandler(this.Window_Closed);


            SyncLogs = this.windowMain.SyncLogs;

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
            loaded = true;

            pixelWidth = (int)logGraphics_Received.ActualWidth;
            pixelHeight = (int)logGraphics_Received.ActualHeight;

            logGraphics_Received.InitGraph(this, LogDataType.Received, System.Drawing.Color.Cyan);
            logGraphics_Sent.InitGraph(this, LogDataType.Sent, System.Drawing.Color.Magenta);

            CalcStartEndOffset();    

        }

        // ========================================================
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        // ========================================================
        {

        }

        // ========================================================
        private void Window_Closed(object sender, EventArgs e)
        // ========================================================
        {
            this.windowMain.nifrekaNetTrafficSettings.Top_WindowLogGraph = this.Top;
            this.windowMain.nifrekaNetTrafficSettings.Left_WindowLogGraph = this.Left;
            this.windowMain.nifrekaNetTrafficSettings.Width_WindowLogGraph = this.ActualWidth;
            this.windowMain.nifrekaNetTrafficSettings.Height_WindowLogGraph = this.ActualHeight;

            if (graphics != null)
            {
                graphics.Dispose();
            }

            this.windowMain.Unregister_WindowLogGraph();

            DoDispose();
        }

        bool isRunning = true;
        // ========================================================
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.P))
            {
                this.isRunning = !this.isRunning;
            }
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
        private void CalcStartEndOffset()
        {
            if (slider_DisplayRange != null)
            {
                displayStartOffset = (int)slider_DisplayRange.Value;

                if (this.windowMain.logList.Count < pixelWidth)
                {
                    displayEndOffset = this.windowMain.logList.Count;
                    slider_DisplayRange.Maximum = this.windowMain.logList.Count;
                }
                else
                {
                    displayEndOffset = displayStartOffset + pixelWidth;
                    slider_DisplayRange.Maximum = this.windowMain.logList.Count - pixelWidth;
                }

                textBlock_DisplayStartOffset.Text = displayStartOffset.ToString("#,##0");
                textBlock_DisplayEndOffset.Text = displayEndOffset.ToString("#,##0");

                int idx_displayStartOffset = this.windowMain.logList.Count - 1 - displayStartOffset;
                if (idx_displayStartOffset >= 0 && idx_displayStartOffset < this.windowMain.logList.Count)
                {
                    LogListItem logListItem_displayStartOffset = this.windowMain.logList.ElementAt(idx_displayStartOffset);
                    textBlock_StartOffsetDateTime.Text = logListItem_displayStartOffset.DataTime_Str;
                }

                int idx_displayEndOffset = this.windowMain.logList.Count - displayEndOffset;
                if (idx_displayEndOffset >= 0 && idx_displayEndOffset < this.windowMain.logList.Count)
                {
                    LogListItem logListItem_displayEndOffset = this.windowMain.logList.ElementAt(idx_displayEndOffset);
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

            for (int i = 0; i < pixelWidth; i++)
            {
                int idx = this.windowMain.logList.Count - 1 - i - displayStartOffset;
                if (idx >= 0)
                {
                    LogListItem logListItem = this.windowMain.logList.ElementAt(idx);

                    long receivedLong = logListItem.BytesReceivedInterval;
                    int receivedInt = (int)receivedLong;
                    max_Received = Math.Max(max_Received, receivedInt);

                    long sentLong = logListItem.BytesSentInterval;
                    int sentInt = (int)sentLong;
                    max_Sent = Math.Max(max_Sent, sentInt);
                }
                else
                {
                    break;
                }

            }

            textBlock_Received_Max.Text = max_Received.ToString("#,##0");
            textBlock_Sent_Max.Text = max_Sent.ToString("#,##0");

            // =================
            long sliderReceivedValue = Convert.ToInt64(slider_ScalerReceived.Value);
            if (sliderReceivedValue == 0)
            {
                sliderReceivedValue = 1;
            }
            textBox_Scale_Received.Text = "x " + sliderReceivedValue.ToString();

            long max_Received_scaled = max_Received / sliderReceivedValue;

            string max_Received_BitStr = NifrekaMathUtil.DecimalBitStr_from_Long(max_Received_scaled);
            textBlock_max_Received_Bit.Text = max_Received_BitStr;


            // =================
            long sliderSentValue = Convert.ToInt64(slider_ScalerSent.Value);
            if (sliderSentValue == 0)
            {
                sliderSentValue = 1;
            }
            textBox_Scale_Sent.Text = "x " + sliderSentValue.ToString();


            long max_Sent_scaled = max_Sent / sliderSentValue;

            string max_Sent_BitStr = NifrekaMathUtil.DecimalBitStr_from_Long(max_Sent_scaled);
            textBlock_max_Sent_Bit.Text = max_Sent_BitStr;

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
                }
                    
            }
            
        }

 

        // ========================================================
        public void UpdateGraphReceived()
        {
            logGraphics_Received.UpdateGraph(faktorReceived, displayStartOffset);
        }

        public void UpdateGraphSent()
        {
            logGraphics_Sent.UpdateGraph(faktorSent, displayStartOffset);
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

                pixelHeight = (int)logGraphics_Received.ActualHeight;
                pixelWidth = (int)logGraphics_Received.ActualWidth;

                logGraphics_Received.InitGraph(this, LogDataType.Received, System.Drawing.Color.Cyan);
                logGraphics_Sent.InitGraph(this, LogDataType.Sent, System.Drawing.Color.Magenta);

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
        // LogGraphics MouseMove
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
            int idx = this.windowMain.logList.Count - 1 - displayStartOffset - (int)mousePoint.X;
            if (idx >= 0 && idx < this.windowMain.logList.Count)
            {
                LogListItem logListItem = this.windowMain.logList.ElementAt(idx);
                this.windowMain.Window_LogTable_ScrollToSelectedItem(logListItem);
            }
        }

        // ========================================================
        private void logGraphics_SetSelectionRange(object sender, Point mousePoint_Selection_Start, Point mousePoint_Selection_End)
        {
            int idx_Start = this.windowMain.logList.Count - 1 - displayStartOffset - (int)mousePoint_Selection_Start.X;
            int idx_End = this.windowMain.logList.Count - 1 - displayStartOffset - (int)mousePoint_Selection_End.X;

            if (idx_Start >= 0 && idx_Start < this.windowMain.logList.Count)
            {
                if (idx_End >= 0 && idx_End < this.windowMain.logList.Count)
                {
                    if (idx_Start == idx_End)
                    {
                        LogListItem logListItem = this.windowMain.logList.ElementAt(idx_Start);
                        this.windowMain.Window_LogTable_ScrollToSelectedItem(logListItem);
                    }
                    else
                    {
                        this.windowMain.Window_LogTable_SetSelectionRange(Math.Min(idx_Start, idx_End),
                                                                      Math.Max(idx_Start, idx_End));
                    }
                    
                }

            }


        }


        // ========================================================
        public void ScrollToSelectedIndex(int idx)
        {
            int sliderDisplayValue = this.windowMain.logList.Count - 1 - (pixelWidth / 2) - idx;
            
            if (sliderDisplayValue < 0)
            {
                sliderDisplayValue = 0;
            }
            slider_DisplayRange.Value = this.windowMain.logList.Count - 1 - (pixelWidth/2) - idx;
        }


        // ========================================================
        // CreateContextmenu
        // ========================================================

        private void CreateContextmenu()
        {
            this.ContextMenu = new ContextMenu();

            // --------------------------------
            // WindowLog_context_SyncLogs
            // --------------------------------
            int menuItem_Idx = 0;
            ContextMenu_Add("WindowLog_context_SyncLogs",
                                menuItem_Idx, Properties.Resources.WindowLog_context_SyncLogs,
                                delegate { WindowLog_context_SyncLogs(); });

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
            this.windowMain.SetSyncLogs(this, syncLogs);

            int id = GetMenuItem_Idx("WindowLog_context_SyncLogs");
            MenuItemSetCheckedFlag(id, syncLogs);
        }

        
        private void Window_Position_TopLeft()
        {
            this.windowMain.MoveWindowsToCorner(Corner.TopLeft);
        }

        private void Window_Position_TopRight()
        {
            this.windowMain.MoveWindowsToCorner(Corner.TopRight);
        }

        private void Window_Position_BottomLeft()
        {
            this.windowMain.MoveWindowsToCorner(Corner.BottomLeft);
        }

        private void Window_Position_BottomRight()
        {
            this.windowMain.MoveWindowsToCorner(Corner.BottomRight);
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










        // ========================================================

    }
}
