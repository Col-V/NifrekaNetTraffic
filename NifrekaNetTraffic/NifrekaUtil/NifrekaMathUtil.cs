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

namespace Nifreka
{
    // ###############################################################
    public class NifrekaMathUtil
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
        static long Kibit = 1024L;
        static long Mibit = 1024L * 1024L;
        static long Gibit = 1024L * 1024L * 1024L;
        static long Tibit = 1024L * 1024L * 1024L * 1024L;

        // DecimalBit
        //
        static long KB = 1000L;
        static long MB = 1000L * 1000L;
        static long GB = 1000L * 1000L * 1000L;
        static long TB = 1000L * 1000L * 1000L * 1000L;

        // ====================================
        public static string BinaryBitStr_from_Long(long bytes)
        {
            long bits = bytes * 8;

            string resultStr = bits.ToString() + "  bit";
            double d = bits;

            if (bits > Kibit)
            {
                d = (double)bits / (double)Kibit;
                resultStr = d.ToString("0.0") + "  Kibit";
            }

            if (bits > Mibit)
            {
                d = (double)bits / (double)Mibit;
                resultStr = d.ToString("0.0") + "  Mibit";
            }

            if (bits > Gibit)
            {
                d = (double)bits / (double)Gibit;
                resultStr = d.ToString("0.0") + "  Gibit";
            }

            if (bits > Tibit)
            {
                d = (double)bits / (double)Tibit;
                resultStr = d.ToString("0.0") + " Tibit";
            }

            return resultStr;
        }
        // ====================================
        public static string DecimalBitStr_from_Long(long bytes)
        {
            long bits = bytes * 8;

            string resultStr = bits.ToString() + "  bit";
            double d = bits;

            if (bits > KB)
            {
                d = (double)bits / (double)KB;
                resultStr = d.ToString("0.0") + "  Kbit";
            }

            if (bits > MB)
            {
                d = (double)bits / (double)MB;
                resultStr = d.ToString("0.0") + "  Mbit";
            }

            if (bits > GB)
            {
                d = (double)bits / (double)GB;
                resultStr = d.ToString("0.0") + "  Gbit";
            }

            if (bits > TB)
            {
                d = (double)bits / (double)TB;
                resultStr = d.ToString("0.0") + " Tbit";
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
            catch (Exception exc)
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
            catch (Exception exc)
            {
            }

            return value;
        }
        // ====================================


    }
}
