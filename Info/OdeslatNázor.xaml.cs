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
using System.Net.Mail;
using System.Net;
using System.IO;

namespace Info
{
    /// <summary>
    /// Interakční logika pro OdeslatNázor.xaml
    /// </summary>
    public partial class OdeslatNázor : Window
    {
        private bool logFile = false;
        public OdeslatNázor()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            e.Cancel = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            sendMessage();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            logFile = false;
        }
        private void sendMessage()
        {
            MailAddress adresa = new MailAddress("cendrb@gmail.com");
            MailMessage zpráva;
            if (logFile)
            {
                string log;
                using (StreamReader reader = new StreamReader(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log")))
                {
                    log = reader.ReadToEnd();
                }
                if (log.Length > 50000)
                    log.Remove(50000);
                zpráva = new MailMessage("adisinfoapp@gmail.com", "cendrb@gmail.com", předmětTextBox.Text, textZprávyTextBox.Text + log);
            }
            else
            {
                zpráva = new MailMessage("adisinfoapp@gmail.com", "cendrb@gmail.com", předmětTextBox.Text, textZprávyTextBox.Text);
            }
            SmtpClient klient = new SmtpClient();
            klient.Host = "smtp.gmail.com";
            klient.Port = 465;
            klient.EnableSsl = true;
            //klient.Send(zpráva);
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            logFile = true;
        }
    }
}
