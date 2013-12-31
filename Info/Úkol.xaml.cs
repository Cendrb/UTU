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
using System.IO;

namespace Info
{
    /// <summary>
    /// Interakční logika pro Úkol.xaml
    /// </summary>
    public partial class Úkol : Window, IComparable<Úkol>
    {
        public string Popis { get; private set; }
        public string Nadpis { get; private set; }
        public DateTime Termín { get; private set; }
        public ListBoxItem AktuálníListBI { get; private set; }
        public int Skupina { get; private set; }
        public předměty Předmět { get; private set; }

        private bool hotovo = false;
        public bool Hotovo
        {
            get
            {
                return hotovo;
            }
            set
            {
                hotovo = value;
                if (hotovo)
                {
                    AktuálníListBI.Content = String.Format("{0} {1} - {2} - dokončeno", Nadpis, Předmět.ToString(), Termín.ToShortDateString());
                    AktuálníListBI.Foreground = new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    AktuálníListBI.Content = String.Format("{0} {1} - {2}", Nadpis, Předmět.ToString(), Termín.ToShortDateString());
                    AktuálníListBI.Foreground = new SolidColorBrush(Colors.Black);
                }
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Úkoly", this.GetId() + ".dat")))
                {
                    sw.Write(Hotovo);
                }
            }
        }

        public Úkol(string nadpisÚkolu, string popisÚkolu, předměty předmět, int skupina, DateTime splnitDo)
        {
            InitializeComponent();
            //Nastavení proměnných
            Termín = splnitDo;
            Popis = popisÚkolu;
            Nadpis = nadpisÚkolu;
            Skupina = skupina;
            Předmět = předmět;

            //Načtení hodnoty Hotovo ze souboru
            if (File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Úkoly", this.GetId() + ".dat")))
                using (StreamReader sr = new StreamReader(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Úkoly", this.GetId() + ".dat")))
                {
                    hotovo = Convert.ToBoolean(sr.ReadLine());   
                }

            //ListBoxItem + události
            AktuálníListBI = new ListBoxItem();
            if (Hotovo)
            {
                hotovoCheckBox.IsChecked = true;
                AktuálníListBI.Content = String.Format("{0} {1} - {2} - dokončeno", Nadpis, Předmět.ToString(), Termín.ToShortDateString());
                AktuálníListBI.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                hotovoCheckBox.IsChecked = false;
                AktuálníListBI.Content = String.Format("{0} {1} - {2}", Nadpis, Předmět.ToString(), Termín.ToShortDateString());
            }
            AktuálníListBI.MouseDoubleClick += AktuálníListBI_MouseDoubleClick;
            this.Closing += Úkol_Closing;

            //Nastavení ovládacích prvků
            nadpisLabel.Content = Nadpis;
            doLabel.Content = Termín.ToShortDateString();
            předmětLabel.Content = Předmět.ToString();
            if (skupina == 1)
                skupinaLabel.Content = "1.";
            else if (skupina == 0)
                skupinaLabel.Content = "1. a 2.";
            else
                skupinaLabel.Content = "2.";
            popisTextBox.Text = Popis;
            this.Title = String.Format("{0} {1}", Nadpis, předmět.ToString()); ;
        }

        private void Úkol_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            e.Cancel = true;
        }
        private void AktuálníListBI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ShowDialog();
        }
        int IComparable<Úkol>.CompareTo(Úkol other)
        {
            if (this.Termín == other.Termín)
                return 0;
            if (this.Termín > other.Termín)
                return 1;
            return -1;
        }

        private void hotovoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Hotovo = true;
        }

        private void hotovoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Hotovo = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string GetId()
        {
            StringBuilder forInt = new StringBuilder();
            foreach (char ch in Předmět.ToString())
            {
                forInt.Append((int)ch);
            }
            foreach (char ch in Nadpis)
            {
                forInt.Append((int)ch);
            }
            foreach (char ch in Termín.ToShortDateString())
            {
                forInt.Append((int)ch);
            }
            foreach (char ch in Popis)
            {
                forInt.Append((int)ch);
            }
            if (forInt.Length > 100)
                return forInt.ToString().Remove(100);
            else
                return forInt.ToString();
        }
    }
}
