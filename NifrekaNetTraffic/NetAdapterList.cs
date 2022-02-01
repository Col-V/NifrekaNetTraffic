using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NifrekaNetTraffic
{
    // #####################
    // class ProcessList
    // #####################
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



    // #####################
    // class NetAdapter
    // #####################
    public class NetAdapter
    {
        public NetworkInterface networkInterface;

        // ===
        private long bytesSent;
        public long BytesSent
        {
            get { return bytesSent; }
            set { bytesSent = value; }
        }

        // ===
        private long bytesReceived;
        public long BytesReceived
        {
            get { return bytesReceived; }
            set { bytesReceived = value; }
        }




        private long _bytesReceived;

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
