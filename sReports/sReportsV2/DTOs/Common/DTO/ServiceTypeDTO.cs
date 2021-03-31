using sReportsV2.Common.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common
{
    public class ServiceTypeDTO
    {
        public string Id { get; set; }
        public string ThesaurusId { get; set; }
        public string Display { get; set; }
        public string Definition { get; set; }
        public ServiceTypeDTO() { }

        public ServiceTypeDTO(string thesaurusId, string display, string definition)
        {
            this.ThesaurusId = thesaurusId;
            this.Display = display;
            this.Definition = definition;
        }

        public string GetDisplay() 
        {
            return SingletonDataContainer.Instance.GetServiceTypeByThesaurus(this.Display);
        }
    }
}