// ==============================
// Copyright 2022 nifreka.nl
// ==============================

using Nifreka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NifrekaNetTraffic
{
    public class UpdateChecker
    {
        private App app;
        private bool notifyOnlyNewVersion;
        private WebClient wc;

        // ========================
        // ctor
        // ========================
        public UpdateChecker()
        {
            this.app = (App)Application.Current;
            this.notifyOnlyNewVersion = true;
        }

        // ========================================================
        public void Dispose()
        {
            if (wc != null)
            {
                if (wc.IsBusy)
                {
                    wc.CancelAsync();
                }
                wc.Dispose();
            }
        }

        // ========================================================
        public void CheckForUpdate(bool notifyOnlyNewVersion)
        {
            this.notifyOnlyNewVersion = notifyOnlyNewVersion;

            if (wc != null)
            {
                if (wc.IsBusy)
                {
                    wc.CancelAsync();
                }
                wc.Dispose();
            }

            String urlStr = "https://nifreka.nl/nnt/nftVersion.txt";

            Uri uri = new Uri(urlStr);

            wc = new WebClient();
            wc.DownloadDataCompleted += DownloadDataCompleted;
            wc.DownloadDataAsync(uri);
        }

        // ----------------------------------------------------------------
        delegate void DownloadDataCompleted_Delegate(Object sender, DownloadDataCompletedEventArgs e);
        private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (app.Dispatcher.CheckAccess())
            {
                if (e.Error != null)
                {
                    // DownloadDataCompleted_Error();
                }
                else
                {
                    DownloadDataCompleted_OK_handle_it(sender, e);
                }
            }
            else
            {
                app.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new DownloadDataCompleted_Delegate(DownloadDataCompleted), sender, e);
            }
        }

        // ========================================================
        private void DownloadDataCompleted_OK_handle_it(Object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                byte[] receivedData = e.Result;
                string receivedDataStr = Encoding.UTF8.GetString(receivedData);
                string[] lines = receivedDataStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                int lines_count = lines.Count();

                if (lines_count == 3)
                {
                    string[] majorVersionLineArr = GetLinePieces(lines[0]);
                    string[] subVersionLineArr = GetLinePieces(lines[1]);
                    string[] buildNumberLineArr = GetLinePieces(lines[2]);

                    if (majorVersionLineArr.Count() == 2
                        &&
                        subVersionLineArr.Count() == 2
                        &&
                        buildNumberLineArr.Count() == 2
                        )
                    {
                        string majorVersion_Server_Label = majorVersionLineArr[0];
                        string subVersion_Server_Label = subVersionLineArr[0];
                        string buildNumber_Server_Label = buildNumberLineArr[0];

                        int majorVersion_Server_Value = NifrekaConversionUtil.StrToInt(majorVersionLineArr[1]);
                        int subVersion_Server_Value = NifrekaConversionUtil.StrToInt(subVersionLineArr[1]);
                        int buildNumber_Server_Value = NifrekaConversionUtil.StrToInt(buildNumberLineArr[1]);

                        int buildNumber_Installed = Const.NifrekaNet_Build;

                        // currently only buildNumber is of interest
                        //
                        if (buildNumber_Server_Label.Equals("buildNumber"))
                        {
                            if (buildNumber_Server_Value > buildNumber_Installed)
                            {
                                Show_DialogNewVersionAvailable(buildNumber_Server_Value.ToString(), buildNumber_Installed.ToString());
                            }

                            if (buildNumber_Server_Value <= buildNumber_Installed)
                            {
                                if (notifyOnlyNewVersion == false)
                                {
                                    Show_DialogVersionUpToDate(buildNumber_Server_Value.ToString(), buildNumber_Installed.ToString());
                                }

                            }
                        }
                        
                    }

                }

            }
            catch (Exception)
            {
                // throw;
            }
        }

        // ========================================================
        private void Show_DialogNewVersionAvailable(
            string serverVersionStr,
            string installedVersionStr)
        {
            try
            {
                DialogNewVersionAvailable dialogNewVersionAvailable = new DialogNewVersionAvailable(serverVersionStr, installedVersionStr);
                bool? result = dialogNewVersionAvailable.ShowDialog();
                if (result == true)
                {
                    app.ContextMenu_GotoHomePage();

                }
            }
            catch (Exception)
            {

                // throw;
            }
        }

        // ========================================================
        private void Show_DialogVersionUpToDate(
            string serverVersionStr,
            string installedVersionStr)
        {
            try
            {
                DialogVersionUpToDate dialogVersionUpToDate = new DialogVersionUpToDate(serverVersionStr, installedVersionStr);
                bool? result = dialogVersionUpToDate.ShowDialog();

            }
            catch (Exception)
            {

                // throw;
            }
        }

        // ========================================================
        private string[] GetLinePieces(string lineString)
        {
            string[] linePieces = lineString.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

            return linePieces;
        }
        // ========================================================
    }
}
