using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Api.Common.Extensions;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Constants;

namespace sReportsV2.Api.Controllers
{
    public class CommonController //: ControllerBase
    {
        /*protected readonly IMapper mapper;
        protected readonly IFormInstanceService formInstanceService;
        protected readonly IEpisodeOfCareService episodeOfCareService;
        protected readonly IEncounterService encounterService;
        protected readonly IFormService formService;


        public CommonController(IMapper mapper, 
            IFormInstanceService formInstanceService, 
            IEpisodeOfCareService episodeOfCareService, 
            IEncounterService encounterService,
            IFormService formService)
        {
            this.mapper = mapper;
            this.formInstanceService = formInstanceService;
            this.episodeOfCareService = episodeOfCareService;
            this.encounterService = encounterService;
            this.formService = formService;
        }

        protected List<FormInstance> GetFormInstances(string patientId)
        {
            List<EpisodeOfCareEntity> episodes = episodeOfCareService.GetByPatientId(patientId);
            List<FormInstance> reports = new List<FormInstance>();
            foreach (EpisodeOfCareEntity episode in episodes)
            {
                reports.AddRange(formInstanceService.GetByEpisodeOfCareId(episode.Id));
            }

            return reports;
        }

        protected Bundle GetFhirElements(string patientId, string type, List<FormInstance> formInstances)
        {
            Bundle bundle = new Bundle();

            foreach (FormInstance formInstance in formInstances)
            {
                if (type == ResourceTypes.Observation)
                {
                    string basePath = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}";
                    bundle.AddObservationsIntoBundle(formInstance, patientId, basePath);
                }
                else if (type == ResourceTypes.Procedure)
                {
                    string basePath = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}";
                    bundle.AddProceduresIntoBundle(formInstance, patientId, basePath);
                }

            }
            return bundle;
        }

        protected Bundle MakeBundleForPatient(string patientId, List<FormInstance> formInstances)
        {
            Bundle patientBundle = new Bundle();
            string basePath = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}";
            foreach (FormInstance formInstance in formInstances)
            {
                DiagnosticReport report = GetDiagnosticReport(formInstance, formInstance.Id);
                string diagReportPath = $"{basePath}/api/DiagnosticReport/{formInstance.Id}";

                patientBundle.AddProceduresIntoBundle(formInstance, patientId, basePath);
                patientBundle.AddObservationsIntoBundle(formInstance, patientId, basePath);
                patientBundle.AddResourceEntry(report, diagReportPath);
            }

            return patientBundle;
        }

        protected OperationOutcome GetError(IssueType issueType) 
        {
            OperationOutcome operationOutcome = new OperationOutcome();
            IssueComponent issue = new IssueComponent
            {
                Severity = IssueSeverity.Error,
                Code = issueType
            };
            operationOutcome.Issue.Add(issue);

            return operationOutcome;
        }

        protected DiagnosticReport GetDiagnosticReport(FormInstance formInstance, string id)
        {
            Form form = new Form(formInstance, formService.GetForm(formInstance.FormDefinitionId) ) { Id = formInstance.Id };
            DiagnosticReport report = new DiagnosticReport();
            report.Identifier.Add(new Identifier(ResourceTypes.O40MtId, formInstance.Id.ToString()));
            report.Subject = new ResourceReference(id);
            report.Issued = formInstance.Date;
            

            foreach (Field observation in form.GetAllObservation())
            {
                report.Result.Add(new ResourceReference($"{formInstance.Id.ToString()}.{observation.Id.Replace('_', '-')}"));
            }

            return report;
        }*/

    }
}
