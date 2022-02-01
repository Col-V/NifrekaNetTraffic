using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifrekaNetTraffic
{
    // #######################################################################
    public class NifrekaNetTrafficSettings
    {
        // =====
        private double windowPos_Top;
        public double WindowPos_Top
        {
            get { return windowPos_Top; }
            set { windowPos_Top = value; }
        }

        // =====
        private double windowPos_Left;
        public double WindowPos_Left
        {
            get { return windowPos_Left; }
            set { windowPos_Left = value; }
        }

        // =====
        private bool window_Topmost;
        public bool Window_Topmost
        {
            get { return window_Topmost; }
            set { window_Topmost = value; }
        }



        // =================
        // ctor
        // =================
        public NifrekaNetTrafficSettings()
        {
            this.windowPos_Top = 0;
            this.windowPos_Left = 0;
            this.window_Topmost = false;
        }

        // =====================================
        public void ReadSettingsData()
        {
            try
            {
                string filepath = Const.NifrekaNetTraffic_Settings_PATH;
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));

                if (File.Exists(filepath) == true)
                {
                    using (BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open)))
                    {
                        int dataCount = br.ReadInt32();

                        for (int i = 0; i < dataCount; i++)
                        {
                            String label = br.ReadString();
                            ReadDataByLabel(label, br);
                        }
                    }
                }

                
            }
            catch (Exception)
            {
                // throw;
            }

        }

        // =====================================
        private void ReadDataByLabel(String label, BinaryReader br)
        {
            try
            {
                switch (label)
                {
                    case "WindowPos_Top":
                        {
                            this.WindowPos_Top = br.ReadDouble();
                            break;
                        }
                    case "WindowPos_Left":
                        {
                            this.WindowPos_Left = br.ReadDouble();
                            break;
                        }
                    case "Window_Topmost":
                        {
                            this.window_Topmost = br.ReadBoolean();
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                // throw;
            }
            



        }

        // =====================================
        public void WriteSettingsData()
        {
            try
            {
                string filepath = Const.NifrekaNetTraffic_Settings_PATH;
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));

                using (BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create)))
                {
                    int dataCount = 3;
                    bw.Write((int)dataCount);

                    bw.Write("WindowPos_Top"); bw.Write(this.WindowPos_Top);
                    bw.Write("WindowPos_Left"); bw.Write(this.WindowPos_Left);
                    bw.Write("Window_Topmost"); bw.Write(this.window_Topmost);
                }
            }
            catch (Exception)
            {
                // throw;
            }

        }




        // =====================================
    }





}
