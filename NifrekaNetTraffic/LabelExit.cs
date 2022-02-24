// ==============================
// Copyright 2022 nifreka.nl
// ==============================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NifrekaNetTraffic
{
    // #######################################################################
    public class LabelExit : Label
    {
        private Window? window;

        // ========================
        // ctor
        // ========================
        public LabelExit()
        {
            this.Visibility = Visibility.Hidden;

            this.MouseEnter += new System.Windows.Input.MouseEventHandler(this.LabelExit_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(this.LabelExit_MouseLeave);
            this.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.LabelExit_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.LabelExit_MouseLeftButtonUp);
        }

        // ========================================================
        public void SetWindow(Window window)
        {
            this.window = window;   
        }

        // ========================================================
        public void SetVisible()
        {
            this.Background = System.Windows.Media.Brushes.Red;
            this.Foreground = System.Windows.Media.Brushes.White;
            this.Visibility = Visibility.Visible;
        }
        // ========================================================
        public void SetInVisible()
        {
            this.Visibility = Visibility.Hidden;
        }

        // ========================================================
        private void LabelExit_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;

            this.Background = System.Windows.Media.Brushes.Red;
            this.Foreground = System.Windows.Media.Brushes.White;
            this.BorderBrush = System.Windows.Media.Brushes.White;
            e.Handled = true;
        }

        // ========================================================
        private void LabelExit_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = System.Windows.Media.Brushes.Red;
            this.Foreground = System.Windows.Media.Brushes.White;
            this.BorderBrush = System.Windows.Media.Brushes.Red;
            e.Handled = true;
        }

        // ========================================================
        private void LabelExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Background = System.Windows.Media.Brushes.White;
            this.Foreground = System.Windows.Media.Brushes.Red;
            e.Handled = true;
        }

        // ========================================================
        private void LabelExit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Background = System.Windows.Media.Brushes.Magenta;
            this.Foreground = System.Windows.Media.Brushes.Black;

            if(this.window != null)
            {
                this.window.Close();
            }
            
            e.Handled = true;
        }

        // ========================================================

        
    }
}
