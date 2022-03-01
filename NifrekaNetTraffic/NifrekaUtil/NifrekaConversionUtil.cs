// ==============================
// Copyright 2022 nifreka.nl
// ==============================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Nifreka
{
    // ###############################################################
    public static class NifrekaConversionUtil
    {
        // ====================================
        public static double Sinus(double zahl)
        {
            return Math.Sin(zahl * (Math.PI / 180));
        }

        public static double Cosinus(double zahl)
        {
            return Math.Cos(zahl * (Math.PI / 180));
        }

        // ====================================
        // BinaryBit
        //
        static long K_1024 = 1024L;
        static long M_1024 = 1024L * 1024L;
        static long G_1024 = 1024L * 1024L * 1024L;
        static long T_1024 = 1024L * 1024L * 1024L * 1024L;

        // DecimalBit
        //
        static long K_1000 = 1000L;
        static long M_1000 = 1000L * 1000L;
        static long G_1000 = 1000L * 1000L * 1000L;
        static long T_1000 = 1000L * 1000L * 1000L * 1000L;

        // ====================================
        public static string Bit1024Str_from_LongBytes(long bytes)
        {
            long bits = bytes * 8;

            string resultStr = bits.ToString() + " bit";
            double d = bits;

            if (bits > K_1024)
            {
                d = (double)bits / (double)K_1024;
                resultStr = d.ToString("0.0") + " Kibit";
            }

            if (bits > M_1024)
            {
                d = (double)bits / (double)M_1024;
                resultStr = d.ToString("0.0") + " Mibit";
            }

            if (bits > G_1024)
            {
                d = (double)bits / (double)G_1024;
                resultStr = d.ToString("0.0") + " Gibit";
            }

            if (bits > T_1024)
            {
                d = (double)bits / (double)T_1024;
                resultStr = d.ToString("0.0") + " Tibit";
            }

            return resultStr;
        }
        // ====================================
        public static string Bit1000Str_from_LongBytes(long bytes)
        {
            long bits = bytes * 8;

            string resultStr = bits.ToString() + " bit";
            double d = bits;

            if (bits > K_1000)
            {
                d = (double)bits / (double)K_1000;
                resultStr = d.ToString("0.0") + " kbit";
            }

            if (bits > M_1000)
            {
                d = (double)bits / (double)M_1000;
                resultStr = d.ToString("0.0") + " Mbit";
            }

            if (bits > G_1000)
            {
                d = (double)bits / (double)G_1000;
                resultStr = d.ToString("0.0") + " Gbit";
            }

            if (bits > T_1000)
            {
                d = (double)bits / (double)T_1000;
                resultStr = d.ToString("0.0") + " Tbit";
            }

            return resultStr;
        }

        // ====================================
        public static string Bytes1000Str_from_LongBytes(long bytes)
        {
            long bits = bytes;

            string resultStr = bits.ToString() + " B";
            double d = bits;

            if (bits > K_1000)
            {
                d = (double)bits / (double)K_1000;
                resultStr = d.ToString("0.0") + " KB";
            }

            if (bits > M_1000)
            {
                d = (double)bits / (double)M_1000;
                resultStr = d.ToString("0.0") + " MB";
            }

            if (bits > G_1000)
            {
                d = (double)bits / (double)G_1000;
                resultStr = d.ToString("0.0") + " GB";
            }

            if (bits > T_1000)
            {
                d = (double)bits / (double)T_1000;
                resultStr = d.ToString("0.0") + " TB";
            }

            return resultStr;
        }

        // ====================================
        public static string BytesUnit1000Str_from_LongBytes(long bytes)
        {
            string resultStr = "B";

            if (bytes > K_1000)
            {
                resultStr = "KB";
            }

            if (bytes > M_1000)
            {
                resultStr = "MB";
            }

            if (bytes > G_1000)
            {
                resultStr = "GB";
            }

            if (bytes > T_1000)
            {
                resultStr = "TB";
            }

            return resultStr;
        }

        // ====================================
        public static string BitUnit1000Str_from_LongBits(long bits)
        {
            string resultStr = "bit";

            if (bits > K_1000)
            {
                resultStr = "Kbit";
            }

            if (bits > M_1000)
            {
                resultStr = "Mbit";
            }

            if (bits > G_1000)
            {
                resultStr = "Gbit";
            }

            if (bits > T_1000)
            {
                resultStr = "Tbit";
            }

            return resultStr;
        }





        // ====================================
        public static Boolean isInteger(String integerString)
        {
            int value;
            try
            {
                if (int.TryParse(integerString, out value))
                    return true;
                else
                    return false;
            }
            catch (Exception )
            {
                return false;
            }
        }

        // ====================================
        public static int StrToInt(String integerString)
        {
            int value = -1;
            try
            {
                if (isInteger(integerString))
                {
                    int.TryParse(integerString, out value);
                }

            }
            catch (Exception )
            {
            }

            return value;
        }

        


        // ====================================


    }
}
