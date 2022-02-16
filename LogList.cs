using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NifrekaNetTraffic
{
    public enum LogDataType { None = 0, Received = 1, Sent = 2 };

    // ###############################################################
    public class LogList : ObservableCollection<LogListItem>
    {
        private int maxItemCount = 86400;   // seconds
        private int maxBuffer = 3600;   // seconds

        private string filepath = Const.NifrekaNetTraffic_Log_PATH;
        private string fileVersion = "version_001";

        private long bytesReceivedTotal;
        public long BytesReceivedTotal
        {
            get { return bytesReceivedTotal; }
            set { bytesReceivedTotal = value; }
        }

        private long bytesSentTotal;
        public long BytesSentTotal
        {
            get { return bytesSentTotal; }
            set { bytesSentTotal = value; }
        }

        // =========================
        private long bytesReceivedLog;
        public long BytesReceivedLog
        {
            get { return bytesReceivedLog; }
            set { bytesReceivedLog = value; }
        }

        private long bytesSentLog;
        public long BytesSentLog
        {
            get { return bytesSentLog; }
            set { bytesSentLog = value; }
        }


        // =========================
        private long maxBytesPerInterval_Received;
        public long MaxBytesPerInterval_Received
        {
            get { return maxBytesPerInterval_Received; }
            set { maxBytesPerInterval_Received = value; }
        }

        private long maxBytesPerInterval_Sent;
        public long MaxBytesPerInterval_Sent
        {
            get { return maxBytesPerInterval_Sent; }
            set { maxBytesPerInterval_Sent = value; }
        }

        

        // ===========================
        // ctor
        // ===========================
        public LogList()
        {
            this.bytesReceivedTotal = 0;
            this.bytesSentTotal = 0;

            this.bytesReceivedLog = 0;
            this.bytesSentLog = 0;

            this.maxBytesPerInterval_Received = 0;
            this.maxBytesPerInterval_Sent = 0;
        }

        // =========================================
        public void Calc_Statistics()
        {
            this.bytesReceivedLog = 0;
            this.bytesSentTotal = 0;

            for (int i = 0; i < this.Count; i++)
            {
                LogListItem logListItem = this.ElementAt(i);
                this.bytesReceivedLog = this.bytesReceivedLog + logListItem.BytesReceivedInterval;
                this.bytesSentLog = this.bytesSentLog + logListItem.BytesSentInterval;

                this.maxBytesPerInterval_Received = Math.Max(this.maxBytesPerInterval_Received, logListItem.BytesReceivedInterval);
                this.maxBytesPerInterval_Sent = Math.Max(this.maxBytesPerInterval_Sent, logListItem.BytesSentInterval);
            }

        }



        // =========================================
        public void AddItem(LogListItem logListItem)
        {
            bytesReceivedLog = bytesReceivedLog + logListItem.BytesReceivedInterval;
            bytesSentLog = bytesSentLog + logListItem.BytesSentInterval;

            this.maxBytesPerInterval_Received = Math.Max(this.maxBytesPerInterval_Received, logListItem.BytesReceivedInterval);
            this.maxBytesPerInterval_Sent = Math.Max(this.maxBytesPerInterval_Sent, logListItem.BytesSentInterval);

            LimitToMaxItemCount();
            Add(logListItem);
        }

        // =========================================
        private void LimitToMaxItemCount()
        {
            if (this.Count >= maxItemCount)
            {
                while (this.Count >= this.maxItemCount - maxBuffer)
                {
                    this.RemoveAt(0);
                }
            }          
        }

        // =========================================
        public void ClearList()
        {
            this.bytesReceivedLog = 0;
            this.bytesSentLog = 0;

            this.maxBytesPerInterval_Received = 0;
            this.maxBytesPerInterval_Sent = 0;

            this.Clear();
        }

        // =========================================
        public void ReadFromFile()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.filepath));

                if (File.Exists(this.filepath) == true)
                {
                    using (BinaryReader br = new BinaryReader(File.Open(this.filepath, FileMode.Open)))
                    {
                        string fileVersion = br.ReadString();
                        if (fileVersion.Equals(this.fileVersion))
                        {
                            int dataCount = br.ReadInt32();

                            for (int i = 0; i < dataCount; i++)
                            {
                                long dataTimeTicks = br.ReadInt64();
                                long bytesReceivedInterval = br.ReadInt64();
                                long bytesSentInterval = br.ReadInt64();

                                LogListItem logListItem = new LogListItem(dataTimeTicks,
                                                                                        bytesReceivedInterval,
                                                                                        bytesSentInterval);

                                this.Add(logListItem);
                            }
                            Calc_Statistics();
                        }

                    }
                }
            }
            catch (Exception)
            {

                // throw;
            }



        }

        // =========================================
        public void WriteToFile()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.filepath));

                using (BinaryWriter bw = new BinaryWriter(File.Open(this.filepath, FileMode.Create)))
                {
                    bw.Write(this.fileVersion);
                    bw.Write((int)this.Count);

                    for (int i = 0; i < this.Count; i++)
                    {
                        LogListItem logListItem = this.ElementAt(i);

                        bw.Write(logListItem.DataTimeTicks);
                        bw.Write(logListItem.BytesReceivedInterval);
                        bw.Write(logListItem.BytesSentInterval);
                    }
                }

            }
            catch (Exception)
            {

                // throw;
            }

        }


        // =========================================
        public void WriteToTextFile(string filepath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));

                bool appendFlag = false;
                using (StreamWriter sw = new StreamWriter(filepath, appendFlag, Encoding.UTF8))
                {

                    for (int i = 0; i < this.Count; i++)
                    {
                        LogListItem logListItem = this.ElementAt(i);

                        sw.Write(logListItem.DataTime_Str); sw.Write("\t");
                        sw.Write(logListItem.BytesReceivedInterval.ToString()); sw.Write("\t");
                        sw.Write(logListItem.BytesSentInterval.ToString()); sw.Write(Environment.NewLine);
                    }
                }

            }
            catch (Exception)
            {

                // throw;
            }

        }

        // =========================================
        public void ExportAsText(double left, double top)
        {
            Window saveDialogWindow = new Window();
            // dialogPositioningWindow.Owner = this;
            saveDialogWindow.Left = left + 100;
            saveDialogWindow.Top = top + 100;
            saveDialogWindow.Width = 0;
            saveDialogWindow.Height = 0;
            saveDialogWindow.WindowStyle = WindowStyle.None;
            saveDialogWindow.ResizeMode = ResizeMode.NoResize;
            saveDialogWindow.AllowsTransparency = false;
            saveDialogWindow.Background = null;

            saveDialogWindow.Show();

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

            DateTime dt = DateTime.Now;
            string exportFilename = dt.Year.ToString() + "-"
                                    + dt.Month.ToString("D2") + "-"
                                    + dt.Day.ToString("D2")
                                    + "___"
                                    + dt.Hour.ToString("D2") + "-"
                                    + dt.Minute.ToString("D2") + "-"
                                    + dt.Second.ToString("D2")
                                    + "___"
                                    + "nftExport";

            saveFileDialog.FileName = exportFilename;
            saveFileDialog.Filter = "Text files |*.txt;";

            bool? result = saveFileDialog.ShowDialog(saveDialogWindow);
            if (result == true)
            {
                WriteToTextFile(saveFileDialog.FileName);

            }

            saveDialogWindow.Close();
        }



        // =========================================

    }



    // ###############################################################
    public class LogListItem
    {
        // ===============
        private long dataTimeTicks;
        public long DataTimeTicks
        {
            get { return dataTimeTicks; }
            set { dataTimeTicks = value; }
        }

        public string DataTime_Str
        {
            get {

                DateTime dt = new DateTime(dataTimeTicks);
                // string dt_str = dt.ToString("dd.MM.yyyy - HH:mm:ss");
                string dt_str = dt.ToString("dd.MM.yyyy - HH:mm:ss");
                return dt_str;
            }
            set {  }
        }

        // ===============
        private long bytesReceivedInterval;
        public long BytesReceivedInterval
        {
            get { return bytesReceivedInterval; }
            set { bytesReceivedInterval = value; }
        }

        public string BytesReceivedInterval_1000
        {
            get { return bytesReceivedInterval.ToString("#,##0"); }
            set { }
        }

        // ===============
        private long bytesSentInterval;
        public long BytesSentInterval
        {
            get { return bytesSentInterval; }
            set { bytesSentInterval = value; }
        }

        public string BytesSentInterval_1000
        {
            get { return bytesSentInterval.ToString("#,##0"); }
            set { }
        }

        // ===========================
        // ctor
        // ===========================
        public LogListItem(long dataTimeTicks, long bytesReceivedInterval, long bytesSentInterval)
        {
            this.dataTimeTicks = dataTimeTicks; 
            this.bytesReceivedInterval = bytesReceivedInterval;
            this.bytesSentInterval = bytesSentInterval;
        }
    }

}
