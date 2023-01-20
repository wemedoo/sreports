using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.SmartOncologyEnums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Contraception
    {
        Needed,
        NotNecessary
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DiseaseContext
    {
        Primary,
        Secondary
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InstanceState
    {
        Active,
        Archived
    }

    public enum TreatmentType
    {
        CancerDirectedTreatment,
        SupportiveTherapy
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChemotherapySchemaInstanceActionType
    {
        DelayDose,
        SaveDose,
        DeleteDose,
        SaveInstance,
        AddMedication,
        ReplaceMedication,
        DeleteMedication
    }
}
