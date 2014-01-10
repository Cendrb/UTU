using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows;
using System.Xml;
using System.Net.Mail;

namespace UTU_Class_Library
{
    public class Tools
    {
        public static event Action<string> DebugMessage = delegate {};
        public static event Action<string> ImportantMessage = delegate {};

        public static void DownloadXML(string password)
        {
            try
            {
                Uri uri = new Uri(@"ftp://adis.g6.cz/databaze.dat");
                WebClient webClient = new WebClient();
                webClient.Credentials = new NetworkCredential("adisinfo", password);
                webClient.DownloadFile(uri, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databaze.dat"));
            }
            catch (WebException ex)
            {
                ImportantMessage("Chyba při stahování souboru ze serveru: " + ex.Message);
            }
        }
        public static void UploadXML(string password)
        {
            try
            {
            Uri uri = new Uri(@"ftp://adis.g6.cz/databaze.dat");
            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential("adisinfo", password);
            webClient.UploadFile(uri, WebRequestMethods.Ftp.UploadFile, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databazeAdmin.dat"));
            }
            catch (WebException ex)
            {
                ImportantMessage("Chyba při nahrávání souboru na server: " + ex.Message);
            }
        }
        public static void InitializeFolders()
        {
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
    }
}
