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
using System.Windows.Navigation;
using System.IO;
using System.Xml;
using Info;

namespace Admin_tool
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Událost> události = new List<Událost>();
        private List<Test> testy = new List<Test>();
        private List<Úkol> úkoly = new List<Úkol>();

        public MainWindow()
        {
            InitializeComponent();

            ////Události
            //události.Add(new Událost("Vražda Ivony", "Cílená eliminace paní učitelky Ivony Součkové", new DateTime(2013, 8, 11)));
            //události.Add(new Událost("Vražda Mařky", "Cílená eliminace paní profesorky Márie Halbrštátové", new DateTime(2013, 9, 21)));
            //události.Add(new Událost("Vražda Iriny", "Cílená eliminace paní učitelky Iriny Kobylkové", new DateTime(2013, 9, 10)));

            ////Úkoly
            //úkoly.Add(new Úkol("Pracovní sešit", "strana 65, cvičení 10", předměty.ČJL, 0, new DateTime(2013, 9, 15)));
            //úkoly.Add(new Úkol("Pracovní sešit", "strana 1 - 100", předměty.AJ, 1, new DateTime(2013, 9, 20)));
            //úkoly.Add(new Úkol("Učebnice", "strana 788, cvičení 11", předměty.AJ, 2, new DateTime(2013, 9, 10)));
            //úkoly.Add(new Úkol("Písnička", "Naučit se zasranou ŠRAČKU", předměty.NJ, 2, new DateTime(2013, 10, 30)));

            ////Testy
            //testy.Add(new Test("Písemná práce", "Rovnice", předměty.MA, 0, new DateTime(2013, 10, 12)));
            //testy.Add(new Test("Písemná práce", "Last shit", předměty.AJ, 2, new DateTime(2013, 10, 14)));
            //testy.Add(new Test("Písemná práce", "Scheisse", předměty.NJ, 1, new DateTime(2013, 10, 18)));
            //testy.Add(new Test("Písemná práce", "Hurry up!", předměty.AJ, 1, new DateTime(2013, 10, 5)));
            //testy.Add(new Test("Testík", "Obojživelníci", předměty.PŘÍ, 0, new DateTime(2013, 10, 30)));
        }

        private void událostButton_Click(object sender, RoutedEventArgs e)
        {
            UdálostWindow událost = new UdálostWindow();
            událost.ShowDialog();
            události.Add(událost.Událost);
        }

        private void úkolButton_Click(object sender, RoutedEventArgs e)
        {
            ÚkolWindow úkol = new ÚkolWindow();
            úkol.ShowDialog();
            úkoly.Add(úkol.Úkol);
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            TestWindow test = new TestWindow();
            test.ShowDialog();
            testy.Add(test.Test);
        }

        private void uložitButton_Click(object sender, RoutedEventArgs e)
        {
            bool chyba = false;
            try
            {
                Info.Nástroje.StáhniXml("suprakindrlo");
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databaze.dat"));
                XmlElement infoElement = (XmlElement)doc.GetElementsByTagName("Info")[0];
                XmlElement událostiElementy = (XmlElement)infoElement.GetElementsByTagName("Udalosti")[0];
                XmlElement úkolyElementy = (XmlElement)infoElement.GetElementsByTagName("Ukoly")[0];
                XmlElement testyElementy = (XmlElement)infoElement.GetElementsByTagName("Testy")[0];

                foreach (Událost událost in události)
                {
                    if (událostiElementy.GetElementsByTagName("UD" + událost.GetId())[0] == null)
                    {
                        XmlElement elementUdálosti = doc.CreateElement("UD" + událost.GetId());
                        elementUdálosti.SetAttribute("nazev", událost.Nadpis);
                        elementUdálosti.SetAttribute("popis", událost.Popis);
                        elementUdálosti.SetAttribute("datumZacatku", událost.Od.ToString());
                        elementUdálosti.SetAttribute("datumKonce", událost.Do.ToString());
                        elementUdálosti.SetAttribute("misto", událost.Místo);

                        událostiElementy.AppendChild(elementUdálosti);
                    }
                    else
                    {
                        MessageBox.Show("Položka: " + událost.Nadpis + " je již na seznamu.", "Duplicitní položka");
                    }
                }
                foreach (Test test in testy)
                {
                    if(testyElementy.GetElementsByTagName("TE" + test.GetId())[0] == null)
                    {
                    XmlElement elementTestu = doc.CreateElement("TE" + test.GetId());
                    elementTestu.SetAttribute("nazev", test.Nadpis);
                    elementTestu.SetAttribute("popis", test.Popis);
                    elementTestu.SetAttribute("splnitDo", test.Termín.ToString());
                    elementTestu.SetAttribute("skupina", test.Skupina.ToString());
                    elementTestu.SetAttribute("predmety", test.Předmět.ToString());

                    testyElementy.AppendChild(elementTestu);
                    }
                    else
                    {
                        MessageBox.Show("Položka: " + test.Nadpis + " - " + test.Předmět.ToString() + " je již na seznamu.", "Duplicitní položka");
                    }
                }
                foreach (Úkol úkol in úkoly)
                {
                    if (úkolyElementy.GetElementsByTagName("UK" + úkol.GetId())[0] == null)
                    {
                        XmlElement elementÚkolu = doc.CreateElement("UK" + úkol.GetId());
                        elementÚkolu.SetAttribute("nazev", úkol.Nadpis);
                        elementÚkolu.SetAttribute("popis", úkol.Popis);
                        elementÚkolu.SetAttribute("splnitDo", úkol.Termín.ToString());
                        elementÚkolu.SetAttribute("skupina", úkol.Skupina.ToString());
                        elementÚkolu.SetAttribute("predmety", úkol.Předmět.ToString());

                        úkolyElementy.AppendChild(elementÚkolu);
                    }
                    else
                    {
                        MessageBox.Show("Položka: " + úkol.Nadpis + " - " + úkol.Předmět.ToString() + " je již na seznamu.", "Duplicitní položka");
                    }
                }
                doc.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databazeAdmin.dat"));
                Info.Nástroje.NahrajXml("suprakindrlo");
            }
            catch (Exception ex)
            {
                chyba = true;
                MessageBox.Show(String.Format("Chyba při zapisování do souboru/stahování ze serveru!\n{0}\n{1}", ex.Message, ex.GetType().ToString()), "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (!chyba)
                MessageBox.Show("Data byla úspěšně zapsána!", "Výborně");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
