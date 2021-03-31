using Hl7.Fhir.Model;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IPatientService
    {
        string Insert(PatientEntity patient);
        int GetAllEntriesCount(PatientFilter filter);
        List<PatientEntity> GetAll(PatientFilter filter);
        PatientEntity GetById(string id);
        PatientEntity GetByIdentifier(IdentifierEntity identifier);
        bool ExistsPatientByIdentifier(IdentifierEntity identifier);
        bool ExistsPatientByObjectId(string id);
        bool Delete(string patientId, DateTime lastUpdate);
        List<IdentifierType> GetIdentifierTypes(IdentifierKind kind);
        List<PatientEntity> GetByParameters(PatientFhirFilter patientFilter);
        List<PatientEntity> GetByIds(List<string> ids);

        PatientEntity GetExpandedPatientById(string id, string organizationId);
    }
}
