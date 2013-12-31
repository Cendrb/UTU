using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows;
using System.Xml;

namespace Info
{
    public class Nástroje
    {
        public static void StáhniXml(string password)
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
                MessageBox.Show(ex.Message, "Chyba");
            }
        }
        public static void NahrajXml(string password)
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
                MessageBox.Show(ex.Message, "Chyba");
            }
        }
        public static void Resetuj()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement info = doc.CreateElement("Info");
            XmlDeclaration deklarace = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(deklarace);
            info.AppendChild(doc.CreateElement("Udalosti"));
            info.AppendChild(doc.CreateElement("Ukoly"));
            info.AppendChild(doc.CreateElement("Testy"));
            doc.AppendChild(info);
            doc.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databaze.dat"));
        }
    }
}
