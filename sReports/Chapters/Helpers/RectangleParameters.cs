using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapters
{
    public class RectangleParameters
    {
        public string Text { get; set; }
        public int Position { get; set; }
        public int PageCounter { get; set; }
        public string Type { get; set; }
        public bool IsPencil { get; set; }
        public int NumOfRows { get; set; }

        public int AdditionalPadding { get; set; }

        public RectangleParameters() { }
        public RectangleParameters(string text, int position, int pageCounter, string type)
        {
            this.Text = text;
            this.Position = position;
            this.PageCounter = pageCounter;
            this.Type = type;
            this.IsPencil = false;
            this.NumOfRows = 1;
            this.AdditionalPadding = 0;

        }
        public RectangleParameters(string text, int position, int pageCounter, string type, int additionalPadding)
        {
            this.Text = text;
            this.Position = position;
            this.PageCounter = pageCounter;
            this.Type = type;
            this.IsPencil = false;
            this.NumOfRows = 1;
            this.AdditionalPadding = additionalPadding;

        }

        public RectangleParameters(string text, int position, int pageCounter, string type, bool isPencil, int numOfRows, int additionalPadding)
        {
            this.Text = text;
            this.Position = position;
            this.PageCounter = pageCounter;
            this.Type = type;
            this.IsPencil = isPencil;
            this.NumOfRows = numOfRows;
            this.AdditionalPadding = additionalPadding;
        }
    }

}
