using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace Info
{
    /// <summary>
    /// Interakční logika pro ThemeChooser.xaml
    /// </summary>
    public partial class ThemeChooser : Window
    {
        private MainWindow okno;
        public ThemeChooser(MainWindow okno)
        {
            InitializeComponent();
            this.okno = okno;
        }
        public void NastavTéma(témata téma)
        {
            switch (téma)
            {
                case témata.původní:
                    nastavPůvodní();
                    break;
                case témata.modré:
                    nastavModré();
                    break;
            }
        }
        private void nastavPůvodní()
        {
            okno.Style = (Style)okno.FindResource("stylPůvodní");
            okno.dnesLabel.Style = (Style)okno.FindResource("stylPůvodní");
            okno.testyGroupBox.Style = (Style)okno.FindResource("stylPůvodní");
            okno.úkolyGroupBox.Style = (Style)okno.FindResource("stylPůvodní");
            okno.událostiGroupBox.Style = (Style)okno.FindResource("stylPůvodní");
            okno.menu.Style = (Style)okno.FindResource("stylPůvodní");
            okno.výběrSkupinyComboBox.Style = (Style)okno.FindResource("stylPůvodní");
            okno.testyListBox.Style = (Style)okno.FindResource("stylPůvodní");
            okno.úkolyListBox.Style = (Style)okno.FindResource("stylPůvodní");
            okno.událostiListBox.Style = (Style)okno.FindResource("stylPůvodní");
            okno.statusBar.Style = (Style)okno.FindResource("stylPůvodní");
            okno.nastaveníMenuItem.Style = (Style)okno.FindResource("stylPůvodní");
            okno.nápovědaMenuItem.Style = (Style)okno.FindResource("stylPůvodní");
        }
        private void nastavModré()
        {
            //XXX
        }
        private void nastavTmavý()
        {
            okno.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "kvarta.png")));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (původníradioButton.IsChecked.Value)
                nastavPůvodní();
            if (tmavéRadioButton.IsChecked.Value)
                nastavTmavý();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Dodělat ukládání posledních hodnot!
        }
    }
    [ValueConversion(typeof(string), typeof(témata))]
    public class PřevodníkTémat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, System.Globalization.CultureInfo culture)
        {
            témata téma = (témata)value;
            string převedeno = String.Empty;

            switch (téma)
            {
                case témata.původní:
                    převedeno = "Původní";
                    break;
                case témata.modré:
                    převedeno = "Modré";
                    break;
            }
            return převedeno;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = (string)value;
            témata převedeno = témata.původní;

            switch (text)
            {
                case "Původní":
                    převedeno = témata.původní;
                    break;
                case "Modré":
                    převedeno = témata.modré;
                    break;
            }
            return převedeno;
        }
    }
}
