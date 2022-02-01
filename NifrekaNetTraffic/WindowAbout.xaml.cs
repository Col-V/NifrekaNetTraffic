using System;
using System.Collections.Generic;
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

using Nifreka;

namespace NifrekaNetTraffic
{
    /// <summary>
    /// Interaction logic for WindowAbout.xaml
    /// </summary>
    // ========================================================
    public partial class WindowAbout : Window
    {
        private readonly DispatcherTimer dispatcherTimer;

        // -----------------------------------------------------
        public WindowAbout()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); // 1 sekunde
            dispatcherTimer.IsEnabled = true;
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        // ========================================================
        public void Window_Loaded(object sender, RoutedEventArgs e)
        // ========================================================
        {
            string versionStr = Const.NifrekaNet_Version;
            string buildStr = Const.NifrekaNet_Build.ToString("D3");
            textBlockVersionBuild.Text = "Version " + versionStr + " - " + "Build "+ buildStr;

            UpdateDisplay();
        }

        // ========================================================
        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        // ========================================================
        {

        }

        // ========================================================
        private void Window_Closed(object sender, EventArgs e)
        // ========================================================
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
            }

        }

        // ========================================================
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();

        }

        // ========================================================
        public void UpdateDisplay()
        {
            DateTime timenow = DateTime.Now;

            double milsec = DateTime.Now.Millisecond;
            double sec = DateTime.Now.Second;
            double min = DateTime.Now.Minute;
            double hr = DateTime.Now.Hour;

            String zeitStr = timenow.Hour.ToString("00:")
                            + timenow.Minute.ToString("00:")
                            + timenow.Second.ToString("00");

            textBoxTime.Text = zeitStr;

            // String datumStr = timenow.ToString("dddd dd.MM. yyyy");

            String datumStr = timenow.ToString("dddd, d. MMMM");
            textBoxDate.Text = datumStr;

        }

        // ========================================================
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ========================================================
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            this.Close();
            e.Handled = true;
        }

        // ========================================================
    }




}
