using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NifrekaNetTraffic
{

    // ###############################################################
    public class NetAdapterList : ObservableCollection<NetAdapter>
    {

        // =============
        // ctor
        // =============
        public NetAdapterList()
        {

        }


        // =========================================
    }


    // ###############################################################
    public class NetAdapter
    {
        public NetworkInterface networkInterface;

        // ===
        private long bytesSent;
        public long BytesSent
        {
            get {
                long bytesSent = 0;
                if (networkInterface != null)
                {
                    bytesSent = networkInterface.GetIPStatistics().BytesSent;
                }

                return bytesSent; 
            }
        }

        // ===
        public long BytesReceived
        {
            get {
                long bytesReceived = 0;
                if (networkInterface != null)
                {
                    bytesReceived = networkInterface.GetIPStatistics().BytesReceived;
                }
                return bytesReceived; }
        }



        // ========================
        // ctor
        // ========================
        public NetAdapter(NetworkInterface networkInterface)
        {
            this.networkInterface = networkInterface;
        }


        // ========================================================





        // ========================================================

    }
}
