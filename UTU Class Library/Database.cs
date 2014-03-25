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
        public List<Event> Events { get; private set; }
        public List<Task> Tasks { get; private set; }
        public List<Exam> Exams { get; private set; }

        public Database(List<Event> events, List<Task> tasks, List<Exam> exams)
        {
            Events = events;
            Tasks = tasks;
            Exams = exams;
        }
    }
}
