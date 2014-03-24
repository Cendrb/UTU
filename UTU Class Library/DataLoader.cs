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

        public void LoadFromFTP(Action<Database> completed)
        {
            if (ftpDatabase == null)
            {
                ImportantMessage("Stahování a zpracovávání souboru");
                BackgroundWorker downloadingThread = new BackgroundWorker();
                downloadingThread.DoWork += (x, y) => Tools.DownloadXML(password);
                downloadingThread.RunWorkerAsync();
                downloadingThread.RunWorkerCompleted += ((x, y) => loadDataFromXML(completed));
            }
            else
            {
                ImportantMessage("Data načtena");
                DebugMessage("Byl použit již dříve stažený soubor.");
                completed(ftpDatabase);
            }
        }
        public void LoadFromFTP(Action<Database> completed, bool forceReDownload)
        {
            if (forceReDownload)
            {
                ImportantMessage("Stahování a zpracovávání souboru");
                BackgroundWorker downloadingThread = new BackgroundWorker();
                downloadingThread.DoWork += (x, y) => Tools.DownloadXML(password);
                downloadingThread.RunWorkerAsync();
                downloadingThread.RunWorkerCompleted += ((x, y) => loadDataFromXML(completed));
            }
            else
            {
                if (ftpDatabase == null)
                {
                    ImportantMessage("Stahování a zpracovávání souboru");
                    BackgroundWorker downloadingThread = new BackgroundWorker();
                    downloadingThread.DoWork += (x, y) => Tools.DownloadXML(password);
                    downloadingThread.RunWorkerAsync();
                    downloadingThread.RunWorkerCompleted += ((x, y) => loadDataFromXML(completed));
                }
                else
                {
                    ImportantMessage("Data načtena");
                    DebugMessage("Byl použit již dříve stažený soubor.");
                    completed(ftpDatabase);
                }
            }
        }
        private void loadDataFromXML(Action<Database> completed)
        {
            List<Tasks> Tasks = new List<UTU_Class_Library.Tasks>();
            List<Exams> Exams = new List<UTU_Class_Library.Exams>();
            List<Event> Events = new List<UTU_Class_Library.Event>();
            try
            {
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

                    Event událost = new Event();
                    událost.Name = název;
                    událost.Description = popis;
                    událost.From = DateTime.Parse(datumZačátku);
                    událost.To = DateTime.Parse(datumKonce);
                    událost.Place = místo;
                    Events.Add(událost);
                }
                foreach (XmlNode testNode in testy)
                {
                    XmlElement testElement = (XmlElement)testNode;
                    subjects předmět;
                    int skupina;
                    string název;
                    string popis;
                    string splnitDo;

                    předmět = (subjects)Enum.Parse(typeof(subjects), testElement.GetAttribute("predmety"));
                    název = testElement.GetAttribute("nazev");
                    popis = testElement.GetAttribute("popis");
                    splnitDo = testElement.GetAttribute("splnitDo");
                    skupina = int.Parse(testElement.GetAttribute("skupina"));

                    Exams test = new Exams();
                    test.Name = název;
                    test.Description = popis;
                    test.Date = DateTime.Parse(splnitDo);
                    test.Group = skupina;
                    test.Subject = předmět.ToString();
                    Exams.Add(test);
                }
                foreach (XmlNode úkolNode in úkoly)
                {
                    XmlElement úkolElement = (XmlElement)úkolNode;
                    subjects předmět;
                    int skupina;
                    string název;
                    string popis;
                    string splnitDo;

                    předmět = (subjects)Enum.Parse(typeof(subjects), úkolElement.GetAttribute("predmety"));
                    název = úkolElement.GetAttribute("nazev");
                    popis = úkolElement.GetAttribute("popis");
                    splnitDo = úkolElement.GetAttribute("splnitDo");
                    skupina = int.Parse(úkolElement.GetAttribute("skupina"));

                    Tasks úkol = new Tasks();
                    úkol.Name = název;
                    úkol.Description = popis;
                    úkol.Date = DateTime.Parse(splnitDo);
                    úkol.Group = skupina;
                    úkol.Subject = předmět.ToString();
                    Tasks.Add(úkol);
                }
            }
            catch (Exception e)
            {
                Error("Chyba při čtení souboru. Restartujte aplikaci a kontaktujte autora. " + e.Message);
            }
            ImportantMessage("Data byla úspešně nahrána");
            ftpDatabase = new Database(Events, Tasks, Exams);
            completed(ftpDatabase);
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

        public void LoadFromPG(Action<Database> completed)
        {
            if (pgDatabase == null)
            {
                loadFromPG(completed);
            }
            else
            {
                completed(pgDatabase);
            }
        }
        public void LoadFromPG(Action<Database> completed, bool forceReLoad)
        {
            if (forceReLoad)
            {
                loadFromPG(completed);
            }
            else
            {
                if (pgDatabase == null)
                {
                    loadFromPG(completed);
                }
                else
                {
                    completed(pgDatabase);
                }
            }
        }

        private void loadFromPG(Action<Database> completed)
        {
            List<Tasks> Tasks = new List<UTU_Class_Library.Tasks>();
            List<Exams> Exams = new List<UTU_Class_Library.Exams>();
            List<Event> Events = new List<UTU_Class_Library.Event>();
            string sql;
            DataSet data = new DataSet();
            DataTable table = new DataTable();
            NpgsqlDataAdapter adapter;

            try
            {
                string connstring = String.Format("Server={0};Port={1};" + "User Id={2};Password={3};Database={4};SSL=true;SslMode=Require;", "ec2-54-197-227-238.compute-1.amazonaws.com", "5432", "yrfxcxkqukzxaa", "N28TYQ_mCVqxrjXin7ZS5tqcRH", "dc734vvumo191f");

                // Making connection with Npgsql provider
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                #region Events
                // quite complex sql statement
                sql = "SELECT * FROM events";
                // data adapter making request from our connection
                adapter = new NpgsqlDataAdapter(sql, conn);

                data.Reset();

                adapter.Fill(data);
                table = data.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    Event e = new Event();
                    e.Name = row.Field<string>("title");
                    e.Description = row.Field<string>("description");
                    e.Place = row.Field<string>("location");
                    e.From = row.Field<DateTime>("event_start");
                    e.To = row.Field<DateTime>("event_end");
                    Events.Add(e);
                }
                #endregion

                #region Exams
                // quite complex sql statement
                sql = "SELECT * FROM exams";
                // data adapter making request from our connection
                adapter = new NpgsqlDataAdapter(sql, conn);

                data.Reset();

                adapter.Fill(data);
                table = data.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    Exams e = new Exams();
                    e.Name = row.Field<string>("title");
                    e.Description = row.Field<string>("description");
                    e.Subject = row.Field<string>("subject");
                    var group = row.Field<int?>("group");
                    if (group == null)
                        e.Group = 0;
                    else
                        e.Group = group.Value;
                    e.Date = row.Field<DateTime>("date");
                    Exams.Add(e);
                }
                #endregion

                #region Tasks
                // quite complex sql statement
                sql = "SELECT * FROM tasks";
                // data adapter making request from our connection
                adapter = new NpgsqlDataAdapter(sql, conn);

                data.Reset();

                adapter.Fill(data);
                table = data.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    Tasks e = new Tasks();
                    e.Name = row.Field<string>("title");
                    e.Description = row.Field<string>("description");
                    e.Subject = row.Field<string>("subject");
                    var group = row.Field<int?>("group");
                    if (group == null)
                        e.Group = 0;
                    else
                        e.Group = group.Value;
                    e.Date = row.Field<DateTime>("date");
                    Tasks.Add(e);
                }
                #endregion

                completed(new Database(Events, Tasks, Exams));
            }
            catch (Exception e)
            {
                Error("Došlo k závažné chybě při načítání dat z databáze. Restarujte aplikaci a kontaktujte autora. " + e.Message);
                DebugMessage(e.Message);
            }
        }

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
            List<Tasks> Tasks = new List<UTU_Class_Library.Tasks>();
            List<Exams> Exams = new List<UTU_Class_Library.Exams>();
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
                    subjects předmět;
                    int skupina;
                    string název;
                    string popis;
                    string splnitDo;
                    int id;

                    předmět = (subjects)Enum.Parse(typeof(subjects), testElement.GetAttribute("subject"));
                    název = testElement.GetAttribute("title");
                    popis = testElement.GetAttribute("description");
                    splnitDo = testElement.GetAttribute("date");
                    skupina = int.Parse(testElement.GetAttribute("group"));
                    id = int.Parse(testElement.GetAttribute("id"));

                    Exams test = new Exams();
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
                    subjects předmět;
                    int skupina;
                    string název;
                    string popis;
                    string splnitDo;
                    int id;

                    předmět = (subjects)Enum.Parse(typeof(subjects), úkolElement.GetAttribute("subject"));
                    název = úkolElement.GetAttribute("title");
                    popis = úkolElement.GetAttribute("description");
                    splnitDo = úkolElement.GetAttribute("date");
                    skupina = int.Parse(úkolElement.GetAttribute("group"));
                    id = int.Parse(úkolElement.GetAttribute("id"));

                    Tasks úkol = new Tasks();
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
