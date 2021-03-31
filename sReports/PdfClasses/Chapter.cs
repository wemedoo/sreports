using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class Chapter
    {
        public List<Page> PageList = new List<Page>();
        public string Name { get; set; }
        public int Id { get; set; }

        public Chapter()
        {

        }

        public Chapter(string n, int i)
        {
            Name = n;
            Id = i;
        }
    }
}
