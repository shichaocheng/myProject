using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;
using System.Threading;

namespace WpfApp1.component
{
    class PopupTimeOut
    {
        public PopupTimeOut() { }
        private Popup p = new Popup();
        private Timer t;
        public void show(string text, int interval)
        {
            display(text, interval);
        }

        //显示
        public void display(string text, int interval)
        {
            Border border = new Border();
            border.Background = new SolidColorBrush(Color.FromArgb(150, 50, 50, 50));
            border.HorizontalAlignment = HorizontalAlignment.Center;
            border.VerticalAlignment = VerticalAlignment.Center;
            border.Padding = new Thickness(20);
            TextBlock txt = new TextBlock();
            txt.Foreground = new SolidColorBrush(Colors.White);
            txt.FontSize = 20;
            txt.Text = text;
            border.Child = txt;
            p.Child = border;
            p.IsOpen = true;
            t = new Timer(new TimerCallback(timerCall), this, interval, 0);
            //MessageBox.Show(Thread.CurrentThread.GetHashCode().ToString());

        }

        //消失

        private void timerCall(object obj)
        {
            //t.Dispose();
            //p.IsOpen = false;
            //MessageBox.Show(Thread.CurrentThread.GetHashCode().ToString());

        }
    }
}
