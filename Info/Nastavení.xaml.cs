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
    public partial class Nastavení : Window
    {
        public static bool ZobrazDokončenéÚkoly { get; private set; }
        public static bool ZobrazOdloženéTesty { get; private set; }

        private ThemeChooser themeChooser;
        private MainWindow okno;

        private delegátProObnoveníInformací obnovZobrazenéInformace;

        public Nastavení(delegátProObnoveníInformací obnovZobrazenéInformace, ThemeChooser themeChooser, MainWindow okno)
        {
            this.okno = okno;
            InitializeComponent();
            this.themeChooser = themeChooser;
            this.obnovZobrazenéInformace = obnovZobrazenéInformace;
            ZobrazDokončenéÚkoly = false;
            ZobrazOdloženéTesty = false;

            if (!File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Nastavení", "nastaveni.dat")))
            {
                okno.výběrSkupinyComboBox.SelectedIndex = 0;
                uložData();
            }
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load((System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Nastavení", "nastaveni.dat")));
                XmlElement nastaveni = ((XmlElement)document.GetElementsByTagName("nastaveni")[0]);
                dokončenéÚkolyCheckBox.IsChecked = Boolean.Parse(nastaveni.GetAttribute("odlozeneTesty")); //TODO doděkalt synchronizaci hodnot zobrazených v GUI mezi hodnotami v proměnných proh´gramiu
                odloženéTestyCheckBox.IsChecked = Boolean.Parse(nastaveni.GetAttribute("dokonceneUkoly"));
                okno.výběrSkupinyComboBox.SelectedIndex = int.Parse(nastaveni.GetAttribute("skupina")) - 1;
                okno.Width = Double.Parse(nastaveni.GetAttribute("sirkaOkna"));
                okno.Height = Double.Parse(nastaveni.GetAttribute("vyskaOkna"));
            }
            catch (Exception e)
            {
                MessageBox.Show("Došlo k chybě při čtení ze souboru nastavení: " + e.Message, "CHYBA");
            }
        }

        private void dokončenéÚkolyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ZobrazDokončenéÚkoly = true;
        }

        private void dokončenéÚkolyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ZobrazDokončenéÚkoly = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void odloženéTestyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ZobrazOdloženéTesty = true;
        }

        private void odloženéTestyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ZobrazOdloženéTesty = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            obnovZobrazenéInformace();
            this.Visibility = System.Windows.Visibility.Collapsed;
            e.Cancel = true;
        }

        public void uložData()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement nastavení = doc.CreateElement("nastaveni");
            XmlDeclaration deklarace = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(deklarace);
            nastavení.SetAttribute("skupina", okno.Skupina.ToString());
            nastavení.SetAttribute("odlozeneTesty", ZobrazOdloženéTesty.ToString());
            nastavení.SetAttribute("dokonceneUkoly", ZobrazDokončenéÚkoly.ToString());
            nastavení.SetAttribute("sirkaOkna", okno.Width.ToString());
            nastavení.SetAttribute("vyskaOkna", okno.Height.ToString());
            doc.AppendChild(nastavení);
            doc.Save(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Nastavení", "nastaveni.dat"));
        }

    }
}
