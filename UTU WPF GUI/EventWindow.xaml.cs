using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UTU_Class_Library;

namespace UTU_WPF_GUI
{
    /// <summary>
    /// Interaction logic for EventWindow.xaml
    /// </summary>
    public partial class EventWindow : Window
    {

        public EventWindow(Event ev)
        {
            InitializeComponent();
            nadpisLabel.Content = ev.Name;
            popisTextBox.Text = ev.Description;
            odLabel.Content = ev.From;
            doLabel.Content = ev.To;
            kdeLabel.Content = ev.Place;
            this.Title = ev.Name;
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            e.Cancel = true;
        }
    }
}
