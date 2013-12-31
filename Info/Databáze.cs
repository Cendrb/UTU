using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace Info
{
    public class Databáze
    {
        //username, password, server
        private delegátProStatusBar zobrazVBaru;
        private delegátProObnoveníInformací obnovInformace;
        public List<Událost> Events { get; private set; }
        public List<Úkol> Tasks { get; private set; }
        public List<Test> Exams { get; private set; }
        private string password;
        public bool Připojeno { get; private set; }

        public Databáze(delegátProStatusBar zobrazVBaru, string password, delegátProObnoveníInformací obnovInformace)
        {
            try
            {
                this.password = password;
                this.zobrazVBaru = zobrazVBaru;
                this.obnovInformace = obnovInformace;

                //Inicializace kolekcí
                Tasks = new List<Úkol>();
                Exams = new List<Test>();
                Events = new List<Událost>();

            }
            catch (Exception e)
            {
                zobrazVBaru("Chyba při inicializaci databáze: " + e.Message);
            }
        }
        public void PřipojAStáhni()
        {
            zobrazVBaru("Zahájeno připojování k databázi");
            BackgroundWorker pracovníVlákno = new BackgroundWorker();
            pracovníVlákno.DoWork += (x, y) => Nástroje.StáhniXml(password);
            pracovníVlákno.RunWorkerAsync();
            pracovníVlákno.RunWorkerCompleted += pracovníVlákno_RunWorkerCompleted;
        }

        private void pracovníVlákno_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            nahrajDataZXML();
            if (e.Error != null)
            {
                zobrazVBaru("Chyba při připojování k databázi. Zkontrolujte své internetové připojení a zkuste to znovu.");
            }
            else
            {
                obnovInformace();
                zobrazVBaru("Data byla úspěšně stažena.");
            }
        }
        private void nahrajDataZXML()
        {
            Tasks.Clear();
            Exams.Clear();
            Events.Clear();

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databaze.dat"));

            XmlElement infoElement = (XmlElement)doc.GetElementsByTagName("Info")[0];
            XmlElement události = (XmlElement)infoElement.GetElementsByTagName("Udalosti")[0];
            XmlElement úkoly = (XmlElement)infoElement.GetElementsByTagName("Ukoly")[0];
            XmlElement testy = (XmlElement)infoElement.GetElementsByTagName("Testy")[0];

            foreach (XmlNode událostNode in události)
            {
                XmlElement událostElement = (XmlElement)událostNode;
                string název;
                string popis;
                string datumZačátku;
                string datumKonce;
                string místo;

                název = událostElement.GetAttribute("nazev");
                popis = událostElement.GetAttribute("popis");
                datumZačátku = událostElement.GetAttribute("datumZacatku");
                datumKonce = událostElement.GetAttribute("datumKonce");
                místo = událostElement.GetAttribute("misto");

                Událost událost = new Událost(název, popis, DateTime.Parse(datumZačátku), místo, DateTime.Parse(datumKonce));
                Events.Add(událost);
            }
            foreach (XmlNode testNode in testy)
            {
                XmlElement testElement = (XmlElement)testNode;
                předměty předmět;
                int skupina;
                string název;
                string popis;
                string splnitDo;

                předmět = (předměty)Enum.Parse(typeof(předměty), testElement.GetAttribute("predmety"));
                název = testElement.GetAttribute("nazev");
                popis = testElement.GetAttribute("popis");
                splnitDo = testElement.GetAttribute("splnitDo");
                skupina = int.Parse(testElement.GetAttribute("skupina"));

                Test test = new Test(název, popis, předmět, skupina, DateTime.Parse(splnitDo));
                Exams.Add(test);
            }
            foreach (XmlNode úkolNode in úkoly)
            {
                XmlElement úkolElement = (XmlElement)úkolNode;
                předměty předmět;
                int skupina;
                string název;
                string popis;
                string splnitDo;

                předmět = (předměty)Enum.Parse(typeof(předměty), úkolElement.GetAttribute("predmety"));
                název = úkolElement.GetAttribute("nazev");
                popis = úkolElement.GetAttribute("popis");
                splnitDo = úkolElement.GetAttribute("splnitDo");
                skupina = int.Parse(úkolElement.GetAttribute("skupina"));

                Úkol úkol = new Úkol(název, popis, předmět, skupina, DateTime.Parse(splnitDo));
                Tasks.Add(úkol);
            }
        }
}
}