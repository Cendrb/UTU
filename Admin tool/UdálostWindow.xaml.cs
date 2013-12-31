using System;
using System.Collections.Generic;
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
using Info;

namespace Admin_tool
{
    /// <summary>
    /// Interakční logika pro Úkol.xaml
    /// </summary>
    public partial class UdálostWindow : Window
    {
        public Událost Událost { get; private set; }
        public UdálostWindow()
        {
            InitializeComponent();
        }

        private void uložitButton_Click(object sender, RoutedEventArgs e)
        {
            if (názevTextBox.Text != "" && popisTextBox.Text != "" && odDatePicker.SelectedDate != null && místoTextBox.Text != "" && doDatePicker.SelectedDate != null)
            {
                Událost = new Událost(názevTextBox.Text, popisTextBox.Text, (DateTime)odDatePicker.SelectedDate, místoTextBox.Text, (DateTime)doDatePicker.SelectedDate);
                this.Close();
            }
            else
            {
                MessageBox.Show("Některá z požadovaných polí nejsou vyplněna!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
