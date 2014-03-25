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
    /// Interaction logic for ExamWindow.xaml
    /// </summary>
    public partial class ExamWindow : Window
    {
        private Exam local;
        private bool done;
        public bool Done
        {
            get
            {
                return done;
            }
            private set
            {
                done = value;
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Testy", local.Id + ".dat")))
                {
                    sw.Write(Done);
                }
            }
        }
        public ExamWindow(Exam input)
        {
            local = input;
            InitializeComponent();
            Title = input.Name;
            nadpisLabel.Content = input.Name;
            popisTextBox.Text = input.Description;
            doLabel.Content = input.Date.ToShortDateString();
            předmětLabel.Content = input.Subject;
            if (input.Group != 0)
                skupinaLabel.Content = input.Group + ".";
            else
                skupinaLabel.Content = "1. a 2.";

            //nahrát odloženo ze soubvoru
            if (File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Testy", local.Id + ".dat")))
                using (StreamReader sr = new StreamReader(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Testy", local.Id + ".dat")))
                {
                    done = Convert.ToBoolean(sr.ReadLine());
                }
            else
                done = false;

            //set checkboxes
            hotovoCheckBox.IsChecked = this.Done;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            e.Cancel = true;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void hotovoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Done = true;
        }

        private void hotovoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Done = false;
        }
    }
}
