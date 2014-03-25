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
using System.Xml;
using System.IO;

namespace Info
{
    /// <summary>
    /// Interakční logika pro Nastavení.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public bool ShowDokončenéÚkoly { get; private set; }
        public bool ShowOdloženéTesty { get; private set; }

        public event Action<string> DebugMessage;
        public event Action<string> ImportantMessage;
        public event Action<string> Error;

        public event Action Refresh;

        private MainWindow window;

        public Settings(MainWindow window, Action refresh)
        {
            ImportantMessage += delegate { };
            Error += delegate { };
            Refresh += refresh;
            this.window = window;
            InitializeComponent();
            ShowDokončenéÚkoly = false;
            ShowOdloženéTesty = false;
        }

        private void dokončenéÚkolyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ShowDokončenéÚkoly = true;
        }
        private void dokončenéÚkolyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowDokončenéÚkoly = false;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void odloženéTestyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ShowOdloženéTesty = true;
        }

        private void odloženéTestyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowOdloženéTesty = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
            Refresh();
            this.Visibility = System.Windows.Visibility.Collapsed;
            e.Cancel = true;
        }

        public void LoadSettings()
        {
            if (!File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Nastavení", "nastaveni.dat")))
            {
                window.výběrSkupinyComboBox.SelectedIndex = 0;
                SaveData();
            }
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load((System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Nastavení", "nastaveni.dat")));
                XmlElement nastaveni = ((XmlElement)document.GetElementsByTagName("nastaveni")[0]);

                dokončenéÚkolyCheckBox.IsChecked = Boolean.Parse(nastaveni.GetAttribute("dokonceneUkoly"));
                odloženéTestyCheckBox.IsChecked = Boolean.Parse(nastaveni.GetAttribute("odlozeneTesty"));
                window.výběrSkupinyComboBox.SelectedIndex = int.Parse(nastaveni.GetAttribute("skupina")) - 1;
                window.Width = Double.Parse(nastaveni.GetAttribute("sirkaOkna"));
                window.Height = Double.Parse(nastaveni.GetAttribute("vyskaOkna"));
            }
            catch (XmlException e)
            {
                Error("Došlo k chybě při čtení ze souboru nastavení: " + e.Message);
            }
        }

        public void SaveData()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement nastavení = doc.CreateElement("nastaveni");
            XmlDeclaration deklarace = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(deklarace);
            nastavení.SetAttribute("skupina", window.Group.ToString());
            nastavení.SetAttribute("odlozeneTesty", ShowOdloženéTesty.ToString());
            nastavení.SetAttribute("dokonceneUkoly", ShowDokončenéÚkoly.ToString());
            nastavení.SetAttribute("sirkaOkna", window.Width.ToString());
            nastavení.SetAttribute("vyskaOkna", window.Height.ToString());
            doc.AppendChild(nastavení);
            doc.Save(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Nastavení", "nastaveni.dat"));
        }

    }
}
