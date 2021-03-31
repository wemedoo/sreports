using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chapters
{
    public static class StringExtensions
    {
        public static int GetNumberOfUpperChar(this string text)
        {
            Char[] array = text.ToCharArray();
            int upperCounter = 0;
            foreach (Char ch in array)
            {
                upperCounter += char.IsUpper(ch) ? 1 : 0;
            }

            return upperCounter;
        }
        public static List<string> SplitInParts(this string s, int partLength)
        {
            List<string> listOfValues = new List<string>();

            for (var i = 0; i < s.Length; i += partLength)
                listOfValues.Add(s.Substring(i, Math.Min(partLength, s.Length - i)));

            return listOfValues;
        }

        public static List<string> GetRows(this string value, int rowLenght)
        {
            string text = string.Empty;
            List<string> rows = new List<string>();
            string[] words = value.Split(' ');

            int upperCounter = 0;

            for (int i = 0; i < words.Count(); i++)
            {
                if ((text.Length + words[i].Length) < rowLenght - upperCounter / 3)
                {
                    upperCounter += words[i].GetNumberOfUpperChar();
                    text += words[i] + " ";
                }
                else
                {
                    rows.Add(text);
                    upperCounter = 0;
                    text = words[i] + " ";
                }

                if (i == words.Count() - 1)
                {
                    rows.Add(text);
                }
            }

            return rows;

        }

        public static string ReplaceAllBr(this string value) 
        {
            return value.Replace("<br>","");
        }
    }
}
