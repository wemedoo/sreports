using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class Input
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }

        public Input()
        {

        }

        public Input(string n, string d, string v)
        {
            Name = n;
            Description = d;
            Value = v;
        }
    }
}
