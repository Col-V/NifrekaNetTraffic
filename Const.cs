using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifrekaNetTraffic
{
    public static class Const
    {
        public const string NifrekaNet_Version= "1.1";
        public const int NifrekaNet_Build = 40;
        public const string NifrekaNet_CopyRight= "(c) 2022 nifreka.nl";



        public const string NifrekaNetTraffic_foldername = "NifrekaNetTraffic";
        public const string NifrekaNetTraffic_Data_foldername = "NifrekaNetTraffic_Data";

        public static string NifrekaNetTraffic_Data_folderpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments) + @"\" + NifrekaNetTraffic_foldername + @"\" + NifrekaNetTraffic_Data_foldername;


        public const string NifrekaNetTraffic_Settings_Filename_Old_v1 = "NifrekaNetTraffic_Settings.DATA";
        public static string NifrekaNetTraffic_Settings_partialPath_Old_v1 = NifrekaNetTraffic_foldername + @"\" + NifrekaNetTraffic_Data_foldername + @"\" + NifrekaNetTraffic_Settings_Filename_Old_v1;
        public static string NifrekaNetTraffic_Settings_PATH_Old_v1 = Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments) + @"\" + NifrekaNetTraffic_Settings_partialPath_Old_v1;


        public const string NifrekaNetTraffic_Settings_Filename = "NifrekaNetTraffic_Settings.nftDATA";
        public static string NifrekaNetTraffic_Settings_partialPath = NifrekaNetTraffic_foldername + @"\" + NifrekaNetTraffic_Data_foldername + @"\" + NifrekaNetTraffic_Settings_Filename;
        public static string NifrekaNetTraffic_Settings_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments) + @"\" + NifrekaNetTraffic_Settings_partialPath;

        public const string NifrekaNetTraffic_Log_Filename = "NifrekaNetTraffic_Log.nftDATA";
        public static string NifrekaNetTraffic_Log_partialPath = NifrekaNetTraffic_foldername + @"\" + NifrekaNetTraffic_Data_foldername + @"\" + NifrekaNetTraffic_Log_Filename;
        public static string NifrekaNetTraffic_Log_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments) + @"\" + NifrekaNetTraffic_Log_partialPath;



    }
}
