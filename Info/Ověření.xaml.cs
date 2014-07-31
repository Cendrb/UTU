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
using System.Security;

namespace Info
{
    /// <summary>
    /// Interakční logika pro Ověření.xaml
    /// </summary>
    public partial class Ověření : Window
    {
        public Ověření()
        {
            InitializeComponent();
        }

        private void enterButton_Click(object sender, RoutedEventArgs e)
        {
            if (hesloBox.Password.Contains(""))
            {
                string pass = hesloBox.Password;
                MessageBox.Show("Heslo přijato!", "Výýýýýýýýýborně", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Špatné heslo\nZkuste to znovu", "Nesmysl, špatně!", MessageBoxButton.OK, MessageBoxImage.Error);
                hesloBox.Clear();
            }
        }
    }
}
