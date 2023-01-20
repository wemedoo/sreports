namespace sReportsV2.DTOs.Field.DataIn
{
    public class FieldNumericDataIn : FieldStringDataIn
    {
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Step { get; set; }
    }
}