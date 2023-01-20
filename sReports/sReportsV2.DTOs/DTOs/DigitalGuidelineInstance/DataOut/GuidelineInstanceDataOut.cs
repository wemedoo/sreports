using sReportsV2.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuidelineInstance.DataOut
{
    public class GuidelineInstanceDataOut
    {
        public string Id { get; set; }
        public string DigitalGuidelineId { get; set; }
        public int EpisodeOfCareId { get; set; }
        public Period Period { get; set; }
        public string Title { get; set; }
        public List<NodeValue> NodeValues { get; set; } = new List<NodeValue>();
    }
}