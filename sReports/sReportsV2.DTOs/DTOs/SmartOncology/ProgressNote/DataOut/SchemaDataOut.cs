using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut
{
    public class SchemaDataOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FirstDay { get; set; }
        public List<SchemaDayDataOut> Days { get; set; } = new List<SchemaDayDataOut>();

        public void AddDay(SchemaDayDataOut day)
        {
            Days.Add(day);
        }
    }
}
