using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTU_Class_Library
{
    public class Task : IComparable<Task>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public int Group { get; set; }

        public int Id
        {
            get;
            set;
        }

        public int CompareTo(Task other)
        {
            if (this.Date == other.Date)
                return 0;
            if (this.Date > other.Date)
                return 1;
            return -1;
        }
    }
}
