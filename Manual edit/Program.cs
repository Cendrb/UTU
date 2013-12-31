using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Info;
using System.Diagnostics;
using System.Threading;

namespace Manual_edit
{
    class Program
    {
        static void Main(string[] args)
        {
            Nástroje.StáhniXml("suprakindrlo");
            Console.WriteLine("Zadejte hotovo pro ukončení práce a nahrání dat na server nebo reset pro smazání všech dat v databázi nebo open pro manuální úpravu databázového souboru nebo program ukončete a tím zrušíte všechny změny");
            string konzole = Console.ReadLine();
            while (konzole != "hotovo")
            {
                switch (konzole)
                {
                    case "reset":
                        Nástroje.Resetuj();
                        break;
                    case "open":
                        Process.Start("notepad.exe", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databaze.dat"));
                        break;
                    default:
                        Console.WriteLine("Zadejte hotovo pro ukončení práce a nahrání dat na server nebo reset pro smazání všech dat v databázi nebo open pro manuální úpravu databázového souboru nebo program ukončete a tím zrušíte všechny změny");
                        break;
                }
                konzole = Console.ReadLine();
            }
            Console.WriteLine("Data byla nahrána na server\nPokračujte stisknutím libovolné klávesy...");
            Console.ReadKey();

            if (File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databazeAdmin.dat")))
                File.Delete(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databazeAdmin.dat"));
            File.Copy(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databaze.dat"), System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Info", "Databáze", "databazeAdmin.dat"));
            Nástroje.NahrajXml("suprakindrlo");
        }
    }
}
