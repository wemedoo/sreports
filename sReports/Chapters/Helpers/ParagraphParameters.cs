using iText.Kernel.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapters.Helpers
{
    public class ParagraphParameters
    {
        public string Text { get; set; }
        public int FontSize { get; set; }
        public int Left { get; set; }
        public int Bottom { get; set; }
        public int Width { get; set; }
        public int PageCounter { get; set; }
        public DeviceRgb Color { get; set; }

        public ParagraphParameters() { }

        public ParagraphParameters(string text, int fontSize, int left, int bottom, int width, int pageCounter, DeviceRgb color)
        {
            this.Text = text;
            this.FontSize = fontSize;
            this.Left = left;
            this.Bottom = bottom;
            this.Width = width;
            this.PageCounter = pageCounter;
            this.Color = color;
        }

    }
}
