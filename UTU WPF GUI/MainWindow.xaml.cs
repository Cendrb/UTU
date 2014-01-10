using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UTU_Class_Library;
using UTU_WPF_GUI;
using System.Threading;
using System.ComponentModel;
using System.Deployment.Application;

namespace Info
{
    public partial class MainWindow : Window
    {
        private DataLoader dataLoader;
        private Settings settings;
        private Database PrimaryDataSource;
        private string password;

        public int Group { get; set; }
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                //initialize folders
                Tools.InitializeFolders();

                //set password
                password = "suprakindrlo";

                //initialize variables
                Group = 1;

                //initialize dataloader
                dataLoader = new DataLoader(password);

                //initialize instances
                settings = new Settings(this, () => showInfo(Group, PrimaryDataSource));

                //initialize events
                Tools.DebugMessage += Console.WriteLine;
                Tools.DebugMessage += writeToLog;
                Tools.ImportantMessage += showInInfobar;
                Tools.ImportantMessage += writeToLog;
                dataLoader.DebugMessage += Console.WriteLine;
                dataLoader.DebugMessage += writeToLog;
                dataLoader.ImportantMessage += showInInfobar;
                dataLoader.ImportantMessage += writeToLog;
                dataLoader.Error += showInInfobar;
                dataLoader.Error += writeToLog;
                dataLoader.Error += sendReport;
                settings.ImportantMessage += showInInfobar;
                settings.ImportantMessage += writeToLog;
                settings.DebugMessage += Console.WriteLine;
                settings.DebugMessage += writeToLog;
                settings.Error += showInInfobar;
                settings.Error += writeToLog;
                settings.Error += sendReport;

                using (StreamWriter logStreamWriter = new StreamWriter(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log")))
                {
                    logStreamWriter.WriteLine();
                    logStreamWriter.WriteLine(String.Format("Spuštění programu UTU dne: {0}, s verzí operačního systému: {1}, s následujícím počtem procesorů (jader): {2}, s následující verzí prostředí: {3}, ve složce: {4}", DateTime.Now.ToString(), Environment.OSVersion, Environment.ProcessorCount, Environment.Version, Environment.CurrentDirectory));
                }

                //Nastavení odchytávání kláves
                KeyEventHandler KEH = new KeyEventHandler((sender, keyArgs) => getKeyboardKey(keyArgs));
                Keyboard.AddKeyDownHandler(this, KEH);

                reset();
                initializeTodayLabel();
            }
            catch (Exception e)
            {
                showInInfobar("VAROVÁNÍ: Došlo k neočekávané vyjímce - program nemusí správně fungovat");
                MessageBox.Show("VAROVÁNÍ: Došlo k neočekávané vyjímce - program nemusí správně fungovat (" + e.Message + ")", "FEKAL ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                BackgroundWorker reporter = new BackgroundWorker();
                reporter.DoWork += (x, y) => sendReport("Neočekávaná vyjímka: " + e.Message + "\nVlákno: " + Thread.CurrentThread.Name);
                reporter.RunWorkerAsync();
            }
        }
        private void reset()
        {
            //initialize datasource
            dataLoader.LoadFromFTP(
                delegate(Database database)
                {
                    PrimaryDataSource = database;
                    settings.LoadSettings();
                    showInfo(Group, PrimaryDataSource);
                }, true);
        }
        private void initializeTodayLabel()
        {
            daysInWeek day = daysInWeek.Neděle;
            day += ((int)DateTime.Now.DayOfWeek);
            dnesLabel.Content = String.Format("Dnes je {1} {0}", DateTime.Now.ToLongDateString(), day.ToString().ToLower());
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            settings.SaveData();
            //sendReport("Application Closed");
            Application.Current.Shutdown();
        }
        private void nápovědaKAplikaciMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Help nápověda = new Help();
            nápověda.ShowDialog();
        }
        private void oAplikaciMenuItem_Click(object sender, RoutedEventArgs e)
        {
            About oAplikaci = new About();
            oAplikaci.ShowDialog();
        }
        /// <summary>
        /// Zobrazí informace ze zadaného zdroje platné pro danou skupinu
        /// </summary>
        /// <param name="skupina">Skupina, pro kterou budou data zobrazena</param>
        /// <param name="dataSource">Zdroj informací pro zobrazení</param>
        private void showInfo(int skupina, Database dataSource)
        {
            //Pole událostí
            událostiListBox.Items.Clear();
            List<Events> events = dataSource.Events;
            events.Sort();

            IEnumerable<Events> vyhovujícíUdálosti = from událost in events
                                                     where DateTime.Now <= událost.To
                                                     select událost;
            foreach (Events eventX in vyhovujícíUdálosti)
            {
                EventWindow EW = new EventWindow(eventX);
                ListBoxItem LBI = new ListBoxItem();
                LBI.Content = eventX.Name + " - " + eventX.From.ToShortDateString();
                LBI.MouseDoubleClick += (x, y) => EW.ShowDialog();
                událostiListBox.Items.Add(LBI);
            }

            //Pole testů
            testyListBox.Items.Clear();
            List<Exams> exams = dataSource.Exams;
            exams.Sort();

            if (settings.ShowOdloženéTesty)
            {
                IEnumerable<Exams> vyhovujícíTesty = from test in exams
                                                     where test.Group == 0 || test.Group == skupina
                                                     where DateTime.Now <= test.Date
                                                     select test;
                foreach (Exams test in vyhovujícíTesty)
                {
                    ExamWindow EW = new ExamWindow(test);
                    ListBoxItem LBI = new ListBoxItem();
                    if (EW.Done)
                    {
                        LBI.Content = test.Name + " " + test.Subject + " - " + test.Date.ToShortDateString() + " - odloženo";
                        LBI.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                    else
                        LBI.Content = test.Name + " " + test.Subject + " - " + test.Date.ToShortDateString();
                    LBI.MouseDoubleClick += (x, y) => EW.ShowDialog();
                    testyListBox.Items.Add(LBI);
                }
            }
            else
            {
                IEnumerable<Exams> vyhovujícíTesty = from test in exams
                                                     where test.Group == 0 || test.Group == skupina
                                                     where DateTime.Now <= test.Date
                                                     select test;
                foreach (Exams test in vyhovujícíTesty)
                {
                    ExamWindow EW = new ExamWindow(test);
                    ListBoxItem LBI = new ListBoxItem();
                    LBI.Content = test.Name + " " + test.Subject + " - " + test.Date.ToShortDateString();
                    LBI.MouseDoubleClick += (x, y) => EW.ShowDialog();
                    if (!EW.Done)
                        testyListBox.Items.Add(LBI);
                }
            }


            //Pole úkolů
            úkolyListBox.Items.Clear();
            List<Tasks> tasks = dataSource.Tasks;
            tasks.Sort();

            if (settings.ShowDokončenéÚkoly)
            {
                IEnumerable<Tasks> vyhovujícíÚkoly = from úkol in tasks
                                                     where úkol.Group == 0 || úkol.Group == skupina
                                                     where DateTime.Now <= úkol.Date
                                                     select úkol;
                foreach (Tasks úkol in vyhovujícíÚkoly)
                {
                    TaskWindow EW = new TaskWindow(úkol);
                    ListBoxItem LBI = new ListBoxItem();
                    if (EW.Done)
                    {
                        LBI.Content = úkol.Name + " " + úkol.Subject + " - " + úkol.Date.ToShortDateString() + " - dokončeno";
                        LBI.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                    else
                        LBI.Content = úkol.Name + " " + úkol.Subject + " - " + úkol.Date.ToShortDateString();
                    LBI.MouseDoubleClick += (x, y) => EW.ShowDialog();
                    testyListBox.Items.Add(LBI);
                }
            }
            else
            {
                IEnumerable<Tasks> vyhovujícíTesty = from úkol in tasks
                                                     where úkol.Group == 0 || úkol.Group == skupina
                                                     where DateTime.Now <= úkol.Date
                                                     select úkol;
                foreach (Tasks úkol in vyhovujícíTesty)
                {
                    TaskWindow EW = new TaskWindow(úkol);
                    ListBoxItem LBI = new ListBoxItem();
                    LBI.Content = úkol.Name + " " + úkol.Subject + " - " + úkol.Date.ToShortDateString();
                    LBI.MouseDoubleClick += (x, y) => EW.ShowDialog();
                    if (!EW.Done)
                        úkolyListBox.Items.Add(LBI);
                }
            }
        }
        private void výběrSkupinyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index;
            index = (sender as ComboBox).SelectedIndex;
            if (0 <= index)
            {
                showInfo(index + 1, PrimaryDataSource);
                Group = index + 1;
            }
        }
        private void resetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            reset();
        }
        private void getKeyboardKey(KeyEventArgs keyArgs)
        {
            switch (keyArgs.Key.ToString())
            {
                case "F5":
                    reset();
                    break;
            }

        }
        private void nastaveníAplikaceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            settings.ShowDialog();
        }
        private void showInInfobar(string message)
        {
            statusBar.Items.Clear();
            statusBar.Items.Add(message);
        }
        private void writeToLog(string message)
        {
            using (StreamWriter logStreamWriter = new StreamWriter(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log"), true))
            {
                logStreamWriter.WriteLine(message);
            }
        }
        private void sendReport(string message)
        {
            //Send Gmail
            MailAddress fromAddress = new MailAddress("adisinfoapp@gmail.com", "UTU Report");
            MailAddress toAddress = new MailAddress("cendrb@gmail.com", "Developer");
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, password),
            };
            using (MailMessage mailMessage = new MailMessage(fromAddress, toAddress))
            {
                mailMessage.Subject = "UTU Report " + DateTime.Now.ToString();
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    Version myVersion;
                    myVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    string verze = String.Format("{0}.{1}.{2}.{3}", myVersion.Major, myVersion.Minor, myVersion.Build, myVersion.Revision);
                    mailMessage.Body = String.Format("Zpráva: {0}\nDalší informace:\n{1}", message, String.Format("Spuštění programu UTU dne: {0}\ns verzí operačního systému: {1}\ns následujícím počtem procesorů (jader): {2}\ns následující verzí prostředí: {3}\nve složce: {4}\nVerze UTU: {5}", DateTime.Now.ToString(), Environment.OSVersion, Environment.ProcessorCount, Environment.Version, Environment.CurrentDirectory, verze));
                }
                else
                {
                    mailMessage.Body = String.Format("Zpráva: {0}\nDalší informace:\n{1}", message, String.Format("Spuštění programu UTU dne: {0}\ns verzí operačního systému: {1}\ns následujícím počtem procesorů (jader): {2}\ns následující verzí prostředí: {3}\nve složce: {4}", DateTime.Now.ToString(), Environment.OSVersion, Environment.ProcessorCount, Environment.Version, Environment.CurrentDirectory));
                }
                mailMessage.Attachments.Add(new Attachment(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log")));
                smtp.Send(mailMessage);
            }
            Console.WriteLine("Zpráva byla odeslána");
        }
        private void sendReport(string message, string subject)
        {
            //Send Gmail
            MailAddress fromAddress = new MailAddress("adisinfoapp@gmail.com", "UTU Report");
            MailAddress toAddress = new MailAddress("cendrb@gmail.com", "Developer");
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, password),
            };
            using (MailMessage mailMessage = new MailMessage(fromAddress, toAddress))
            {
                mailMessage.Subject = "UTU Report - " + subject + " - " + DateTime.Now.ToString();
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    Version myVersion;
                    myVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    string verze = String.Format("{0}.{1}.{2}.{3}", myVersion.Major, myVersion.Minor, myVersion.Build, myVersion.Revision);
                    mailMessage.Body = String.Format("Zpráva: {0}\nDalší informace:\n{1}", message, String.Format("Spuštění programu UTU dne: {0}\ns verzí operačního systému: {1}\ns následujícím počtem procesorů (jader): {2}\ns následující verzí prostředí: {3}\nve složce: {4}\nVerze UTU: {5}", DateTime.Now.ToString(), Environment.OSVersion, Environment.ProcessorCount, Environment.Version, Environment.CurrentDirectory, verze));
                }
                else
                {
                    mailMessage.Body = String.Format("Zpráva: {0}\nDalší informace:\n{1}", message, String.Format("Spuštění programu UTU dne: {0}\ns verzí operačního systému: {1}\ns následujícím počtem procesorů (jader): {2}\ns následující verzí prostředí: {3}\nve složce: {4}", DateTime.Now.ToString(), Environment.OSVersion, Environment.ProcessorCount, Environment.Version, Environment.CurrentDirectory));
                }
                mailMessage.Attachments.Add(new Attachment(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Log", "logStatusBaru.log")));
                smtp.Send(mailMessage);
            }
            Console.WriteLine("Zpráva byla odeslána");
        }
        private void Update()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {
                    MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (!info.IsUpdateRequired)
                    {
                        MessageBoxResult mbr = MessageBox.Show("Byla nalezena aktualizace programu UTU. Přejete si ji stáhnout?", "Aktualizace k dispozici", MessageBoxButton.YesNo);
                        if (mbr == MessageBoxResult.No)
                        {
                            doUpdate = false;
                        }
                    }
                    if (doUpdate)
                    {
                        try
                        {
                            ad.Update();
                            MessageBox.Show("Aplikace bude ukončena a aktualizována...\nPo dalším spuštění budete mít již nejnovější verzi");
                            Application.Current.Shutdown();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            MessageBox.Show("Nelze nainstalovat nejnovější verzi aplikace\n\nOvěřte, že jste připojeni k interneu a zkuste to znovu. Error: " + dde);
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Používáte nejnovější verzi aplikace UTU School Helper");
                }
            }
        }
        private void posledníVerzeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void odeslatNázor_Click(object sender, RoutedEventArgs e)
        {
            Report report = new Report(sendReport);
            report.Show();
        }
    }

}
