using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifrekaNetTraffic
{
    public static class Const
    {
        public const string NifrekaNet_Version= "1.0";
        public const int NifrekaNet_Build = 1;
        public const string NifrekaNet_CopyRight= "(c) 2022 nifreka.nl";



        public const string NifrekaNetTraffic_foldername = "NifrekaNetTraffic";
        public const string NifrekaNetTraffic_Data_foldername = "NifrekaNetTraffic_Data";
        public const string NifrekaNetTraffic_Settings_Filename = "NifrekaNetTraffic_Settings.DATA";

        public static string NifrekaNetTraffic_Settings_partialPath = NifrekaNetTraffic_foldername + @"\" + NifrekaNetTraffic_Data_foldername + @"\" + NifrekaNetTraffic_Settings_Filename;
        public static string NifrekaNetTraffic_Settings_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments) + @"\" + NifrekaNetTraffic_Settings_partialPath;




    }
}
