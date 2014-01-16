using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTU_Class_Library;

namespace Console_Edit
{
    class Program
    {
        private static DataLoader dataLoader;
        static void Main(string[] args)
        {
            dataLoader = new DataLoader("suprakindrlo");
            //nastavení událostí databáze
            dataLoader.ImportantMessage += Console.WriteLine;
            dataLoader.Error += Console.WriteLine;
            dataLoader.DebugMessage += Console.WriteLine;

            switch (Console.ReadLine())
            {
                case "copytonewsqldatabase":
                    dataLoader.FTPToMSSQL();
                    break;
            }
            Console.WriteLine("End");
            Console.ReadKey();
        }


    }
}
