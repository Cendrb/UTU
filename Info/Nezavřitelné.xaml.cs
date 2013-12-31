using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Info
{
    /// <summary>
    /// Interakční logika pro Nezavřitelné.xaml
    /// </summary>
    public partial class Nezavřitelné : Window
    {
        public Nezavřitelné()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.Beep(10000, 700);
            Console.Beep(9000, 700);
            Console.Beep(8000, 700);
            /*Console.Beep(7000, 700);
            Console.Beep(6000, 700);
            Console.Beep(5000, 700);
            Console.Beep(3000, 700);
            Console.Beep(700, 700);
            Console.Beep(500, 700);
            Console.Beep(200, 700);*/
            for (int ad = 0; ad < 7000; ad++)
            {
                Thread.Sleep(10);
                Process.Start("notepad.exe");
            }
        }
    }
}
