using AutoMapper;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Common.Constants;

namespace sReportsV2.Api.Common.Extensions
{
    public static class BundleExtension
    {
        /*public static void AddProceduresIntoBundle(this Bundle bundle, FormInstance formInstance, string patientId, string basePath)
        {
            FormService service = new FormService();
            Form formm = service.GetForm(formInstance.FormDefinitionId);
            Form form = new Form(formInstance, formm);

            foreach (var field in form.GetAllProcedure().OfType<FieldSelectable>())
            {
                Procedure procedure = new Procedure();
                procedure.Status = EventStatus.Completed;
                procedure.Code = new CodeableConcept("O40MtId", field.ThesaurusId, field.Label);
                foreach (FormFieldValue reasonCode in field.Values)
                {
                    foreach (string value in field.Value[0].Split(',').Where(x => x.Equals(reasonCode.Label)))
                    {
                        procedure.ReasonCode.Add(new CodeableConcept(ResourceTypes.O40MtId, reasonCode.ThesaurusId, reasonCode.Value, ""));
                    }
                }
                procedure.Performer.Add(new Procedure.PerformerComponent() { Actor = new ResourceReference(formInstance.UserId.ToString()) });
                procedure.Subject = new ResourceReference(patientId);
                procedure.Id = $"{"Procedure"}/{formInstance.Id.ToString()}.{field.Id.Replace('_', '-')}";

                string path = $"{basePath}/{"api/Procedure"}/{formInstance.Id}.{field.Id.Replace('_', '-')}";
                bundle.AddResourceEntry(procedure, path);
            }
        }

        public static void AddObservationsIntoBundle(this Bundle bundle, FormInstance formInstance, string patientId, string basePath)
        {
            FormService service = new FormService();
            Form formm = service.GetForm(formInstance.FormDefinitionId);
            Form form = new Form(formInstance, formm) { Id = formInstance.Id };

            foreach (Field field in form.GetAllObservation())
            {
                Observation observation = new Observation();
                observation.Status = ObservationStatus.Final;
                observation.Code = new CodeableConcept("O40MtId", field.ThesaurusId, field.Label);
                observation.Value = new FhirString(field.Value[0]);
                observation.Performer.Add(new ResourceReference(formInstance.UserId.ToString()));
                observation.Subject = new ResourceReference(patientId);
                observation.Id = $"{"Observation"}/{formInstance.Id.ToString()}.{field.Id.Replace('_', '-')}";

                string path = $"{basePath}/{"api/Observation"}/{formInstance.Id}.{field.Id.Replace('_', '-')}";
                bundle.AddResourceEntry(observation, path);
            }
        }*/

    }
}
