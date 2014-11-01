using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Npgsql;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace UTU_Class_Library
{
    public class DataLoader
    {
        private string password;
        private bool msSQLConnected = false;
        private DataClasses1DataContext msSQL;

        public event Action<string> Error;
        public event Action<string> ImportantMessage;
        public event Action<string> DebugMessage;

        private event Action<Database> dataFromFTPLoaded;

        private Database ftpDatabase, mySQLDatabase, msSQLDatabase, sqliteDatabase, pgDatabase, httpDatabase;

        public DataLoader(string password)
        {
            this.password = password;

            Error += delegate { };
            ImportantMessage += delegate { };
            DebugMessage += delegate { };

            dataFromFTPLoaded += delegate { };
        }


        /* Conflicts
        private void findAndRepairConflicts(Database from)
        {
            //EVENTS
            IEnumerable<double> eventsFromSql = from item in msSQL.Events
                                                select item.Id;
            List<Event> removedEvents = new List<Event>();
            foreach (Event item in from.Events)
            {
                DebugMessage("Porovnávám id: " + item.Id.ToString());
                if (eventsFromSql.Contains(item.Id))
                {
                    DebugMessage("Nalezeno shodné id - mažu záznam v kolekci nahrané ze souboru");
                    removedEvents.Add(item);
                }
            }
            foreach (Event remove in removedEvents)
                from.Events.Remove(remove);


            //TASKS
            IEnumerable<double> tasksFromSql = from item in msSQL.Tasks
                                               select item.Id;
            List<Tasks> removedTasks = new List<Tasks>();
            foreach (Tasks item in from.Tasks)
            {
                DebugMessage("Porovnávám id: " + item.Id.ToString());
                if (tasksFromSql.Contains(item.Id))
                {
                    DebugMessage("Nalezeno shodné id - mažu záznam v kolekci nahrané ze souboru");
                    removedTasks.Add(item);
                }
            }
            foreach (Tasks remove in removedTasks)
                from.Tasks.Remove(remove);


            //EXAMS
            IEnumerable<double> examsFromSql = from item in msSQL.Exams
                                               select item.Id;
            List<Exams> removedExams = new List<Exams>();
            foreach (Exams item in from.Exams)
            {
                DebugMessage("Porovnávám id: " + item.Id.ToString());
                if (examsFromSql.Contains(item.Id))
                {
                    DebugMessage("Nalezeno shodné id - mažu záznam v kolekci nahrané ze souboru");
                    removedExams.Add(item);
                }
            }
            foreach (Exams remove in removedExams)
                from.Exams.Remove(remove);

        }
         */

        public void LoadFromWebUsingHttpRequest(Action<Database> completed, bool forceReload)
        {
            if (forceReload)
            {
                loadFromWebUsingHttpRequest(completed);
            }

            if (httpDatabase == null)
            {
                loadFromWebUsingHttpRequest(completed);
            }
            else
            {
                completed(httpDatabase);
            }
        }

        private void loadFromWebUsingHttpRequest(Action<Database> completed)
        {
            WebRequest request = WebRequest.Create("http://utu.herokuapp.com/details.xml");
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();

            loadDataFromXMLFromWeb(completed, response.GetResponseStream());
        }
        private void loadDataFromXMLFromWeb(Action<Database> completed, Stream stream)
        {
            List<Task> Tasks = new List<UTU_Class_Library.Task>();
            List<Exam> Exams = new List<UTU_Class_Library.Exam>();
            List<Event> Events = new List<UTU_Class_Library.Event>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);

                XmlElement infoElement = (XmlElement)doc.GetElementsByTagName("utu")[0];
                XmlElement události = (XmlElement)infoElement.GetElementsByTagName("events")[0];
                XmlElement úkoly = (XmlElement)infoElement.GetElementsByTagName("tasks")[0];
                XmlElement testy = (XmlElement)infoElement.GetElementsByTagName("exams")[0];

                foreach (XmlNode událostNode in události)
                {
                    XmlElement událostElement = (XmlElement)událostNode;
                    string název;
                    string popis;
                    string datumZačátku;
                    string datumKonce;
                    string místo;
                    string id;

                    název = událostElement.GetAttribute("title");
                    popis = událostElement.GetAttribute("description");
                    datumZačátku = událostElement.GetAttribute("eventStart");
                    datumKonce = událostElement.GetAttribute("eventEnd");
                    místo = událostElement.GetAttribute("location");
                    id = událostElement.GetAttribute("id");

                    Event událost = new Event();
                    událost.Name = název;
                    událost.Description = popis;
                    událost.From = DateTime.Parse(datumZačátku);
                    událost.To = DateTime.Parse(datumKonce);
                    událost.Place = místo;
                    událost.Id = int.Parse(id);
                    Events.Add(událost);
                }
                foreach (XmlNode testNode in testy)
                {
                    XmlElement testElement = (XmlElement)testNode;
                    string předmět;
                    int skupina;
                    string název;
                    string popis;
                    string splnitDo;
                    int id;

                    předmět = testElement.GetAttribute("subject");
                    název = testElement.GetAttribute("title");
                    popis = testElement.GetAttribute("description");
                    splnitDo = testElement.GetAttribute("date");
                    skupina = int.Parse(testElement.GetAttribute("group"));
                    id = int.Parse(testElement.GetAttribute("id"));

                    Exam test = new Exam();
                    test.Name = název;
                    test.Description = popis;
                    test.Date = DateTime.Parse(splnitDo);
                    test.Group = skupina;
                    test.Subject = předmět.ToString();
                    test.Id = id;
                    Exams.Add(test);
                }
                foreach (XmlNode úkolNode in úkoly)
                {
                    XmlElement úkolElement = (XmlElement)úkolNode;
                    string předmět;
                    int skupina;
                    string název;
                    string popis;
                    string splnitDo;
                    int id;

                    předmět = úkolElement.GetAttribute("subject");
                    název = úkolElement.GetAttribute("title");
                    popis = úkolElement.GetAttribute("description");
                    splnitDo = úkolElement.GetAttribute("date");
                    skupina = int.Parse(úkolElement.GetAttribute("group"));
                    id = int.Parse(úkolElement.GetAttribute("id"));

                    Task úkol = new Task();
                    úkol.Name = název;
                    úkol.Description = popis;
                    úkol.Date = DateTime.Parse(splnitDo);
                    úkol.Group = skupina;
                    úkol.Subject = předmět.ToString();
                    úkol.Id = id;
                    Tasks.Add(úkol);
                }
            }
            catch (Exception e)
            {
                Error("Chyba při čtení souboru. Restartujte aplikaci a kontaktujte autora. " + e.Message);
            }
            ImportantMessage("Data byla úspešně nahrána");
            httpDatabase = new Database(Events, Tasks, Exams);
            completed(httpDatabase);
        }
    }
}
