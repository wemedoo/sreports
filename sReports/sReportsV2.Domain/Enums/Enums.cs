using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormDefinitionState
    {
        OnGoing,
        ReadyForReview,
        ReadyForDataCapture,
        Disabled
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormFieldDependableType
    {
        Toggle,
        Email
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LayoutType
    {
        Horizontal,
        Vertical
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        Male,
        Female,
        Other,
        Unknown
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EOCStatus
    {
        Planned,
        Waitlist,
        Active,
        Onhold,
        Finished,
        Cancelled,
        EnteredInError
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TelecomSystemType
    {
        Phone,
        Fax,
        Email,
        Pager,
        Url,
        Sms,
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TelecomUseType
    {
        Home,
        Work,
        Temp,
        Old,
        Mobile
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdentifierUseType
    {
        Usual,
        Official,
        Temp,
        Secondary,
        Old,
        
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeOfIdentifier
    {
        IdentifierName,
        IdentifierValue,
        IdentifierUse
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdentifierKind
    {
        Patient,
        Organization
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormState
    {
        Finished,
        OnGoing
    }
}
