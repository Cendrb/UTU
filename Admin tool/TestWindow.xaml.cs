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
    /// Interakční logika pro TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public Test Test { get; private set; }
        public TestWindow()
        {
            InitializeComponent();
            skupinaCombobox.SelectedIndex = 0;
            předmětComboBox.SelectedIndex = 0;
        }

        private void uložitButton_Click(object sender, RoutedEventArgs e)
        {
            if (názevTextBox.Text != "" && popisTextBox.Text != "" && splnitDoDatePicker.SelectedDate != null)
            {
                int skupina;
                switch (skupinaCombobox.SelectedIndex)
                {
                    case 0:
                        skupina = 1;
                        break;
                    case 1:
                        skupina = 2;
                        break;
                    default:
                        skupina = 0;
                        break;
                }
                Test = new Test(názevTextBox.Text, popisTextBox.Text, (předměty)Enum.Parse(typeof(předměty), ((ComboBoxItem)předmětComboBox.SelectedItem).Content.ToString()), skupina, (DateTime)splnitDoDatePicker.SelectedDate);
                this.Close();
            }
            else
            {
                MessageBox.Show("Některá z požadovaných polí nejsou vyplněna!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
