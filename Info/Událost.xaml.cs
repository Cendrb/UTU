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

namespace Info
{
    /// <summary>
    /// Interakční logika pro Událost.xaml
    /// </summary>
    public partial class Událost : Window, IComparable<Událost>
    {
        public DateTime Od { get; private set; }
        public DateTime Do { get; private set; }
        public string Popis { get; private set; }
        public string Nadpis { get; private set; }
        public string Místo { get; private set; }
        public ListBoxItem AktuálníListBI { get; private set; }

        public Událost(string nadpisUdálosti, string popisUdálosti, DateTime začátekUdálosti, string místoKonání = "Vězení Písnická", DateTime konecUdálosti = default(DateTime))
        {
            InitializeComponent();

            //nastavení proměnných
            Popis = popisUdálosti;
            Od = začátekUdálosti;
            Místo = místoKonání;
            Nadpis = nadpisUdálosti;
            if (konecUdálosti == default(DateTime))
            {
                Do = začátekUdálosti;
            }
            else
            {
                Do = konecUdálosti;
            }

            //ListBoxItem + události
            AktuálníListBI = new ListBoxItem();
            AktuálníListBI.Content = nadpisUdálosti + " - " +  Od.ToShortDateString();
            AktuálníListBI.MouseDoubleClick += AktuálníListBI_MouseDoubleClick;
            this.Closing += Událost_Closing;


            //nastavení ovládacích prvků v okně
            if (Od.Hour == 0 || Do.Hour == 0)
            {
                odLabel.Content = Od.ToShortDateString();
                doLabel.Content = Do.ToShortDateString();
            }
            else
            {
                odLabel.Content = Od.ToString();
                doLabel.Content = Do.ToString();
            }
            kdeLabel.Content = Místo;
            popisTextBox.Text = Popis;
            nadpisLabel.Content = Nadpis;
            this.Title = Nadpis;
        }

        void Událost_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            e.Cancel = true;
        }

        private void AktuálníListBI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.ShowDialog();
        }


        int IComparable<Událost>.CompareTo(Událost other)
        {
            if (this.Od == other.Od)
                return 0;
            if (this.Od > other.Od)
                return 1;
            return -1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public string GetId()
        {
            StringBuilder forInt = new StringBuilder();
            foreach (char ch in Nadpis)
            {
                forInt.Append((int)ch);
            }
            foreach (char ch in Od.ToString())
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
