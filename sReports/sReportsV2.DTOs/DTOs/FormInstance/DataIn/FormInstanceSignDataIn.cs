using sReportsV2.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataIn
{
    public class FormInstanceSignDataIn
    {
        [Required]
        public string FormInstanceId { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public FormState FormInstanceNextState { get; set; }
    }
}
