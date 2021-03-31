using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class InputGroup
    {
        public List<Input> InputList = new List<Input>();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }

        public InputGroup()
        {

        }

        public InputGroup(string n, string d, string v)
        {
            Name = n;
            Description = d;
            Value = v;
        }

    }
}
