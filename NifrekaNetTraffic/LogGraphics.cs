using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NifrekaNetTraffic
{
    // ###############################################################
    public class LogGraphics : Border
    {
        private int pixelWidth;
        private int pixelHeight;


        private LogDataType logDataType;
        private System.Drawing.Color color;

        private WriteableBitmap wBitmap;
        private System.Drawing.Graphics graphics;


        private WindowMain windowMain;
        private WindowLogGraph windowLogGraph;

        public TextBox textBoxMax;

        private Grid grid;
        private Image image;

        


        // ========================================================
        public void InitGraph(WindowLogGraph windowLogGraph, 
                                LogDataType logDataType,
                                System.Drawing.Color color)
        {
            try
            {
                this.windowLogGraph = windowLogGraph;
                this.windowMain = windowLogGraph.windowMain;

                this.logDataType = logDataType;
                this.color = color;

                pixelWidth = (int)this.ActualWidth;
                pixelHeight = (int)this.ActualHeight;

                this.grid = new Grid();
                this.Child = grid;

                this.image = new Image();
                grid.Children.Add(image);

                // ====
                wBitmap = new WriteableBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32, null);
                image.Source = wBitmap;

                System.Drawing.Bitmap backBitmap = new System.Drawing.Bitmap(pixelWidth,
                                                                            pixelHeight,
                                                                            wBitmap.BackBufferStride,
                                                                            System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                                                                            wBitmap.BackBuffer);

                // ====
                graphics = System.Drawing.Graphics.FromImage(backBitmap);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                System.Drawing.Color bgColor = System.Drawing.Color.FromArgb(0xFF, 0x40, 0x40, 0x40);
                graphics.Clear(bgColor);

                backBitmap.Dispose();
            }
            catch (Exception)
            {

                // throw;
            }
        }

        // ========================================================
        public void UpdateGraph(double faktorValue, int displayStartOffset)
        {
            try
            {
                System.Drawing.Pen myPen = new System.Drawing.Pen(color, 0.5f);

                wBitmap.Lock();

                System.Drawing.Color bgColor = System.Drawing.Color.FromArgb(0xFF, 0x20, 0x20, 0x20);
                graphics.Clear(bgColor);

                for (int i = 0; i < pixelWidth; i++)
                {
                    int idx = this.windowMain.logList.Count - 1 - i - displayStartOffset;

                    if (idx >= 0 && idx < this.windowMain.logList.Count)
                    {
                        LogListItem logListItem = this.windowMain.logList.ElementAt(idx);

                        long valueLong = 0;

                        switch (logDataType)
                        {
                            case LogDataType.Received:
                                valueLong = logListItem.BytesReceivedInterval;
                                break;

                            case LogDataType.Sent:
                                valueLong = logListItem.BytesSentInterval;
                                break;
                        }

                        double value = valueLong * faktorValue;
                        int valueInt = (int)value;

                        System.Drawing.Point startPoint = new System.Drawing.Point(i, pixelHeight);
                        System.Drawing.Point endPoint = new System.Drawing.Point(i, pixelHeight - valueInt);

                        graphics.DrawLine(myPen, startPoint, endPoint);
                    }

                }

                graphics.Flush();

                wBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, pixelWidth, pixelHeight));
                wBitmap.Unlock();
            }
            catch (Exception)
            {
                // throw;
            }



        }

        // ========================================================
        public void Dispose()
        {
            if (graphics != null)
            {
                graphics.Dispose();
            }

            grid.Children.Remove(image);
            this.Child = null;
        }

        // ========================================================
        public void Resize()
        {
            try
            {
                Dispose();

                pixelWidth = (int)this.ActualWidth;
                pixelHeight = (int)this.ActualHeight;

                this.image = new Image();
                grid.Children.Add(image);

                wBitmap = new WriteableBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32, null);
                image.Source = wBitmap;

                System.Drawing.Bitmap backBitmap = new System.Drawing.Bitmap(pixelWidth,
                                                                            pixelHeight,
                                                                            wBitmap.BackBufferStride,
                                                                            System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                                                                            wBitmap.BackBuffer);

                // ====
                graphics = System.Drawing.Graphics.FromImage(backBitmap);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.Clear(System.Drawing.Color.LightCyan);
            }
            catch (Exception)
            {

                // throw;
            }
        }

        // ========================================================

    }
}
