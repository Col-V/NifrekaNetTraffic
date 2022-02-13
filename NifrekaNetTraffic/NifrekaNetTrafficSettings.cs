using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifrekaNetTraffic
{
    // ###############################################################
    public class NifrekaNetTrafficSettings
    {
        public double WindowLogTable_DefaultHeight = 325;
        public double WindowLogGraph_DefaultHeight = 300;


        // ========================
        // WindowMain
        // ========================

        private double left_WindowMain;
        public double Left_WindowMain
        {
            get { return left_WindowMain; }
            set { left_WindowMain = value; }
        }

        // ==============================
        private double top_WindowMain;
        public double Top_WindowMain
        {
            get { return top_WindowMain; }
            set { top_WindowMain = value; }
        }

        // ==============================
        private bool topmost_WindowMain;
        public bool Topmost_WindowMain
        {
            get { return topmost_WindowMain; }
            set { topmost_WindowMain = value; }
        }

        // ========================
        // WindowLogTable
        // ========================
    
        private double left_WindowLogTable;
        public double Left_WindowLogTable
        {
            get { return left_WindowLogTable; }
            set { left_WindowLogTable = value; }
        }

        // ==============================
        private double top_WindowLogTable;
        public double Top_WindowLogTable
        {
            get { return top_WindowLogTable; }
            set { top_WindowLogTable = value; }
        }

        // ==============================
        private double width_WindowLogTable = 445;
        public double Width_WindowLogTable
        {
            get { return width_WindowLogTable; }
            set { width_WindowLogTable = value; }
        }

        // ==============================
        private double height_WindowLogTable = 320;
        public double Height_WindowLogTable
        {
            get { return height_WindowLogTable; }
            set { height_WindowLogTable = value; }
        }

        // ==============================
        private bool visibleAtStart_WindowLogTable;
        public bool VisibleAtStart_WindowLogTable 
        {
            get { return visibleAtStart_WindowLogTable; }
            set { visibleAtStart_WindowLogTable = value; }
        }

        // ========================
        // WindowLogGraph
        // ========================
        
        private double left_WindowLogGraph;
        public double Left_WindowLogGraph
        {
            get { return left_WindowLogGraph; }
            set { left_WindowLogGraph = value; }
        }

        // ==============================
        private double top_WindowLogGraph;
        public double Top_WindowLogGraph
        {
            get { return top_WindowLogGraph; }
            set { top_WindowLogGraph = value; }
        }

        // ==============================
        private double width_WindowLogGraph = 445;
        public double Width_WindowLogGraph
        {
            get { return width_WindowLogGraph; }
            set { width_WindowLogGraph = value; }
        }

        // ==============================
        private double height_WindowLogGraph = 300;
        public double Height_WindowLogGraph
        {
            get { return height_WindowLogGraph; }
            set { height_WindowLogGraph = value; }
        }

        // ==============================
        private bool visibleAtStart_WindowLogGraph;
        public bool VisibleAtStart_WindowLogGraph
        {
            get { return visibleAtStart_WindowLogGraph; }
            set { visibleAtStart_WindowLogGraph = value; }
        }


        // =================
        // ctor
        // =================
        public NifrekaNetTrafficSettings()
        {
            this.top_WindowMain = 0;
            this.left_WindowMain = 0;
            this.topmost_WindowMain = false;
        }

        // =====================================
        public void SetDefaultHeight()
        {
            height_WindowLogTable = WindowLogTable_DefaultHeight;
            height_WindowLogGraph = WindowLogGraph_DefaultHeight;
        }

        // =====================================
        public void CleanUp_OldDataFile()
        {
            string oldFilepath = Const.NifrekaNetTraffic_Settings_PATH_Old_v1;

            try
            {
                if (File.Exists(oldFilepath) == true)
                {
                    File.Delete(oldFilepath);
                }
            }
            catch (Exception)
            {
                // throw;
            }

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
                    case "Left_WindowMain":
                        {
                            this.Left_WindowMain = br.ReadDouble();
                            break;
                        }

                    case "Top_WindowMain":
                        {
                            this.Top_WindowMain = br.ReadDouble();
                            break;
                        }
                    
                    case "Topmost_WindowMain":
                        {
                            this.Topmost_WindowMain = br.ReadBoolean();
                            break;
                        }

                    // ========================
                    // WindowLogTable
                    // ========================
                    case "Left_WindowLogTable":
                        {
                            this.Left_WindowLogTable = br.ReadDouble();
                            break;
                        }
                    
                    case "Top_WindowLogTable":
                        {
                            this.Top_WindowLogTable = br.ReadDouble();
                            break;
                        }
                    
                    case "Width_WindowLogTable":
                        {
                            this.Width_WindowLogTable = br.ReadDouble();
                            break;
                        }

                    case "Height_WindowLogTable":
                        {
                            this.Height_WindowLogTable = br.ReadDouble();
                            break;
                        }

                    case "VisibleAtStart_WindowLogTable":
                        {
                            this.VisibleAtStart_WindowLogTable = br.ReadBoolean();
                            break;
                        }

                    // ========================
                    // WindowLogGraph
                    // ========================
                    case "Left_WindowLogGraph":
                        {
                            this.Left_WindowLogGraph = br.ReadDouble();
                            break;
                        }
                    case "Top_WindowLogGraph":
                        {
                            this.Top_WindowLogGraph = br.ReadDouble();
                            break;
                        }

                    case "Width_WindowLogGraph":
                        {
                            this.Width_WindowLogGraph = br.ReadDouble();
                            break;
                        }

                    case "Height_WindowLogGraph":
                        {
                            this.Height_WindowLogGraph = br.ReadDouble();
                            break;
                        }

                    case "VisibleAtStart_WindowLogGraph":
                        {
                            this.VisibleAtStart_WindowLogGraph = br.ReadBoolean();
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
                    int dataCount = 13;
                    bw.Write((int)dataCount);

                    bw.Write("Left_WindowMain"); bw.Write(this.Left_WindowMain);
                    bw.Write("Top_WindowMain"); bw.Write(this.Top_WindowMain);                   
                    bw.Write("Topmost_WindowMain"); bw.Write(this.Topmost_WindowMain);

                    bw.Write("Left_WindowLogTable"); bw.Write(this.Left_WindowLogTable);
                    bw.Write("Top_WindowLogTable"); bw.Write(this.Top_WindowLogTable);
                    bw.Write("Width_WindowLogTable"); bw.Write(this.Width_WindowLogTable);
                    bw.Write("Height_WindowLogTable"); bw.Write(this.Height_WindowLogTable);
                    bw.Write("VisibleAtStart_WindowLogTable"); bw.Write(this.VisibleAtStart_WindowLogTable);

                    bw.Write("Left_WindowLogGraph"); bw.Write(this.Left_WindowLogGraph);
                    bw.Write("Top_WindowLogGraph"); bw.Write(this.Top_WindowLogGraph);
                    bw.Write("Width_WindowLogGraph"); bw.Write(this.Width_WindowLogGraph);
                    bw.Write("Height_WindowLogGraph"); bw.Write(this.Height_WindowLogGraph);
                    bw.Write("VisibleAtStart_WindowLogGraph"); bw.Write(this.VisibleAtStart_WindowLogGraph);
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
