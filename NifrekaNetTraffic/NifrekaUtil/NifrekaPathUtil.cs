// ==============================
// Copyright 2022 nifreka.nl
// ==============================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Nifreka
{
    // ###############################################################
    public class NifrekaPathUtil
    {
        // ---------------------------------------------------------------------
        static public String GetAppAbsolutePath()
        {
            String exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            return exePath;
        }

        // --------------------------------------------------------------------------
        static public String GetParentDirPath(String filepath)
        {
            String path = "";
            try
            {
                FileInfo fi = new FileInfo(filepath);
                path = fi.DirectoryName;
            }
            catch (Exception)
            {
                // throw;
            }
            return path;
        }

 

        // ---------------------------------------------------------------------




    }
}
