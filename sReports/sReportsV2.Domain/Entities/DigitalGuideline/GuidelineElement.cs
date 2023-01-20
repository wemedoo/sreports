namespace sReportsV2.Domain.Entities.DigitalGuideline
{
    public class GuidelineElement
    {
        public string Group { get; set; }
        public bool Removed { get; set; }
        public bool Selected { get; set; }
        public bool Selectable { get; set; }
        public bool Locked { get; set; }
        public bool Grabbable { get; set; }
        public bool Pannable { get; set; }
        public string Classes { get; set; }
        public Position Position { get; set; }
        public GuidelineElementData Data { get; set; }
    }
}
