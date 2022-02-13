// ==============================
// Copyright 2021 vennway.com
// ==============================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Nifreka
{
    // ###############################################################
    public class NifrekaScreenUtil
    {
        // ====================================
        public int GetScreenCount()
        {
            System.Windows.Forms.Screen[] screenArr = System.Windows.Forms.Screen.AllScreens;
            return screenArr.Count();
        }

        // ====================================
        public static System.Windows.Forms.Screen GetScreen_by_Window_Intersects_Most(Window window)
        {
            System.Windows.Forms.Screen screen = null;

            int windowPosition_x = Convert.ToInt32(window.Left);
            int windowPosition_y = Convert.ToInt32(window.Top);

            int windowWidth = Convert.ToInt32(window.ActualWidth);
            int windowHeight = Convert.ToInt32(window.ActualHeight);

            System.Drawing.Rectangle windowRect = new System.Drawing.Rectangle(windowPosition_x, windowPosition_y, windowWidth, windowHeight);           

            System.Windows.Forms.Screen[] screenArr = System.Windows.Forms.Screen.AllScreens;
            int screenCount = screenArr.Count();

            List<System.Windows.Forms.Screen> screenList = new List<System.Windows.Forms.Screen>();

            for (int i = 0; i < screenCount; i++)
            {
                System.Windows.Forms.Screen aScreen = screenArr[i];

                if (aScreen.Bounds.IntersectsWith(windowRect))
                {
                    screenList.Add(aScreen);
                }
            }

            if (screenList.Count == 1)
            {
                screen = screenList.ElementAt(0);
            }
            
            if (screenList.Count > 1)
            {
                screen = GetScreen_by_Window_Intersects_Most(windowRect, screenList);
            }

            return screen;
        }

        // ====================================
        private static System.Windows.Forms.Screen GetScreen_by_Window_Intersects_Most(
                                                                        System.Drawing.Rectangle windowRect,
                                                                        List<System.Windows.Forms.Screen> screenList)
        {
            System.Windows.Forms.Screen screen = null;

            int maxArea = 0;
            int maxArea_idx = 0;

            for (int i = 0; i < screenList.Count; i++)
            {
                System.Drawing.Rectangle windowRect_i = new System.Drawing.Rectangle(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height);

                System.Windows.Forms.Screen aScreen = screenList.ElementAt(i);
                windowRect_i.Intersect(aScreen.Bounds);
                int area_i = windowRect_i.Width * windowRect_i.Height;

                if(area_i > maxArea)
                {
                    maxArea = area_i;
                    maxArea_idx = i;

                    screen = aScreen;
                }
            }

            return screen;
        }


        // ====================================
        public static System.Windows.Forms.Screen GetScreen_By_Mouse()
        {
            System.Windows.Forms.Screen screen = null;

            System.Drawing.Point mousePosition = System.Windows.Forms.Control.MousePosition;

            int position_x = Convert.ToInt32(mousePosition.X);
            int position_y = Convert.ToInt32(mousePosition.Y);

            System.Drawing.Rectangle pointRect = new System.Drawing.Rectangle(position_x, position_y, 1, 1);

            System.Windows.Forms.Screen[] screenArr = System.Windows.Forms.Screen.AllScreens;
            int screenCount = screenArr.Count();

            for (int i = 0; i < screenCount; i++)
            {
                System.Windows.Forms.Screen aScreen = screenArr[i];

                if (aScreen.Bounds.IntersectsWith(pointRect))
                {
                    screen = aScreen;
                    break;
                }
            }

            return screen;
        }

        // ====================================
        public static System.Windows.Forms.Screen GetScreen_By_Point(Point point)
        {
            System.Windows.Forms.Screen screen = null;

            int position_x = Convert.ToInt32(point.X);
            int position_y = Convert.ToInt32(point.Y);

            System.Drawing.Rectangle pointRect = new System.Drawing.Rectangle(position_x, position_y, 1, 1);

            System.Windows.Forms.Screen[] screenArr = System.Windows.Forms.Screen.AllScreens;
            int screenCount = screenArr.Count();

            for (int i = 0; i < screenCount; i++)
            {
                System.Windows.Forms.Screen aScreen = screenArr[i];

                if (aScreen.Bounds.IntersectsWith(pointRect))
                {
                    screen = aScreen;
                    break;
                }
            }

            return screen;
        }

        // ====================================
        public static System.Windows.Forms.Screen GetScreen_By_WindowTopLeft(Window window)
        {
            System.Windows.Forms.Screen screen = null;

            if (!Double.IsNaN(window.Left)
                &&
                !Double.IsNaN(window.Top)
                )
            {

                Double left = window.Left;
                Double top = window.Top;

                int pointWindowLeftTop_x = Convert.ToInt32(left);
                int pointWindowLeftTop_y = Convert.ToInt32(top);

                System.Drawing.Rectangle mpRect = new System.Drawing.Rectangle(pointWindowLeftTop_x, pointWindowLeftTop_y, 1, 1);

                System.Windows.Forms.Screen[] screenArr = System.Windows.Forms.Screen.AllScreens;
                int screenCount = screenArr.Length;

                for (int i = 0; i < screenCount; i++)
                {
                    System.Windows.Forms.Screen aScreen = screenArr[i];

                    if (aScreen.Bounds.IntersectsWith(mpRect))
                    {
                        screen = aScreen;
                        break;
                    }
                }
            }

            return screen;
        }

        // ====================================
        public static System.Windows.Forms.Screen GetScreen_By_WindowCenter(Window window)
        {
            System.Windows.Forms.Screen screen = null;

            if (!Double.IsNaN(window.Left)
                &&
                !Double.IsNaN(window.Top)
                )
            {
                Double centerX = window.Left + window.ActualWidth / 2;
                Double centerY = window.Top + window.ActualHeight / 2;

                int pointWindowCenter_x = Convert.ToInt32(centerX);
                int pointWindowCenter_y = Convert.ToInt32(centerY);

                System.Drawing.Rectangle mpRect = new System.Drawing.Rectangle(pointWindowCenter_x, pointWindowCenter_y, 1, 1);

                System.Windows.Forms.Screen[] screenArr = System.Windows.Forms.Screen.AllScreens;
                int screenCount = screenArr.Length;

                for (int i = 0; i < screenCount; i++)
                {
                    System.Windows.Forms.Screen aScreen = screenArr[i];

                    if (aScreen.Bounds.IntersectsWith(mpRect))
                    {
                        screen = aScreen;
                        break;
                    }
                }
            }

            return screen;
        }









        // ---------------------------------------------------------------------




    }
}
