// ==============================
// Copyright 2022 nifreka.nl
// ==============================

using Nifreka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NifrekaNetTraffic
{
    // ###############################################################
    public partial class WindowLogTable : Window
    {
        public App app;

        private Dictionary<String, int> dictContextMenu_resIdentifierToInt;

        // ===============
        private bool syncLogs;
        public bool SyncLogs
        {
            get { return syncLogs; }
            set
            {
                syncLogs = value;
                checkBox_AutoScroll.IsChecked = !syncLogs;
                app.MenuItemSetCheckedFlag("WindowLog_context_SyncLogs", syncLogs);
            }
        }

        // ========================
        // ctor
        // ========================
        public WindowLogTable()
        {
            this.app = (App)Application.Current;

            dictContextMenu_resIdentifierToInt = new Dictionary<string, int>();
            CreateContextmenu();

            InitializeComponent();

            this.Title = Properties.Resources.Window_Title_LogTable;

            SetVisibile_Received_Sent_Columns(false);

            textBlock_BytesReceivedSelection.Text = "";
            textBlock_BytesSentSelection.Text = "";

            this.Left = app.nifrekaNetTrafficSettings.Left_WindowLogTable;
            this.Top = app.nifrekaNetTrafficSettings.Top_WindowLogTable;                    
            this.Width = app.nifrekaNetTrafficSettings.Width_WindowLogTable;
            this.Height = app.nifrekaNetTrafficSettings.Height_WindowLogTable;

            this.Loaded += new RoutedEventHandler(this.Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            this.Closed += new EventHandler(this.Window_Closed);

            SetBindings();
            ListViewLog_ScrollToEnd();

            SyncLogs = app.SyncLogs;

            Update_comboBox_NetAdapter();
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

        // ========================================================
        private void Window_Loaded(object sender, RoutedEventArgs e)
        // ========================================================
        {
            MenuItemSetCheckedFlag(GetMenuItem_Idx("ContextMenu_CheckForUpdateAuto"), app.nifrekaNetTrafficSettings.CheckForUpdateAuto);
        }

        // ========================================================
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        // ========================================================
        {
            app.nifrekaNetTrafficSettings.Top_WindowLogTable = this.Top;
            app.nifrekaNetTrafficSettings.Left_WindowLogTable = this.Left;
            app.nifrekaNetTrafficSettings.Width_WindowLogTable = this.ActualWidth;
            app.nifrekaNetTrafficSettings.Height_WindowLogTable = this.ActualHeight;
        }

        // ========================================================
        private void Window_Closed(object sender, EventArgs e)
        // ========================================================
        {
            app.UnregisterWindow(this);
        }

        // =========================================
        public void SetBindings()
        // =========================================
        {
            ListViewLog_SetBinding();
        }

        // =========================================
        public void ListViewLog_SetBinding()
        // =========================================
        {
            CollectionViewSource source = new CollectionViewSource();
            source.Source = app.logList;
            Binding binding = new Binding();
            binding.Source = source;
            BindingOperations.SetBinding(listViewLog, ListView.ItemsSourceProperty, binding);
        }

        // =========================================
        public void ListViewLog_ScrollToEnd()
        // =========================================
        {
            if (checkBox_AutoScroll.IsChecked == true)
            {
                int count = listViewLog.Items.Count;
                if (count > 0)
                {
                    listViewLog.ScrollIntoView(listViewLog.Items[count - 1]);

                    // added in build 40
                    listViewLog.SelectedIndex = count - 1;
                }
            }          
        }

        // =========================================
        public void ListViewLog_ScrollToSelectedItem(LogListItem logListItem)
        // =========================================
        {
            if (checkBox_AutoScroll.IsChecked == false)
            {
                listViewLog.SelectedItem = logListItem;
                listViewLog.ScrollIntoView(logListItem);
            }

        }

        // =========================================
        public void ListViewLog_SetSelectionRange(int idx_Start, int idx_End)
        // =========================================
        {
            if (checkBox_AutoScroll.IsChecked == false)
            {
                listViewLog.SelectedItems.Clear();

                for (int i = idx_Start; i < idx_End; i++)
                {
                    LogListItem logListItem = (LogListItem)listViewLog.Items[i];
                    listViewLog.SelectedItems.Add(logListItem);
                    
                }

                LogListItem logListItem_ScrollIntoView = (LogListItem)listViewLog.Items[idx_Start + (idx_End - idx_Start) / 2];
                listViewLog.ScrollIntoView(logListItem_ScrollIntoView);
            }

        }

        // =========================================
        public void ListViewLog_Update()
        // =========================================
        {
            textBlock_Count.Text = app.logList.Count.ToString("#,##0");

            textBlock_BytesReceivedTotal.Text = app.logList.BytesReceivedTotal.ToString("#,##0");
            textBlock_BytesSentTotal.Text = app.logList.BytesSentTotal.ToString("#,##0");

            textBlock_BytesReceivedLog.Text = app.logList.BytesReceivedLog.ToString("#,##0");
            textBlock_BytesSentLog.Text = app.logList.BytesSentLog.ToString("#,##0");

            ResizeGridViewColumn(gridViewColumn_DateTimeTicks);
            ResizeGridViewColumn(gridViewColumn_BytesReceivedInterval);
            ResizeGridViewColumn(gridViewColumn_BytesSentInterval);
        }

        // ========================================================
        private void ResizeGridViewColumn(GridViewColumn column)
        {
            if (double.IsNaN(column.Width))
            {
                column.Width = column.ActualWidth;
            }

            column.Width = double.NaN;
        }

        // ========================================================
        public void Update_comboBox_NetAdapter()
        {
            comboBox_NetAdapter.Items.Clear();
            comboBox_NetAdapter.SelectedIndex = -1;

            for (int i = 0; i < app.netAdapterList.Count(); i++)
            {
                NetAdapter netAdapter = app.netAdapterList[i];
                comboBox_NetAdapter.Items.Add(netAdapter.networkInterface.Name);
            }

            if (comboBox_NetAdapter.Items.Count > 0)
            {
                comboBox_NetAdapter.SelectedIndex = 0;
            }
        }


        // ========================================================
        private void ComboBox_NetAdapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_NetAdapter.SelectedIndex >= 0)
            {
                app.selectedNetAdapter = app.netAdapterList[comboBox_NetAdapter.SelectedIndex];
            }

        }

        // ========================================================
        private void button_ClearLog_Click(object sender, RoutedEventArgs e)
        {
            Dialog_Question_ClearLog();            
        }

        // ========================================================
        private void Dialog_Question_ClearLog()
        {
            string question = NifrekaNetTraffic.Properties.Resources.Dialog_Question_ClearLog;

            DialogYesNo dialogYesNo = new DialogYesNo(question);
            bool? result = dialogYesNo.ShowDialog();
            if (result == true)
            {
                Yes_ClearLog();

            }
        }

        // ========================================================
        private void Yes_ClearLog()
        {
            app.ClearLog();
        }

        // ========================================================
        private void button_ExportAsText_Click(object sender, RoutedEventArgs e)
        {
            Do_WindowLogTable_ExportAsText();
        }

        private void Do_WindowLogTable_ExportAsText()
        {
            app.ExportAsText(this.Left, this.Top);
        }

        // ========================================================
        private void button_OpenLogDir_Click(object sender, RoutedEventArgs e)
        {
            this.Do_OpenLogDir();
        }

        public void Do_OpenLogDir()
        {
            try
            {
                Directory.CreateDirectory(Const.NifrekaNetTraffic_Data_folderpath);

                // System.Diagnostics.Process.Start("explorer.exe", Const.NifrekaNetTraffic_Data_folderpath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = Const.NifrekaNetTraffic_Data_folderpath,
                    UseShellExecute = true,
                    Verb = "open"
                });


            }
            catch (Exception)
            {
                //  MessageBox.Show(ex.Message);
            }
        }

        // ========================================================
        private void listViewLog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int idx = listViewLog.SelectedIndex;

            if (syncLogs)
            {
                app.Window_LogGraph_ScrollToSelectedIndex(idx);
            }
        }

        // ========================================================
        private void checkBox_AutoScroll_Checked(object sender, RoutedEventArgs e)
        {
            if (checkBox_AutoScroll.IsChecked == true)
            {
                this.syncLogs = false;

                app.MenuItemSetCheckedFlag("WindowLog_context_SyncLogs", syncLogs);

                app.SetSyncLogs(this, syncLogs);
            }
            
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
            // wind_table_ClearLog
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("WindowLogTable_ClearLog",
                                menuItem_Idx, Properties.Resources.WindowLogTable_ClearLog,
                                delegate { WindowLogTable_ClearLog(); });




            // --------------------------------
            // WindowLogTable_context_OpenLogDir
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("ContextMenu_OpenLogDir",
                                menuItem_Idx, Properties.Resources.ContextMenu_OpenLogDir,
                                delegate { ContextMenu_OpenLogDir(); });



            // --------------------------------
            // Separator
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            Contextmenu_addSeparator();

            // --------------------------------
            // WindowLogTable_ExportAsText
            // --------------------------------
            menuItem_Idx = menuItem_Idx + 1;
            ContextMenu_Add("WindowLogTable_ExportAsText",
                                menuItem_Idx, Properties.Resources.WindowLogTable_ExportAsText,
                                delegate { WindowLogTable_ExportAsText(); });



      
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
            // ContextMenu_Exit
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

            checkBox_AutoScroll.IsChecked = !syncLogs;
            app.SetSyncLogs(this, syncLogs);

            app.MenuItemSetCheckedFlag("WindowLog_context_SyncLogs", syncLogs);
        }

        private void checkBox_AutoScroll_ON_OFF(bool flag)
        {
            checkBox_AutoScroll.IsChecked = flag;
            app.MenuItemSetCheckedFlag("WindowLog_context_SyncLogs", flag);
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

        // ========================================================
        private void WindowLogTable_ClearLog()
        {
            Dialog_Question_ClearLog();
        }

        // ========================================================
        private void ContextMenu_OpenLogDir()
        {
            Do_OpenLogDir();
        }

        private void WindowLogTable_ExportAsText()
        {
            Do_WindowLogTable_ExportAsText();
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
        public int GetMenuItem_Idx(String key)
        {
            int value = -1;
            dictContextMenu_resIdentifierToInt.TryGetValue(key, out value);
            return value;
        }

        // ========================================================
        public void MenuItemSetCheckedFlag(int itemIndex, Boolean flag)
        {
            if (itemIndex >= 0)
            {
                ItemCollection ic = this.ContextMenu.Items;
                MenuItem menuItem = (MenuItem)ic.GetItemAt(itemIndex);
                menuItem.IsChecked = flag;
            }
            
        }

        // ========================================================
        private void listViewLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSumSelectionInHeader();
        }

        private void UpdateSumSelectionInHeader()
        {
            long sum_Sel_Received = 0;
            long sum_Sel_Sent = 0;

            var selList = listViewLog.SelectedItems;
            if (selList != null)
            {
                for (int i = 0; i < selList.Count; i++)
                {
                    var item = (LogListItem)selList[i];
                    sum_Sel_Received = sum_Sel_Received + item.BytesReceivedInterval;
                    sum_Sel_Sent = sum_Sel_Sent + item.BytesSentInterval;
                }

                textBlock_BytesReceivedSelection.Text = sum_Sel_Received.ToString("#,##0");
                textBlock_BytesSentSelection.Text = sum_Sel_Sent.ToString("#,##0");
            }
            else
            {
                textBlock_BytesReceivedSelection.Text = "";
                textBlock_BytesSentSelection.Text = "";
            }

        }


        bool isVisibile_Received_Sent_Columns = false;
        // ========================================================
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ModifierKeys modifierKeysShiftControl = ModifierKeys.Shift | ModifierKeys.Control;  

            if (Keyboard.Modifiers == modifierKeysShiftControl && e.Key.Equals(Key.T))
            {
                isVisibile_Received_Sent_Columns = !isVisibile_Received_Sent_Columns;
                SetVisibile_Received_Sent_Columns(isVisibile_Received_Sent_Columns);
            }
        }

        private void SetVisibile_Received_Sent_Columns(bool isVisible)
        {
            isVisibile_Received_Sent_Columns = isVisible;

            if (isVisibile_Received_Sent_Columns == true)
            {
                gridViewColumn_BytesReceived.Width = 100;
                gvch_BytesReceived.IsEnabled = true;

                gridViewColumn_BytesSent.Width = 100;
                gvch_BytesSent.IsEnabled = true;

            }
            else
            {
                gridViewColumn_BytesReceived.Width = 0;
                gvch_BytesReceived.IsEnabled = false;

                gridViewColumn_BytesSent.Width = 0;
                gvch_BytesSent.IsEnabled = false;
            }
        }


        // ========================================================
        void ScrollBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            checkBox_AutoScroll_ON_OFF(false);

        }

        // ========================================================
    }
}
