// ==============================
// Copyright 2022 nifreka.nl
// ==============================

using Nifreka;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private int plotWidth;
        private int plotHeight;

        private DataDirection dataDirection;
        private System.Drawing.Color color;

        private WriteableBitmap wBitmap;
        private System.Drawing.Graphics graphics;


        private WindowLogGraph windowLogGraph;
        private App app;

        public TextBox textBoxMax;

        private Grid grid;
        private System.Windows.Controls.Image image;

        // Label
        int yAxisLabelCount = 1;



        // ========================================================
        public void InitGraph(WindowLogGraph windowLogGraph, DataDirection dataDirection,
                                System.Drawing.Color color)
        {
            try
            {
                this.app = (App)Application.Current;
                this.windowLogGraph = windowLogGraph;

                this.dataDirection = dataDirection;
                this.color = color;

                pixelWidth = (int)this.ActualWidth;
                pixelHeight = (int)this.ActualHeight;

                plotWidth = pixelWidth - windowLogGraph.labelWidth;
                plotHeight = pixelHeight;

                this.grid = new Grid();
                this.Child = grid;

                this.image = new System.Windows.Controls.Image();
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

                System.Drawing.Color bgColor = System.Drawing.Color.FromArgb(0xFF, 0x40, 0x40, 0x80);
                graphics.Clear(bgColor);

                backBitmap.Dispose();
            }
            catch (Exception)
            {

                // throw;
            }
        }

        // ========================================================
        public void UpdateGraph(double faktorValue, int displayStartOffset, long max_Value, long max_scaled)
        {
            try
            {
                System.Drawing.Pen LinePen = new System.Drawing.Pen(color, 0.5f);

                wBitmap.Lock();

                System.Drawing.Color bgColor = System.Drawing.Color.FromArgb(0xFF, 0x20, 0x20, 0x20);
                graphics.Clear(bgColor);

                // ============================================
                // Draw logList                

                for (int i = 0; i < (plotWidth); i++)
                {
                    int idx = app.logList.Count - 1 - i - displayStartOffset;

                    if (idx > 0 && idx < app.logList.Count)
                    {
                        LogListItem logListItem = app.logList.ElementAt(idx);
                        LogListItem logListItem_previous = app.logList.ElementAt(idx - 1);

                        long valueLong = 0;

                        switch (dataDirection)
                        {
                            case DataDirection.Received:
                                valueLong = LogList.CalcBytesPerSecond(logListItem, logListItem_previous, DataDirection.Received);
                                break;

                            case DataDirection.Sent:
                                valueLong = LogList.CalcBytesPerSecond(logListItem, logListItem_previous, DataDirection.Sent);
                                break;
                        }

                        double value = valueLong * faktorValue;
                        int valueInt = (int)value;

                        System.Drawing.Point startPoint = new System.Drawing.Point(i, pixelHeight);
                        System.Drawing.Point endPoint = new System.Drawing.Point(i, pixelHeight - valueInt);
                        graphics.DrawLine(LinePen, startPoint, endPoint);
                    }

                }

                // ============================================
                // Draw y-Axis labels
                //
                Draw_YAxis(faktorValue, displayStartOffset, max_Value, max_scaled);

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
        private void Draw_YAxis(double faktorValue, int displayStartOffset, long max_Value, long max_Value_scaled)
        {
            long bitMultiplier = 1;

            if (windowLogGraph.dataUnit_useBit == true)
            {
                bitMultiplier = 8;
            }

            long max_Value_Bits = max_Value_scaled * bitMultiplier;

            Font font = new Font("Consolas", 14, GraphicsUnit.Pixel);
            SizeF sizeF_Label = graphics.MeasureString("1000", font);
            int label_Height = (int)sizeF_Label.Height + 1;

            int labelCount = plotHeight / label_Height;
            double vl = max_Value_Bits / labelCount;

            double vlog10 = Math.Log10(vl);
            int vlog10_int = (int)vlog10;
            double labelUnit = Math.Pow(10, vlog10_int);

            int labelVPos = 0;

            int y = 0;
            int k = 1;
            while (y < plotHeight && k < 300)
            {
                double yk = labelUnit * k;

                double yk_faktor = yk * faktorValue / bitMultiplier;
                y = (int)yk_faktor;

                if (y > (labelVPos + label_Height))
                {
                    labelVPos = y;

                    // label
                    //
                    // Font font = new Font("Consolas", 14, GraphicsUnit.Pixel);                    
                    string yLabelStr = yk.ToString("#,##0");
                    sizeF_Label = graphics.MeasureString(yLabelStr, font);
                    PointF point = new PointF(pixelWidth - (int)sizeF_Label.Width, plotHeight - y - sizeF_Label.Height / 2);
                    graphics.DrawString(yLabelStr, font, System.Drawing.Brushes.White, point);

                    // horizontal grid line
                    //
                    System.Drawing.Color LabelLinePen_Color = System.Drawing.Color.FromArgb(0xFF, 0x80, 0x80, 0x80);
                    System.Drawing.Pen LabelLinePen = new System.Drawing.Pen(LabelLinePen_Color, 0.5f);

                    System.Drawing.Point sp = new System.Drawing.Point(0, plotHeight - y);
                    System.Drawing.Point ep = new System.Drawing.Point(plotWidth, plotHeight - y);
                    graphics.DrawLine(LabelLinePen, sp, ep);
                }

                k = k + 1;
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

    }
}
