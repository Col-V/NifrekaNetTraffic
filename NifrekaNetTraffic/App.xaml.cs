using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NifrekaNetTraffic
{
    // #######################################################################
    public partial class App : Application
    {
        public WindowMain windowMain;

        // ========================
        // ctor
        // ========================
        App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture;

            // zum Test
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en"); 
        }


        // ========================================================
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            OpenWindowMain();
        }

        // ========================================================
        private void OpenWindowMain()
        {
            windowMain = new WindowMain();
            windowMain.Show();
        }



        // ========================================================

    }
}
