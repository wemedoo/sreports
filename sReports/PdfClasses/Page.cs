using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class Page
    {

        public List<InputGroup> InputGroupList = new List<InputGroup>();
        public string Name { get; set; }
        public int Id { get; set; }

        public Page()
        {

        }

        public Page(string n, int i)
        {
            Name = n;
            Id = i;
        }
    }
}
