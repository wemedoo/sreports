using sReportsV2.Common.Constants;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.AccessManagment.DataOut
{
    public class ModuleDataOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionDataOut> Permissions { get; set; }

        public string GetModuleImage(string name)
        {
            switch (name)
            {
                case ModuleNames.Designer: return "designer_black.svg";
                case ModuleNames.Simulator: return "simulator_black.svg";
                case ModuleNames.Engine: return "engine_black.svg";
                case ModuleNames.Thesaurus: return "thesaurus_black.svg";
                case ModuleNames.Patients: return "patients_black.svg";
                case ModuleNames.Administration: return "designer_black.svg";
                default: return "overview_black.svg";
            }
        }
    }
}
