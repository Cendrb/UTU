
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTU_Class_Library
{
    public class Event : IComparable<Event>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        private int id;
        public int Id
        {
            get;
            set;
        }

        public int CompareTo(Event other)
        {
            if (this.From == other.From)
                return 0;
            if (this.From > other.From)
                return 1;
            return -1;
        }
    }
}
