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

namespace UTU_WPF_GUI
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        Action<string, string> report;
        public Report(Action<string, string> report)
        {
            this.report = report;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            report(textZprávyTextBox.Text, předmětTextBox.Text);
            this.Close();
        }

        private void textZprávyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textZprávyTextBox.Text == "Sem napište svou připomínku/nápad/zprávu/chybu")
                textZprávyTextBox.Text = "";
        }

        private void předmětTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (předmětTextBox.Text == "Sem zadejte předmět svojí zprávy")
                předmětTextBox.Text = "";
        }
    }
}
