using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Info
{
    public partial class MainWindow : Window
    {
        private delegátProStatusBar zobrazVBaru;
        private delegátProObnoveníInformací obnovInformace;
        private string password;
        private Rozvrh rozvrh;
        private Databáze databáze = null;
        private Nastavení nastavení;
        public int Skupina { get; set; }
        private OdeslatNázor odeslatNázor = new OdeslatNázor();
        public MainWindow()
        {
            this.password = "suprakindrlo";
            InitializeComponent();

            //Vytvoření složek pro data aplikace + logu
            try
            {
                //Vytvoření složek pro data jednotlivých datových složek
                if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info")))
                    Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info"));
                if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log")))
                    Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log"));
                if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Testy")))
                    Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Testy"));
                if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Úkoly")))
                    Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Úkoly"));
                if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Události")))
                    Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Události"));
                if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze")))
                    Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze"));
                if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Nastavení")))
                    Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Nastavení"));
            }
            catch (Exception e)
            {
                zobrazVBaru("Chyba při vytváření složek aplikace: " + e.Message);
            }

            //Vytvoření hlavičky pro logStreamWriter// + přepsání starého logu na nové jméno
            //bool freeFileFound = false;
            //int id = 0;
            //if (!File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log")))
            //    File.Create(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log"));

            //while (!freeFileFound)
            //{
            //    freeFileFound = !File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru_" + id + ".log"));
            //    if (freeFileFound)
            //        File.Move(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log"), System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru_" + id + ".log"));
            //    id++;
            //}

            using (StreamWriter logStreamWriter = new StreamWriter(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log")))
            {
                logStreamWriter.WriteLine();
                logStreamWriter.WriteLine(String.Format("Spuštění programu Info dne: {0}, na počítači jménem: {1}, s verzí operačního systému: {2}, s následujícím počtem procesorů (jader): {3}, na účtu uživatele jménem: {4}, s následující verzí prostředí: {5}, ve složce: {6}", DateTime.Now.ToString(), Environment.MachineName, Environment.OSVersion, Environment.ProcessorCount, Environment.UserName, Environment.Version, Environment.CurrentDirectory));
            }
            
            //Přidání metod do delegátů
            obnovInformace += zobrazInfoAdaptér;
            zobrazVBaru += zobrazVBaruMetoda;

            //Inicializace třídy pro výběr témat
            ThemeChooser themeChooser = new ThemeChooser(this);

            //Inicializace tříd s delegáty
            nastavení = new Nastavení(obnovInformace, themeChooser, this);
            databáze = new Databáze(zobrazVBaru, password, obnovInformace);

            //Nastavení odchytávání kláves
            KeyEventHandler KEH = new KeyEventHandler((sender, keyArgs) => getKeyboardKey(keyArgs));
            Keyboard.AddKeyDownHandler(this, KEH);

            //Připojení k databázi + Resetování hodnot
            Resetuj();

            //Nastavení ukazovače skupin
            výběrSkupinyComboBox.SelectedIndex = Skupina - 1;

        }

        private void Resetuj()
        {
            databáze.PřipojAStáhni();
            //Ve vývoji
            rozvrh = new Rozvrh(databáze);

            dnyVTýdnu den = dnyVTýdnu.Neděle;
            den += ((int)DateTime.Now.DayOfWeek);
            dnesLabel.Content = String.Format("Dnes je {1} {0}", DateTime.Now.ToLongDateString(), den.ToString().ToLower());
            zobrazInfo(Skupina);
            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            nastavení.uložData();
            Application.Current.Shutdown();
        }
        private void nápovědaKAplikaciMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Nápověda nápověda = new Nápověda();
            nápověda.ShowDialog();
        }
        private void marybuttonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Nezavřitelné nz = new Nezavřitelné();
            nz.Show();
        }
        private void oAplikaciMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OAplikaci oAplikaci = new OAplikaci();
            oAplikaci.ShowDialog();
        }
        private void zobrazInfoAdaptér()
        {
            zobrazInfo(Skupina);
        }
        private void zobrazInfo(int skupina)
        {
            try
            {
                //Pole událostí
                událostiListBox.Items.Clear();
                databáze.Events.Sort();

                IEnumerable<Událost> vyhovujícíUdálosti = from událost in databáze.Events
                                                          where DateTime.Now < událost.Do
                                                          select událost;
                foreach (Událost událost in vyhovujícíUdálosti)
                {
                        událostiListBox.Items.Add(událost.AktuálníListBI);
                }

                //Pole úkolů
                úkolyListBox.Items.Clear();
                databáze.Tasks.Sort();
                if (Nastavení.ZobrazDokončenéÚkoly)
                {
                    IEnumerable<Úkol> vyhovujícíÚkoly = from úkol in databáze.Tasks
                                                        where úkol.Skupina == 0 || úkol.Skupina == skupina
                                                        where DateTime.Now < úkol.Termín
                                                        select úkol;
                    foreach (Úkol úkol in vyhovujícíÚkoly)
                    {
                        úkolyListBox.Items.Add(úkol.AktuálníListBI);
                    }
                }
                else
                {
                    IEnumerable<Úkol> vyhovujícíÚkoly = from úkol in databáze.Tasks
                                                        where úkol.Skupina == 0 || úkol.Skupina == skupina
                                                        where DateTime.Now < úkol.Termín
                                                        where !úkol.Hotovo
                                                        select úkol;
                    foreach (Úkol úkol in vyhovujícíÚkoly)
                    {
                        úkolyListBox.Items.Add(úkol.AktuálníListBI);
                    }
                }

                //Pole testů
                testyListBox.Items.Clear();
                databáze.Exams.Sort();
                if (Nastavení.ZobrazOdloženéTesty)
                {
                    IEnumerable<Test> vyhovujícíTesty = from test in databáze.Exams
                                                        where test.Skupina == 0 || test.Skupina == skupina
                                                        where DateTime.Now < test.Termín
                                                        select test;
                    foreach (Test test in vyhovujícíTesty)
                    {
                            testyListBox.Items.Add(test.AktuálníListBI);
                    }
                }
                else
                {
                    IEnumerable<Test> vyhovujícíTesty = from test in databáze.Exams
                                                        where test.Skupina == 0 || test.Skupina == skupina
                                                        where DateTime.Now < test.Termín
                                                        where !test.Odloženo
                                                        select test;
                    foreach (Test test in vyhovujícíTesty)
                    {
                        testyListBox.Items.Add(test.AktuálníListBI);
                    }
                }
            }
            catch (Exception e)
            {
                zobrazVBaru("Chyba při zobrazování událostí/testů/úkolů: " + e.Message);
            }
        }
        private void výběrSkupinyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index;
            index = (sender as ComboBox).SelectedIndex;
            if (0 <= index)
            {
                zobrazInfo(index + 1);
                Skupina = index + 1;
            }
        }
        private void resetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Resetuj();
        }
        private void odeslatNázorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                odeslatNázor.ShowDialog();
            }
            catch (Exception exception)
            {
                zobrazVBaru("Zprávu se nepodařilo odeslat: " + exception.Message);
            }
        }
        private void getKeyboardKey(KeyEventArgs keyArgs)
        {
            switch (keyArgs.Key.ToString())
            {
                case "F5":
                    resetMenuItem_Click(this, new RoutedEventArgs());
                    break;
            }

        }
        private void nastaveníAplikaceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            nastavení.ShowDialog();
        }
        private void zobrazVBaruMetoda(string zpráva)
        {
            statusBar.Items.Clear();
            statusBar.Items.Add(zpráva);
            using (StreamWriter logStreamWriter = new StreamWriter(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log"), true))
            {
                logStreamWriter.WriteLine(zpráva);
            }
            
        }

        private void nastaveníTématu_Click(object sender, RoutedEventArgs e)
        {
            ThemeChooser vybratTéma = new ThemeChooser(this);
            vybratTéma.ShowDialog();
        }

    }
    
}
