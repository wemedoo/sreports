using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class CommunicationDTO
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public bool Preferred { get; set; }
        public CommunicationDTO() { }
        public CommunicationDTO(string language, bool preferred)
        {
            this.Language = language;
            this.Preferred = preferred;
        }
    }
}