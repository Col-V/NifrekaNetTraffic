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

    // ###############################################################
    public partial class DialogNewVersionAvailable : Window
    {

        // -----------------------------------------------------
        public DialogNewVersionAvailable(
            string serverVersionStr,
            string installedVersionStr)
        {
            InitializeComponent();

            textBox_ServerVersion.Text = serverVersionStr;
            textBox_InstalledVersion.Text = installedVersionStr;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            this.Closed += new EventHandler(this.Window_Closed);

        }

        // ========================================================
        public void Window_Loaded(object sender, RoutedEventArgs e)
        // ========================================================
        {

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

        }

        // ========================================================
        private void button_Yes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        // ========================================================
    }




}
