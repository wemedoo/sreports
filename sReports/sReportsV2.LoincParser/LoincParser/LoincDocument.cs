using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sReportsV2.LoincParser.LoincParser
{
    public class LoincDocument
    {
        public string Name { get; set; }
        public string Kind { get; set; }
        public string TypeOfService { get; set; }
        public string Setting { get; set; }
        public string SubjectMatterDomain { get; set; }
        public string Role { get; set; }
        public string LoincIdentifier { get; set; }

        public void SetProperty(string property, string value)
        {
            switch (property)
            {
                case "Document.Kind":
                    this.Kind = value;
                    break;
                case "Document.TypeOfService":
                    this.TypeOfService = value;
                    break;
                case "Document.Setting":
                    this.Setting = value;
                    break;
                case "Document.Role":
                    this.Role = value;
                    break;
                case "Document.SubjectMatterDomain":
                    this.SubjectMatterDomain = ToUpperFirstLetter(value.Replace("-", " ").Replace(",", " ")).Replace(" ", "");
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
        }

        public string ToUpperFirstLetter(string source)
        {
            return Regex.Replace(source, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
        }

    }
}
