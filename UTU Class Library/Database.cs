using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml;
using System.IO;

namespace UTU_Class_Library
{
    public class Database
    {
        //public DataClasses1DataContext SQL { get; private set; }
        public List<Events> Events { get; private set; }
        public List<Tasks> Tasks { get; private set; }
        public List<Exams> Exams { get; private set; }

        public Database(List<Events> events, List<Tasks> tasks, List<Exams> exams)
        {
            Events = events;
            Tasks = tasks;
            Exams = exams;
        }
    }
}
